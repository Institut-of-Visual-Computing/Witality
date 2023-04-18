using UnityEngine;
using extOSC;
using Oculus.Interaction;
using System.Collections.Generic;
using Oculus.Voice.Demo;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(OSCReceiver))]
public class OpenCVTransRotMapper : MonoBehaviour
{
    OSCReceiver receiver;
    public bool calibration = false;
    public Transform Camera_origin;
    public Transform objects_parent;
    public bool deleteObjectsAfterTime;
    public float deletionTime = 5f;
    public float pos_threshold = 0.005f, rot_threshold = 5f;
    float[] lastTimeTracked;
    public float overrideHeightThreshold = 0.3f;
    public float overrideHeight = 0.743f;
    public float releaseByDistanceThreshold = 0.1f;
    public OVRHand handL, handR;
    public Vector3 inHandOffset;
    [Header("Tracking Software Simulator")]
    public bool simulate = false;
    public float simRate = 0.5f;
    float simTimer = 0;
    [Range(0,10)]
    public int simID;
    public Vector2 noise;
    public Vector3 simPos, simRot;


    [Header("Read Only")]
    public Transform[] objects;
    public List<int> grabbedIDs;
    public float[] standingStillTimer;
    public Vector3[] standingStillPos;
   
    //Data is send in this format
    struct TrackingData
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
        objects = new Transform[50];
        lastTimeTracked = new float[50];
        standingStillPos = new Vector3[50];
        standingStillTimer = new float[50];
        for (int i = 0; i < objects_parent.childCount; i++)
        {
            string n = objects_parent.GetChild(i).Find("id").GetComponent<Renderer>().material.name;
            n = n.Substring(0, 1);
            int id = int.Parse(n);
            objects[id] = objects_parent.GetChild(i);
            objects[id].position = Vector3.down * 5;
        }

        //init UDP
        if (receiver == null)
        {
            Debug.Log("Receiver null!");
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
        if (deleteObjectsAfterTime && !calibration)
            deleteOldObjects(deletionTime);
        grabbedIDs = Grabbable.grabbedArUcoId;
        if (simulate)
            SimulateTracking();
    }

    public void Receive(extOSC.OSCMessage message)
    {
        if (message.ToString(out string msg))
        {
            //read data
            //-------------------------------------------------------------------------------------------------------
            TrackingData data = new TrackingData();
            string[] msgs = msg.Replace('.', ',').Split(' ');
            data.id = int.Parse(msgs[0]);
            data.pos = new Vector3(float.Parse(msgs[1]), float.Parse(msgs[2]), float.Parse(msgs[3]));
            data.pos.y *= -1;

            data.up = new Vector3(float.Parse(msgs[4]), float.Parse(msgs[5]), float.Parse(msgs[6]));
            data.fwd = new Vector3(float.Parse(msgs[7]), float.Parse(msgs[8]), float.Parse(msgs[9]));
            data.up.y *= -1;
            data.fwd.y *= -1;

            if (calibration)
                ProcessCalib(data);
            else
                ProcessData(data);
        }
    }

