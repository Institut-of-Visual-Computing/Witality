// Author: Tom David Eibich, edited: Paul Krause

using UnityEngine;
using extOSC;

[RequireComponent(typeof(OSCReceiver))]
public class OpenCVTransRotMapper : MonoBehaviour
{
    OSCReceiver receiver;
    public Transform Camera_origin;
    public Transform[] objects;
    public int id_to_obj_offset = -1;

    [Range(0, 1)]
    public float rot_linear_interpolation = 0.85f, pos_linear_interpolation = 0;

    public bool upAndFwd = true;
    Vector3[] lastPos;
    Quaternion[] lastRot;

    [Header("Calibration")]
    public Vector3 offset;
    public Vector3 scaling = Vector3.one;

    [Header("Border")]

    public Material borderMat;
    public KeyCode toggleBorder = KeyCode.B;
    public Vector3 border;
    public GameObject[] borders;
    bool borderActive = false;
    //Data is send in this format
    struct trackingData
    {
        public int id;
        public Vector3 pos;
        public Vector3 rot;
        public Vector3 up;
        public Vector3 fwd;
    }

    // Start is called before the first frame update
    void Start()
    {
        receiver = GetComponent<OSCReceiver>();
        //init interpolation
        lastPos = new Vector3[50];
        lastRot = new Quaternion[50];
        for (int i = 0; i < objects.Length; i++)
        {
            lastPos[i] = objects[i].position;
            lastRot[i] = objects[i].rotation;
        }

        //init UDP
        if (receiver == null)
        {
            Debug.Log("Receiver null.");
        }
        else if (!receiver.IsStarted)
        {
            receiver.Connect();
        }

        receiver.Bind("/msg", Receive);

    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleBorder))
        {
            borderActive = !borderActive;
            showBorderPoints(borderActive);
        }
        if (borderActive)
        {
            positionBorder();
        }
    }

    public void Receive(extOSC.OSCMessage message)
    {
        if (message.ToString(out string msg))
        {
            //read data
            trackingData data = new trackingData();
            string[] msgs = msg.Replace('.', ',').Split(' ');
            data.id = int.Parse(msgs[0]);
            data.pos = new Vector3(float.Parse(msgs[1]), float.Parse(msgs[2]), float.Parse(msgs[3]));
            data.pos.y *= -1;
            if (upAndFwd)
            {
                data.up = new Vector3(float.Parse(msgs[4]), float.Parse(msgs[5]), float.Parse(msgs[6]));
                data.fwd = new Vector3(float.Parse(msgs[7]), float.Parse(msgs[8]), float.Parse(msgs[9]));
                data.up.y *= -1;
                data.fwd.y *= -1;
            }
            else
                data.rot = new Vector3(float.Parse(msgs[4]), float.Parse(msgs[5]), float.Parse(msgs[6]));

            //target object
            int id = data.id;
            Transform o = objects[id + id_to_obj_offset];

            //position

            Vector3 newPos = new Vector3(data.pos.x * scaling.x, data.pos.y * scaling.y, data.pos.z * scaling.z);
            newPos += offset;
            newPos = Camera_origin.position + Camera_origin.rotation * newPos;
            o.position = Vector3.Lerp(newPos, lastPos[id], pos_linear_interpolation);
            lastPos[id] = o.position;

            //rotation
            if (upAndFwd)
            {


                o.rotation = Quaternion.Lerp(Camera_origin.rotation * Quaternion.LookRotation(data.fwd, data.up), lastRot[id], rot_linear_interpolation);

            }
            else
            {
                Vector3 vector = data.rot;
                float theta = vector.magnitude * 180f / Mathf.PI;
                Vector3 axis = new Vector3(-vector.x, vector.y, -vector.z);

                o.rotation = Quaternion.Lerp(Camera_origin.rotation * Quaternion.AngleAxis(theta, axis), lastRot[id], rot_linear_interpolation);
            }
            lastRot[id] = o.rotation;
        }
    }

    public void showBorderPoints(bool activate)
    {
        if (borders == null || borders.Length != 4)
        {
            borders = new GameObject[4];
            for (int i = 0; i < 4; i++)
            {
                borders[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(borders[i].GetComponent<Collider>());
                borders[i].transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);
                borders[i].GetComponent<Renderer>().material = borderMat;

                //borders[i].transform.parent = HMD_cam;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            borders[i].SetActive(activate);
        }

        Debug.Log("Border " + activate);

    }

    public void positionBorder()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 pos = Vector3.zero;
            pos.x = (i > 1) ? -border.x : border.x;
            pos.y = (i % 2 == 0) ? -border.y : border.y;
            pos.z = 0.3f;
            pos.x *= scaling.x; pos.y *= scaling.y; pos.z *= scaling.z;
            pos += offset;

            borders[i].transform.position = Camera_origin.position + Camera_origin.rotation * pos;
        }
    }
}
