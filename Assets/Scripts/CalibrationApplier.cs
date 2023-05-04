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
            ovrRig.position = PlayerPrefsGetVector3(calib_rig_pos); 
            ovrRig.rotation = Quaternion.Euler(PlayerPrefsGetVector3(calib_rig_rot));
        }
        if (PlayerPrefs.HasKey(calib_cam_pos+"_x") && realsense)
        {
            realsense.position = PlayerPrefsGetVector3(calib_cam_pos);

            realsense.rotation = Quaternion.Euler(PlayerPrefsGetVector3(calib_cam_rot));
        }
        if (PlayerPrefs.HasKey(sensivity_pos) && tracker)
        {
            tracker.pos_threshold = PlayerPrefs.GetFloat(sensivity_pos) / 100f;
            tracker.rot_threshold = PlayerPrefs.GetInt(sensivity_rot);
        }
        if (PlayerPrefs.HasKey(sensivity_pos) && calib)
        {
            calib.camPosSensivity = PlayerPrefs.GetFloat(sensivity_pos) / 100f;
            calib.camRotSensivity = PlayerPrefs.GetInt(sensivity_rot);
            calib.pinchThreshold = PlayerPrefs.GetFloat(pinchDistance) / 100f;
        }
        if(PlayerPrefs.HasKey(sensivity_pos) && grab)
        {
            grab.pinchThreshold = PlayerPrefs.GetFloat(pinchDistance) / 100f;
        }

    }
}
