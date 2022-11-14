using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Calibration))]
public class CalibrationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Calibration calib = (Calibration)target;

        if(GUILayout.Button("Calibrate Cam"))
        {
            calib.CamCalib(true);
        }
    }
}
