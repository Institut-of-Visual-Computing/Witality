// Author: Tom David Eibich, edited: Paul Krause

using UnityEngine;
using extOSC;
using Oculus.Interaction;
using System.Collections.Generic;

[RequireComponent(typeof(OSCReceiver))]
public class OpenCVTransRotMapper : MonoBehaviour
{
    OSCReceiver receiver;
    public Transform Camera_origin;
    public Transform objects_parent;
    public bool deleteObjectsAfterTime;
    public float deletionTime = 5f;
    [Range(0, 1)]
    public float rot_linear_interpolation = 0.85f, pos_linear_interpolation = 0;
    public float pos_threshold = 0.005f, rot_threshold = 5f;
    public bool upAndFwd = true;
    Vector3[] lastPos;
    Quaternion[] lastRot;
    float[] lastTimeTracked;
    public bool overrideByHandtracking = true;
    public float overrideHeightThreshold = 0.3f;
    public float overrideHeight = 0.743f;
    [Header("Read Only")]
    public Transform[] objects;
    public List<int> grabbedIDs;
   
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
    public void Init(Transform t = null)
    {
        if(t)
            objects_parent = t;
        receiver = GetComponent<OSCReceiver>();
        //init interpolation
        lastPos = new Vector3[50];
        lastRot = new Quaternion[50];
        objects = new Transform[50];
        lastTimeTracked = new float[50];
        for (int i = 0; i < objects_parent.childCount; i++)
        {
            string n = objects_parent.GetChild(i).Find("id").GetComponent<Renderer>().material.name;
            n = n.Substring(0, 1);
            int id = int.Parse(n);
            objects[id] = objects_parent.GetChild(i);
        }
        for (int i = 0; i < objects.Length; i++)
        {
            if (!objects[i])
                continue;
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
        if (!receiver)
            return;
        if (deleteObjectsAfterTime)
            deleteOldObjects(deletionTime);
        grabbedIDs = Grabbable.grabbedArUcoId;
    }

    public void Receive(extOSC.OSCMessage message)
    {
        if (message.ToString(out string msg))
        {
            //read data
            trackingData data = new trackingData();
            string[] msgs = msg.Replace('.', ',').Split(' ');
            data.id = int.Parse(msgs[0]);
            bool overrideByGrabbable = overrideByHandtracking ? Grabbable.grabbedArUcoId.Contains(data.id): false;
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
            Transform o = objects[id];
            if (o == null)
            {
                Debug.Log("Tracked ArUco Id: " + id + " without connected object!");
                return;
            }
            
            //position

            Vector3 newPos = new Vector3(data.pos.x , data.pos.y , data.pos.z);
            newPos = Camera_origin.position + Camera_origin.rotation * newPos;
            lookingUpBehaviour lookingUpBeh = o.GetComponent<lookingUpBehaviour>();
            float newOverrideHeight = (lookingUpBeh != null) ? lookingUpBeh.optimalHeightForTable : overrideHeight;

            bool isOnTable = Mathf.Abs(o.position.y - newOverrideHeight) < overrideHeightThreshold;
            if (!overrideByGrabbable)    //object in hand
                if (Vector3.Distance(newPos, o.position) > pos_threshold)
                {  //tracked pos threshold to last pos
                    o.position = Vector3.Lerp(newPos, lastPos[id], pos_linear_interpolation);
                    if (isOnTable)
                    {
                        newPos.y = newOverrideHeight;
                        o.position = newPos;
                    }
                        //o.position += Vector3.up * (o.position.y - newOverrideHeight);
                }

            lastPos[id] = o.position;

            //rotation
            if (!overrideByGrabbable)
            {
                if (upAndFwd)//using up and fwd vector in python script or euler angles
                {

                    if (!isOnTable)
                    { //otherwise the LookingUpBehaviour Script will rotate the object upwards
                        Quaternion newRot = Quaternion.Lerp(Camera_origin.rotation * Quaternion.LookRotation(data.fwd, data.up), lastRot[id], rot_linear_interpolation);
                        if (Quaternion.Angle(newRot, o.rotation) > rot_threshold)
                            o.rotation = newRot;
                    }

                }
                else
                {
                    Vector3 vector = data.rot;
                    float theta = vector.magnitude * 180f / Mathf.PI;
                    Vector3 axis = new Vector3(-vector.x, vector.y, -vector.z);

                    Quaternion newRot = Quaternion.Lerp(Camera_origin.rotation * Quaternion.AngleAxis(theta, axis), lastRot[id], rot_linear_interpolation);
                    if (Quaternion.Angle(newRot, o.rotation) > rot_threshold)
                        o.rotation = newRot;
                }
            }
            lastRot[id] = o.rotation;
            lastTimeTracked[id] = Time.time;
        }
    }
    public void deleteOldObjects(float max_seconds)
    {
        for (int i = 0; i < 50; i++)
        {
            if (lastTimeTracked[i] == 0 || (overrideByHandtracking ? Grabbable.grabbedArUcoId.Contains(i) : false))
                continue;
            if( Time.time - lastTimeTracked[i] > max_seconds)
            {
                objects[i].position = Vector3.down * 5;
                lastTimeTracked[i] = 0;
            }
        }
    }
}
