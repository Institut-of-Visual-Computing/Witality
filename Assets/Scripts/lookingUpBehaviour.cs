using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lookingUpBehaviour : MonoBehaviour
{

    public float belowYThreshold = 0.1f;
    public Transform rotationPoint;
    public float optimalHeightForTable = 0.743f;
    public bool showThreshold = false;
    // Update is called once per frame
    void Update()
    {
        if(transform.position.y - optimalHeightForTable < belowYThreshold)
        {
            lookUp();
        }
        if (showThreshold)
        {
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, optimalHeightForTable + belowYThreshold, transform.position.z), Color.red);
        }
    }

    public void lookUp()
    {
        transform.RotateAround(rotationPoint.position, Vector3.Cross(transform.forward, Vector3.up), Vector3.SignedAngle(transform.forward, Vector3.up, Vector3.Cross(transform.forward, Vector3.up)));

    }

    
}
