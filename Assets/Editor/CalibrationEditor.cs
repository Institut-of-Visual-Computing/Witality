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

        if (GUILayout.Button("Calibrate"))
        {
            Calibration _target = (Calibration)target;

            _target.resetCalibration();
        }
    }
}
