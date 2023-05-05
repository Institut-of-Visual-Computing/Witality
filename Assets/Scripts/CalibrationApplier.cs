using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MenuBehaviour.SavedDataNames;
using static MenuBehaviour;

public class CalibrationApplier : MonoBehaviour
{
    public Transform ovrRig, realsense;
    public OpenCVTransRotMapper tracker;
    public LineCalibration calib;
    public GrabSimulator grab;
    private void Awake()
    {
        if (PlayerPrefs.HasKey(calib_rig_pos + "_x") && ovrRig)
        {
            Debug.Log("Applied OVRRig Pos and Rot.");
            ovrRig.position = PlayerPrefsGetVector3(calib_rig_pos); 
            ovrRig.rotation = Quaternion.Euler(PlayerPrefsGetVector3(calib_rig_rot));
        }
        if (PlayerPrefs.HasKey(calib_cam_pos + "_x") && realsense)
        {
            Debug.Log("Applied Realsense Pos and Rot.");
            realsense.position = PlayerPrefsGetVector3(calib_cam_pos);
            realsense.rotation = Quaternion.Euler(PlayerPrefsGetVector3(calib_cam_rot));
        }
        if (PlayerPrefs.HasKey(sensivity_pos) && tracker)
        {
            Debug.Log("Applied Sensivity Values for Tracking.");
            tracker.pos_threshold = PlayerPrefs.GetInt(sensivity_pos) / 100f;
            tracker.rot_threshold = PlayerPrefs.GetInt(sensivity_rot);
        }
        if (PlayerPrefs.HasKey(sensivity_pos) && calib)
        {
            Debug.Log("Applied Calibration Values.");
            calib.camPosSensivity = PlayerPrefs.GetInt(sensivity_pos) / 100f;
            calib.camRotSensivity = PlayerPrefs.GetInt(sensivity_rot);
            calib.pinchThreshold = PlayerPrefs.GetInt(pinchDistance) / 100f;
        }
        if(PlayerPrefs.HasKey(sensivity_pos) && grab)
        {
            Debug.Log("Applied Pinch Distance Value.");
            grab.pinchThreshold = PlayerPrefs.GetInt(pinchDistance) / 100f;
        }

    }
}
