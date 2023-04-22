using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maths;
public class GrabSimulator : MonoBehaviour
{

    public OVRHand handL, handR;
    public List<Transform> objL, objR;
    Transform thumbL, thumbR, indexL, indexR;
    Quaternion rotL, rotR;
    Vector3 posL, posR;
    public float pinchThreshold=0.035f;
    List<Offset> offset;
    struct Offset
    {
        public Transform o;
        public Quaternion r;
    }
    private void Start()
    {
        offset = new List<Offset>();
    }
    void Update()
    {
        posL = PinchPos(true);
        posR = PinchPos(false);
        rotL = PinchRot(true);
        rotR = PinchRot(false);

        for (int i = 0; i < objL.Count; i++)
        {
            CopyPosRot(objL[i],true);
        }
        for(int i = 0; i < objR.Count; i++)
        {
            CopyPosRot(objR[i], false);
        }


        bool pinchingL = Vector3.Distance(thumbL.position, indexL.position) < pinchThreshold;
        bool pinchingR = Vector3.Distance(thumbR.position, indexR.position) < pinchThreshold;
        if (!pinchingL)
            Release(true);
        if (!pinchingR)
            Release(false);

        Debug.DrawLine(thumbL.position, indexL.position, pinchingL ? Color.green : Color.red);
        Debug.DrawLine(thumbR.position, indexR.position, pinchingR ? Color.green : Color.red);
    }

    public void Init(Transform o, Vector3 trackedPos)
    {
        if (objL.Contains(o) || objR.Contains(o))
            return;

        if (Vector3.Distance(posR, trackedPos) > Vector3.Distance(posL, trackedPos))
        {
            objL.Add(o);
            NewOffset(o, true);
        }
        else
        {
            objR.Add(o);
            NewOffset(o, false);
        }
    }
    void CopyPosRot(Transform o, bool left)
    {
        //o.rotation = (left ? rotL : rotR) * GetOffset(o);
        Vector3 handle = o.Find("rotation") ? (o.rotation * o.Find("rotation").localPosition) : Vector3.zero;
        o.position = (left ? posL : posR) - handle;
        
    }
    public int IsGrabbedInt(Transform o)
    {
        if (objL.Contains(o))
            return 1;
        if (objR.Contains(o))
            return 2;
        return 0;
    }
    public bool IsGrabbed(Transform o)
    {
        return IsGrabbedInt(o) != 0;
    }
    public void Release(bool left)
    {
        if (left)
        {
            for (int i = 0; i < objL.Count; i++)
                RemoveOffset(objL[i]);
            objL.Clear();
        }
        else
        {
            for (int i = 0; i < objR.Count; i++)
                RemoveOffset(objR[i]);
            objR.Clear();
        }
    }
    public void ReleaseAll()
    {
        Release(true);
        Release(false);
        offset.Clear();
    }
    public void Release(Transform o)
    {
        objL.Remove(o);
        objR.Remove(o);
        RemoveOffset(o);
    }
    Vector3 PinchPos(bool left)
    {
        if (!thumbL || !thumbR || !indexL || !indexR)
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
    Quaternion PinchRot(bool left)
    {
        

        Vector3 fwd = GetWrist(left ? handL.transform : handR.transform).forward.normalized * 0.1f;
        Vector3 up = Vector3.Cross(indexL.position - thumbL.position, fwd).normalized * 0.1f;


        if (Vector3.Angle(Vector3.up, fwd) > 90)
            fwd *= -1;


        return Quaternion.LookRotation(fwd, up);
    }
    void NewOffset(Transform o, bool left)
    {
        Offset off = new Offset();
        off.o = o;
        off.r = Quaternion.Inverse(left ? rotL : rotR) * o.rotation;

        offset.Add(off);
    }
    Quaternion GetOffset(Transform o)
    {
        for (int i = 0; i < offset.Count; i++)
        {
            if(offset[i].o == o)
            {
                return offset[i].r;
            }
        }
        Debug.Log("Offset not found");
        return new Quaternion();
    }
    void RemoveOffset(Transform o)
    {
        for (int i = 0; i < offset.Count; i++)
        {
            if (offset[i].o == o)
            {
                offset.RemoveAt(i);
                return;
            }
        }
    }
}
