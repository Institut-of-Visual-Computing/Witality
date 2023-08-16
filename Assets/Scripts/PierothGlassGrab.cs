using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PierothGlassGrab : MonoBehaviour
{
    public OVRHand handL, handR;
    public GameObject rayL, rayR;
    OVRHand hand;
    Transform thumbL, thumbR, indexL, indexR;
    public Transform[] glasses;
    Transform glas;
    public float pinchThreshold = 0.035f;
    public float yawOffset = 25f;
    
    public enum HandLockState
    {
        dynamic,
        left,
        right
    }
    public HandLockState handLock;

    // Update is called once per frame
    void Update()
    {
        CheckHandLock();
        SetThumbAndIndex();
        rayL.SetActive(glas == null ? true : hand != handL);
        rayR.SetActive(glas == null ? true : hand != handR);

        if (!SetActiveGlass())
            return;
        if (!SetActiveHand())
            return;

        glas.rotation = GetRot(hand == handL);
        glas.position = GetPos(hand == handL);

       

    }
    void CheckHandLock()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            handLock = handLock == HandLockState.left ? HandLockState.dynamic : HandLockState.left;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            handLock = handLock == HandLockState.right ? HandLockState.dynamic : HandLockState.right;
        }
    }
    void SetThumbAndIndex()
    {

        if (!thumbL || thumbL == handL.transform)
            thumbL = Maths.GetThumb(handL.transform); 

        if (!thumbR || thumbR == handR.transform)
            thumbR = Maths.GetThumb(handR.transform);

        if (!indexL || indexL == handL.transform)
            indexL = Maths.GetIndex(handL.transform);

        if (!indexR || indexR == handR.transform)
            indexR = Maths.GetIndex(handR.transform);
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
        switch (handLock)
        {
            case HandLockState.dynamic:
                bool pinchingL = Vector3.Distance(thumbL.position, indexL.position) < pinchThreshold;
                bool pinchingR = Vector3.Distance(thumbR.position, indexR.position) < pinchThreshold;

                Debug.DrawLine(thumbL.position, indexL.position, pinchingL ? Color.green : Color.red);
                Debug.DrawLine(thumbR.position, indexR.position, pinchingR ? Color.green : Color.red);

                if (!pinchingL && !pinchingR)
                    hand = null;

                else if (pinchingL && pinchingR && hand != null)
                    return hand != null;

                else if (pinchingR)
                    hand = handR;
                else if (pinchingL)
                    hand = handL;
                else
                    hand = null;
                break;
            case HandLockState.left:
                hand = handL;
                break;
            case HandLockState.right:
                hand = handR;
                break;
        }
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
    
    public void AdjustYaw(int value)
    {
        yawOffset += value;
    }
}
