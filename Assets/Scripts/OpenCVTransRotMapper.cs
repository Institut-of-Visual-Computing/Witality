using UnityEngine;
using extOSC;
using Oculus.Interaction;
using System.Collections.Generic;
using Oculus.Voice.Demo;
using UnityEngine.Rendering.Universal;
using static Maths;
[RequireComponent(typeof(OSCReceiver))]
public class OpenCVTransRotMapper : MonoBehaviour
{
    OSCReceiver receiver;
    [Header("Calibration")]
    public bool calibration = false;
    public LineCalibration lineCalib;

    [Header("Tracking")]
    public Transform Camera_origin;
    public Transform objects_parent;
    public OVRHand handL, handR;

    [Header("Automated Parameter")]
    public bool deleteObjectsAfterTime;
    public float deletionTime = 5f;
    public bool autoRelease;
    public float releaseByDistanceThreshold = 0.1f;
    public bool useGrabSimulatorWhenNotOnTable = false;
    public bool snapToTable = true;
    public float overrideHeightThreshold = 0.3f;
    public float overrideHeight = 0.743f;
    public Vector3 inHandOffset;

    [Header("Tracking Parameter")]
    public bool[] invertPosXYZRotXYZ;
    public float pos_threshold = 0.005f, rot_threshold = 5f;
    float[] lastTimeTracked;
    Transform thumbL, thumbR, indexL, indexR;
    public float updateRate = 0.1f;
    float updateRateTimer;
    List<TrackingData>[] buffer;
    GrabSimulator grabSimulator;
    [Header("Tracking Software Simulator")]
    public bool simulate = false;
    public float simRate = 0.1f;
    float simTimer = 0;
    [Range(0,10)]
    public int simID;
    public Vector2 noise;
    public Vector3 simPos, simRot;


    [Header("Read Only")]
    public Transform[] objects;
    public float[] standingStillTimer;
    Vector3[] standingStillPos;
   
