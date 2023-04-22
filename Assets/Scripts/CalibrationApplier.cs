using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationApplier : MonoBehaviour
{
    public Transform ovrRig, realsense;
    public OpenCVTransRotMapper tracker;
    public LineCalibration calib;
    public GrabSimulator grab;
    private void Awake()
    {
        if (ovrRig)
        {
            ovrRig.position = MenuSceneLoader.calibPosition_Hmd;
            ovrRig.rotation = MenuSceneLoader.calibRotation_Hmd;
        }
        if (PlayerPrefs.HasKey("calibration_pos_x") && realsense)
        {
            realsense.position = new Vector3(   PlayerPrefs.GetFloat("calibration_pos_x"),
                                                PlayerPrefs.GetFloat("calibration_pos_y"),
                                                PlayerPrefs.GetFloat("calibration_pos_z"));

            realsense.rotation = Quaternion.Euler(new Vector3(  PlayerPrefs.GetFloat("calibration_rot_x"),
                                                                PlayerPrefs.GetFloat("calibration_rot_y"),
                                                                PlayerPrefs.GetFloat("calibration_rot_z")));
        }
        if (PlayerPrefs.HasKey("pos_sensivity") && tracker)
        {
            tracker.pos_threshold = PlayerPrefs.GetFloat("pos_sensivity_cm") / 100f;
            tracker.rot_threshold = PlayerPrefs.GetInt("rot_sensivity");
        }
        if (PlayerPrefs.HasKey("pos_sensivity") && calib)
        {
            calib.camPosSensivity = PlayerPrefs.GetFloat("pos_sensivity_cm") / 100f;
            calib.camRotSensivity = PlayerPrefs.GetInt("rot_sensivity");
            calib.pinchThreshold = PlayerPrefs.GetFloat("pinch_distance") / 100f;
        }
        if(PlayerPrefs.HasKey("pos_sensivity") && grab)
        {
            grab.pinchThreshold = PlayerPrefs.GetFloat("pinch_distance") / 100f;
        }

    }
}