    void ProcessCalib(TrackingData data)
    {

        int id = data.id;
        Transform o = objects[id];
        if (o == null)
        {
            Debug.Log("Tracked ArUco Id: " + id + " without connected object!");
            return;
        }
        o.position = data.pos;
        o.rotation = Camera_origin.rotation * Quaternion.LookRotation(data.fwd, data.up);
    }
    void ProcessData(TrackingData data)
    {

        //target object
        //-------------------------------------------------------------------------------------------------------
        bool isGrabbed = Grabbable.grabbedArUcoId.Contains(data.id);
        int id = data.id;
        Transform o = objects[id];
        if (o == null)
        {
            Debug.Log("Tracked ArUco Id: " + id + " without connected object!");
            return;
        }


        //position
        //-------------------------------------------------------------------------------------------------------
        //tracked position
        Vector3 trackedPos = new Vector3(data.pos.x, data.pos.y, data.pos.z);
        trackedPos = Camera_origin.position + Camera_origin.rotation * trackedPos;

        //override Height. "magnetic" to table
        lookingUpBehaviour lookingUpBeh = o.GetComponent<lookingUpBehaviour>();
        float newOverrideHeight = (lookingUpBeh != null) ? lookingUpBeh.optimalHeightForTable : overrideHeight;
        float newOverrideTreshold = (lookingUpBeh != null) ? lookingUpBeh.belowYThreshold : overrideHeightThreshold;
        bool isOnTable = Mathf.Abs(trackedPos.y - newOverrideHeight) < newOverrideTreshold;
        Vector3 tablePos = new Vector3(trackedPos.x, newOverrideHeight, trackedPos.z);
        bool standingStill = Vector3.Distance(standingStillPos[id], trackedPos) < pos_threshold;

        if (!isGrabbed) {     //object not in hand
            if (Vector3.Distance((isOnTable ? tablePos : trackedPos), o.position) > pos_threshold)
            {
                if (isOnTable)
                {
                    trackedPos.y = newOverrideHeight;
                    o.position = trackedPos;
                }
                else
                {
                    if (Vector3.Distance(handR.PointerPose.position, trackedPos) > Vector3.Distance(handL.PointerPose.position, trackedPos))
                    {
                         o.position = LineCalibration.Pointer(handL) + o.rotation * inHandOffset;
                    }
                    else
                    {
                        o.position = LineCalibration.Pointer(handR) + o.rotation * inHandOffset;
                    }
                    //snap o to closest hand

                }
            }
        }
        else //object in hand
        {
            //auto release when object is tracked further away than 10cm for more than 2 seconds
            if(Vector3.Distance(o.position, trackedPos) > releaseByDistanceThreshold && standingStillTimer[id] > 2f)
            {
                o.GetComponent<Grabbable>().EndTransform();
                standingStillTimer[id] = 0;

            }
            if (standingStill)
            {
                standingStillTimer[id] += Time.time - lastTimeTracked[id];
            }
        }
        if (!standingStill)
        {
            standingStillTimer[id] = 0;
            standingStillPos[id] = trackedPos;
        }



        //rotation
        //-------------------------------------------------------------------------------------------------------
        //Can be confusing: The ArUcos Fwd and Up are not equal to Unitys Fwd and Up. Up is the Normal of the ArUco Marker. Fwd marking the upside.
        Quaternion trackedRot = Camera_origin.rotation * Quaternion.LookRotation(data.fwd, data.up);
        Vector3 f = data.up;
        f.y = 0;
        //f = f.normalized;
        if (f == Vector3.zero)
        {
            Debug.LogError("ArUcos Up Vector cant be Zero!");
            f = Vector3.forward;
        }
        Quaternion flatRot  = Camera_origin.rotation * Quaternion.LookRotation(Vector3.up, f);
        
        if (isOnTable)
        {
            if (Quaternion.Angle(flatRot, o.rotation) > rot_threshold)
            {
                o.rotation = flatRot;
            }
        }
        else if (!isGrabbed)
        {
            if (Quaternion.Angle(trackedRot, o.rotation) > rot_threshold)
            {
                o.rotation = trackedRot;
            }
        }
        lastTimeTracked[id] = Time.time;
    }

    public void deleteOldObjects(float max_seconds)
    {
        for (int i = 0; i < 50; i++)
        {
            if (lastTimeTracked[i] == 0 || Grabbable.grabbedArUcoId.Contains(i))
                continue;
            if (Time.time - lastTimeTracked[i] > max_seconds)
            {
                objects[i].position = Vector3.down * 5;
                lastTimeTracked[i] = 0;
            }
        }
    }

    void SimulateTracking()
    {

        simTimer += Time.deltaTime;
        if (simTimer > simRate)
        {
            simTimer = 0;
            
            TrackingData data = new TrackingData();
            data.id = simID;
            data.pos = simPos / 100 + RandomVector(noise.x/100);
            Quaternion rot = Quaternion.Euler(simRot + RandomVector(noise.y)) ;
            data.up = rot * Vector3.up;
            data.fwd = rot * Vector3.forward;

            Vector3 origin = Camera_origin.position + Camera_origin.rotation * data.pos;
            Debug.DrawRay(origin, data.up.normalized * 0.1f, Color.green, simRate);
            Debug.DrawRay(origin, data.fwd.normalized * 0.1f, Color.blue, simRate);
            Debug.DrawRay(origin, Vector3.Cross(data.fwd,data.up).normalized * 0.1f, Color.red, simRate);

            ProcessData(data);
        }
    }

    Vector3 RandomVector(float deviation)
    {
        return new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation), Random.Range(-deviation, deviation));
    }
}
