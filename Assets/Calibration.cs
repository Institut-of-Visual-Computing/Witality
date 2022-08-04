using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    public Transform calibCube, calibCubeShouldBe, hmd, ovrRig, realsense, handR, handL;



    public void resetCalibration()
    {

        //Realsense anpassen das CalibCube stimmt

        Vector3 moveRealsense = calibCubeShouldBe.position - calibCube.position;
        Quaternion rotateRealsense = Quaternion.FromToRotation(calibCube.forward, calibCubeShouldBe.forward);

        realsense.position += moveRealsense;
        //realsense.rotation *= rotateRealsense;

        //ovrRig anpassen dass Fingertip stimmt

        

    }
}
