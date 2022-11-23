using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookingUpBehaviour : MonoBehaviour
{

    public float belowYThreshold = 0.78f;
    public Transform rotationPoint;
    public float optimalHeightForTable = 0.743f;

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < belowYThreshold)
        {
            lookUp();
        }
    }

    public void lookUp()
    {
        transform.RotateAround(rotationPoint.position, Vector3.Cross(transform.forward, Vector3.up), Vector3.SignedAngle(transform.forward, Vector3.up, Vector3.Cross(transform.forward, Vector3.up)));

    }
}
