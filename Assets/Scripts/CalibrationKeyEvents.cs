using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CalibrationKeyEvents : MonoBehaviour
{
    Calibration calib;
    private void Start()
    {
        calib = GetComponent<Calibration>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            calib.SwitchState(1);
    }
}
