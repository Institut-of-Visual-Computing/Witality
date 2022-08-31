using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationApplier : MonoBehaviour
{
    public Transform ovrRig, realsense;

    private void Awake()
    {
        ovrRig.position = MenuSceneLoader.calibPosition_Hmd;
        ovrRig.rotation = MenuSceneLoader.calibRotation_Hmd;

        realsense.position = MenuSceneLoader.calibPosition_Camera;
        realsense.rotation = MenuSceneLoader.calibRotation_Camera;
    }
}
