using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maths;
public class Resetter : MonoBehaviour
{

    public Transform OVRRig, HMD;
    public KeyCode resetButton = KeyCode.Space;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(resetButton))
        {
            ResetHMD();
        }
    }

    private void ResetHMD()
    {
        OVRRig.position += transform.position - HMD.position;
        OVRRig.RotateAround(transform.position, transform.up, Vector3.SignedAngle(VectorNewY(HMD.right, 0), transform.right, transform.up));
    }
}
