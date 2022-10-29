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

        if (PlayerPrefs.HasKey("calibration_pos_x"))
        {
            realsense.position = new Vector3(   PlayerPrefs.GetFloat("calibration_pos_x"),
                                                PlayerPrefs.GetFloat("calibration_pos_y"),
                                                PlayerPrefs.GetFloat("calibration_pos_z"));

            realsense.rotation = Quaternion.Euler(new Vector3(  PlayerPrefs.GetFloat("calibration_rot_x"),
                                                                PlayerPrefs.GetFloat("calibration_rot_y"),
                                                                PlayerPrefs.GetFloat("calibration_rot_z")));
        }
    }
}
