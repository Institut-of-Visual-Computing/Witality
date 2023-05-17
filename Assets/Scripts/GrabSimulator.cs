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
    public bool setRotation = true;
    public float yawOffset = 25f;
    public float tableHeight = 0.743f;
    public float tableHeightThreshold = 0.1f;
    struct Offset
    {
        public Transform o;
        public Quaternion r;
    }
    void Update()
    {
        posL = PinchPos(true);
        posR = PinchPos(false);
        rotL = PinchRot(true);
        rotR = PinchRot(false);

        for (int i = 0; i < objL.Count; i++)
        {
            CopyPosRot(objL[i], true);
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
        }
        else
        {
            objR.Add(o);
        }
    }
    void CopyPosRot(Transform o, bool left)
    {
        Quaternion q = (left ? rotL : rotR);
        Vector3 p = (left ? posL : posR);

        if (setRotation)
        {
            //linear interpolate as hand gets closer to table
            float t = Mathf.Clamp01((p.y-tableHeight) / tableHeightThreshold);

            o.rotation = Quaternion.Lerp(RotationVertical(q), q, t);
        
        }
        Vector3 handle = o.Find("rotation") ? (o.rotation * o.Find("rotation").localPosition) : Vector3.zero;
        if (p.y - handle.y <= tableHeight)
            p.y = tableHeight + handle.y;

        o.position = p - handle;
        
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
            objL.Clear();
        }
        else
        {
            objR.Clear();
        }
    }
    public void ReleaseAll()
    {
        Release(true);
        Release(false);
    }
    public void Release(Transform o)
    {
        objL.Remove(o);
        objR.Remove(o);
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
        
        Transform thumb = GetThumb(left ? handL.transform : handR.transform);
        //Vector3 fwd = GetWrist(left ? handL.transform : handR.transform).forward.normalized * 0.1f;
        Vector3 fwd = (thumb.forward + thumb.up * -1).normalized * 0.1f;
        if (Vector3.Angle(Vector3.up, fwd) > 90)
            fwd *= -1;

        //Vector3 up = Vector3.Cross((left ? indexL : indexR).position - (left ? thumbL : thumbR).position, fwd).normalized * 0.1f;
        Vector3 up = thumb.right * (left? -1 : 1);

        Vector3 axis = Vector3.Cross(up, fwd);
        up = Quaternion.AngleAxis(-yawOffset, axis) * up;
        fwd = Quaternion.AngleAxis(-yawOffset, axis) * fwd;

        Debug.DrawRay(left ? posL : posR, up, Color.blue);
        Debug.DrawRay(left ? posL : posR, fwd, Color.red);



        return Quaternion.LookRotation(fwd, up);
    }
}
