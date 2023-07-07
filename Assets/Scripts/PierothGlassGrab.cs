using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierothGlassGrab : MonoBehaviour
{
    public OVRHand handL, handR;
    OVRHand hand;
    Transform thumbL, thumbR, indexL, indexR;
    public Transform[] glasses;
    Transform glas;
    public float pinchThreshold = 0.035f;
    public float yawOffset = 25f;
    

    // Update is called once per frame
    void Update()
    {
        SetThumbAndIndex();

        if (!SetActiveGlass())
            return;
        if(!SetActiveHand())
            return;

        Vector3 p = GetPos(hand == handL);
        glas.rotation = GetRot(hand == handL);

        Vector3 handle = glas.Find("rotation") ? (glas.rotation * glas.Find("rotation").localPosition) : Vector3.zero;

        glas.position = p - handle;
         
        

    }

    void SetThumbAndIndex()
    {
        if (!thumbL || !thumbR || !indexL || !indexR)
        {
            thumbL = Maths.GetThumb(handL.transform);
            thumbR = Maths.GetThumb(handR.transform);
            indexL = Maths.GetIndex(handL.transform);
            indexR = Maths.GetIndex(handR.transform);
        }
    }

    bool SetActiveGlass()
    {
        glas = null;
        for (int i = 0; i < glasses.Length; i++)
        {
            if (glasses[i].gameObject.activeSelf)
                glas = glasses[i];
        }

        return glas != null;
    }

    bool SetActiveHand()
    {
        bool pinchingL = Vector3.Distance(thumbL.position, indexL.position) < pinchThreshold;
        bool pinchingR = Vector3.Distance(thumbR.position, indexR.position) < pinchThreshold;

        hand = pinchingR ? handR : (pinchingL ? handL : null);

        return hand != null;
    }

    Vector3 GetPos(bool left)
    {
        return Maths.Avg_v((left?thumbL:thumbR).position, (left?indexL:indexR).position);
    }

    Quaternion GetRot(bool left)
    {
        Transform thumb = left ? thumbL : thumbR;
        Vector3 fwd = (thumb.forward + thumb.up * -1).normalized * 0.1f;
        if (Vector3.Angle(Vector3.up, fwd) > 90)
            fwd *= -1;

        Vector3 up = thumb.right * (left ? -1 : 1);

        Vector3 axis = Vector3.Cross(up, fwd);
        up = Quaternion.AngleAxis(-yawOffset, axis) * up;
        fwd = Quaternion.AngleAxis(-yawOffset, axis) * fwd;


        return Quaternion.LookRotation(fwd, up);
    }
}