    //Data is send in this format
    struct TrackingData
    {
        public int id;
        public Vector3 pos;
        public Quaternion rot;
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
        buffer = new List<TrackingData>[50];

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
        PinchPos(true);
        grabSimulator = GetComponent<GrabSimulator>();

    }
    private void Update()
    {
        if (!receiver)
            return;
        if (calibration)
            return;

        if (deleteObjectsAfterTime)
            deleteOldObjects(deletionTime);

        if (simulate)
            SimulateTracking();

        
        updateRateTimer += Time.deltaTime;
        if(updateRateTimer > updateRate)
        {
            updateRateTimer = 0;
            ProcessBuffer();
        }
        
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
            


            Vector3 up = new Vector3(float.Parse(msgs[4]), float.Parse(msgs[5]), float.Parse(msgs[6]));
            Vector3 fwd = new Vector3(float.Parse(msgs[7]), float.Parse(msgs[8]), float.Parse(msgs[9]));

            #region Apply Invertions
            if (invertPosXYZRotXYZ[0])
                data.pos.x *= -1;
            if (invertPosXYZRotXYZ[1])
                data.pos.y *= -1;
            if (invertPosXYZRotXYZ[2])
                data.pos.z *= -1;
            if (invertPosXYZRotXYZ[3])
            {
                up.x *= -1;
                fwd.x *= -1;
            }
            if (invertPosXYZRotXYZ[4])
            {
                up.y *= -1;
                fwd.y *= -1;
            }
            if (invertPosXYZRotXYZ[5])
            {
                up.z *= -1;
                fwd.z *= -1;
            }
            #endregion

            data.pos = Camera_origin.position + Camera_origin.rotation * data.pos;
            data.rot = Camera_origin.rotation * Quaternion.LookRotation(fwd, up);

            if (calibration)
                ProcessCalib(data);
            else
                WriteBuffer(data);
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
        o.rotation = data.rot;

        lineCalib.ReceiveTrackingData(id);
    }
    void ProcessData(TrackingData data)
    {

        //target object
        //-------------------------------------------------------------------------------------------------------
        #region Read Parameter
        int id = data.id;
        Transform o = objects[id];
        bool isGrabbed = Grabbable.grabbedArUcoId.Contains(data.id);
        if (o == null)
        {
            Debug.Log("Tracked ArUco Id: " + id + " without connected object!");
            return;
        }

        #endregion
        //position
        //-------------------------------------------------------------------------------------------------------
        #region Set Position
        //tracked position

        //override Height. "magnetic" to table
        lookingUpBehaviour lookingUpBeh = o.GetComponent<lookingUpBehaviour>();
        float newOverrideHeight = (lookingUpBeh != null) ? lookingUpBeh.optimalHeightForTable : overrideHeight;
        float newOverrideTreshold = (lookingUpBeh != null) ? lookingUpBeh.belowYThreshold : overrideHeightThreshold;
        bool isOnTable = (data.pos.y - newOverrideHeight) < newOverrideTreshold;
        Vector3 tablePos = VectorNewY(data.pos, newOverrideHeight);
        bool standingStill = Vector3.Distance(standingStillPos[id], data.pos) < pos_threshold;

        if (!isGrabbed && !grabSimulator.IsGrabbed(o)) {     //object not in hand
            if (!snapToTable)
                o.position = data.pos;
            else if (Vector3.Distance((isOnTable ? tablePos : data.pos), o.position) > pos_threshold) //Only move when moving more than threshold
            {
                if (isOnTable)
                {
                    o.position = tablePos; 
                }
                else
                {
                    //not grabbed & not on table -> position in closest Hand
                    //SetObjectToClosestHand(o, data.pos);
                    if (useGrabSimulatorWhenNotOnTable)
                        grabSimulator.Init(o, data.pos);
                    else
                        o.position = data.pos;
                }
            }else if(o.position.y < newOverrideHeight) //object under table after release
            {
                o.position = tablePos;
            }
        }
        else //object in hand
        {
            //auto release when object is tracked further away than 10cm for more than 2 seconds
            if(autoRelease && Vector3.Distance(o.position, data.pos) > releaseByDistanceThreshold && standingStillTimer[id] > 2f)
            {
                o.GetComponent<Grabbable>().EndTransform();
                grabSimulator.Release(o);
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
            standingStillPos[id] = data.pos;
        }


        #endregion
        //rotation
        //-------------------------------------------------------------------------------------------------------
        #region Set Rotation
        //Can be confusing: The ArUcos Fwd and Up are not equal to Unitys Fwd and Up. Up is the Normal of the ArUco Marker. Fwd marking the upside.
        Vector3 data_fwd = ZeroVector(data.rot * Vector3.up,false,true,false);
        Quaternion tableRot  = Quaternion.LookRotation(Vector3.up, data_fwd);

        if (isOnTable && snapToTable)
        {
            if (Quaternion.Angle(tableRot, o.rotation) > rot_threshold)
            {
                o.rotation = tableRot;
            }
        }
        else if (!isGrabbed)
        {
            if (Quaternion.Angle(data.rot, o.rotation) > rot_threshold)
            {
                o.rotation = data.rot;
            }
        }
        #endregion
        lastTimeTracked[id] = Time.time;
    }
    Vector3 PinchPos(bool left)
    {
        if(!thumbL || !thumbR || !indexL || !indexR)
        {
            thumbL = GetThumb(handL.transform);
            thumbR = GetThumb(handR.transform);
            indexL = GetIndex(handL.transform);
            indexR = GetIndex(handR.transform);
        }

        if (left)
            return Avg_v(thumbL.position, indexL.position);
        else
            return Avg_v(thumbR.position, indexR.position);
    }
    void WriteBuffer(TrackingData data)
    {
        if (buffer[data.id] == null)
            buffer[data.id] = new List<TrackingData>();
        
        buffer[data.id].Add(data);
    }
    void ProcessBuffer()
    {
        for (int i = 0; i < 50; i++)
        {
            if (buffer[i] != null && buffer[i].Count > 0)
            {
                TrackingData avg = new TrackingData();
                avg.id = i;
                avg.pos = Median_v(PosFromBuffer(buffer[i]));
                avg.rot = Median_q(RotFromBuffer(buffer[i]));
                ProcessData(avg);
                buffer[i].Clear();
            }
        }
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
            data.pos = Camera_origin.position + Camera_origin.rotation * (simPos + RandomVector(noise.x));
            data.rot = Quaternion.Euler(simRot + RandomVector(noise.y) );
            Vector3 up = data.rot * Vector3.up, fwd = data.rot * Vector3.forward;
            Debug.DrawRay(data.pos, up.normalized * 0.1f, Color.green, simRate);
            Debug.DrawRay(data.pos, fwd.normalized * 0.1f, Color.blue, simRate);
            Debug.DrawRay(data.pos, Vector3.Cross(fwd, up).normalized * 0.1f, Color.red, simRate);

            ProcessData(data);
        }
    }
    Vector3 RandomVector(float deviation)
    {
        return new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation), Random.Range(-deviation, deviation));
    }
    List<Vector3> PosFromBuffer(List<TrackingData> b)
    {
        List<Vector3> a = new List<Vector3>();
        if (b == null || b.Count == 0)
            return a;

        for (int i = 0; i < b.Count; i++)
        {
            a.Add(b[i].pos);
        }
        return a;
    }
    List<Quaternion> RotFromBuffer(List<TrackingData> b)
    {
        List<Quaternion> a = new List<Quaternion>();
        if (b == null || b.Count == 0)
            return a;

        for (int i = 0; i < b.Count; i++)
        {
            a.Add(b[i].rot);
        }
        return a;
    }
}
