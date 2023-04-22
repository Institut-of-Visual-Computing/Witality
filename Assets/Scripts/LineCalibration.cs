using Oculus.Interaction.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LineCalibration.CalibState;
using static Maths;
public class LineCalibration : MonoBehaviour
{
    [Header("Table")]
    public float pinchThreshold = 0.01f, minDrawingDistance = 0.1f;
    public OVRHand handL, handR;
    public Transform ovrRig, tableEdge;
    public GameObject tableButton, Desk;
    public Material calibLineMatGood, calibLineMatBad;
    public int latency = 50;
    public float fillRate = 0.01f;

    Transform[] pinchL, pinchR;
    float fillRateTimer = 0;
    LineRenderer lr;
    Vector3 tablePos, tableRight;
    Vector3 pointL, pointR, medianL,medianR;
    float tableWidth = 0.75f;
    float tableDepth = 0.8f;
    bool adjusted = true;
    List<Vector3> pointsL;
    List<Vector3> pointsR;
    bool startDrawing = false;

    [Header("Camera")]
    public bool debugging = false;
    public Transform realsense;
    public Transform[] calibCube, calibCubeShould;
    public GameObject cameraVisual;
    public float camPosSensivity = 0.05f, camRotSensivity = 15, calibTime;
    public GameObject blackScreen;
    public TextMeshProUGUI info;


    public CalibState state;
    float stateTimer = 0;
    Vector3[] lastCubePos;
    Quaternion[] lastCubeRot;
    List<Vector3> camCalibPosValues;
    List<Quaternion> camCalibRotValues;
    public enum CalibState
    {
        idle,
        table,
        camStart,
        camSetup,
        camRun,
        ready
    }
    private void Start()
    {
        lastCubePos = new Vector3[calibCube.Length];
        lastCubeRot = new Quaternion[calibCube.Length];
        pointsL = new List<Vector3>();
        pointsR = new List<Vector3>();
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 5;
        state = table;
        SetPinchTransforms();
        ClearCamData();
    }
    // Update is called once per frame
    void Update()
    {
        stateTimer += Time.deltaTime;
        switch (state)
        {
            case idle:
                break;

            case table:
                CheckForPinchStatus();
                break;

            case camStart:
                if (MenuSceneLoader.task == 0)
                    SwitchState(ready);
                ClearCamData();
                break;

            case camSetup:
                lr.material = calibLineMatGood;
                cameraVisual.SetActive(true);
                SetActiveCalibCubes(true);
                SwitchState(camRun);
                break;

            case camRun:
                info.text = "Kalibriere Kamera...\n" + (calibTime - stateTimer).ToString("0.0");
                if (camCalibPosValues.Count == 0 || camCalibRotValues.Count == 0)
                    stateTimer = 0;
                if (stateTimer > calibTime && !debugging)
                {
                    info.text = "Kalibrierung abgeschlossen!\nStudie wird gestartet.";
                    SwitchState(ready);
                }
                break;

            case ready:
                SaveCam();
                if (stateTimer >= 2.5f)
                    blackScreen.SetActive(true);
                if (stateTimer >= 3f)
                    CalibrationFinished();
                break;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SaveCam();
            CalibrationFinished();
        }
    }
    public void SwitchState(int s)
    {
        SwitchState((CalibState)s);
    }
    public void SwitchState(CalibState s)
    {
        state = s;
        stateTimer = 0;
    }
    #region Table
    void CheckForPinchStatus()
    {
        if (!pinchL[0] || !pinchL[1] || !pinchR[0] || !pinchR[1])
            SetPinchTransforms();

        bool pinchingL = IsPinching(pinchL);
        bool pinchingR = IsPinching(pinchR);
        Debug.DrawLine(pinchL[0].position, pinchL[1].position, pinchingL ? Color.green : Color.red);
        Debug.DrawLine(pinchR[0].position, pinchR[1].position, pinchingR ? Color.green : Color.red);

        pointL = Avg_v(pinchL[0].position, pinchL[1].position);
        pointR = Avg_v(pinchR[0].position, pinchR[1].position);
        fillRateTimer += Time.deltaTime;
        if(fillRateTimer > fillRate)
        {
            pointsL.Add(pointL);
            pointsR.Add(pointR);
            if (pointsL.Count >= latency)
                pointsL.RemoveAt(0);
            if (pointsR.Count >= latency)
                pointsR.RemoveAt(0);
            medianL = Median_v(pointsL);
            medianR = Median_v(pointsR);
        }

        if (Vector3.Distance(pointL, pointR) < minDrawingDistance && pinchingL && pinchingR)
            startDrawing = true;

        if(!pinchingL || !pinchingR)
            startDrawing = false;

        if (startDrawing)
        {
            startDrawing = true;
            adjusted = false;
            tableButton.SetActive(false);
            Desk.SetActive(false);

            Vector3 mid = (medianL + medianR) / 2;
            Vector3 r = medianR - mid;
            r.y = 0;
            r = r.normalized * tableWidth;
            Vector3 fwd = -Vector3.Cross(Vector3.up, r).normalized * tableDepth;
            lr.positionCount = 5;
            lr.SetPosition(0, medianL);
            lr.SetPosition(1, medianR);
            lr.SetPosition(2, medianR + fwd);
            lr.SetPosition(3, medianL + fwd);
            lr.SetPosition(4, medianL);

            tablePos = mid;
            tableRight = r;

            lr.material = (Vector3.Distance(pointL, pointR) > 0.4f && Vector3.Distance(pointL, pointR) < 0.85f) ? calibLineMatGood : calibLineMatBad;
            
        }
        if (!adjusted && (!pinchingL || !pinchingR))
        {
            Desk.SetActive(true);
            Adjust();
        }

    }
    public void Adjust()
    {
        adjusted = true;
        ovrRig.position += tableEdge.position - tablePos;
        ovrRig.RotateAround(tableEdge.position, tableEdge.up, Vector3.SignedAngle(tableRight, tableEdge.right, tableEdge.up));
        Desk.SetActive(true);
        tablePos = tableEdge.position;
        tableRight = tableEdge.right;
        lr.positionCount = 0;


        MenuSceneLoader.calibPosition_Hmd = ovrRig.position;
        MenuSceneLoader.calibRotation_Hmd = ovrRig.rotation;
        tableButton.SetActive(true);
    }
    void SetPinchTransforms()
    {
        pinchL = new Transform[2];
        pinchR = new Transform[2];
        pinchL[0] = GetThumb(handL.transform);
        pinchL[1] = GetIndex(handL.transform);
        pinchR[0] = GetThumb(handR.transform);
        pinchR[1] = GetIndex(handR.transform);
    }
    bool IsPinching(Transform[] fingers)
    {
        return Vector3.Distance(fingers[0].position, fingers[1].position) < pinchThreshold;
    }
    #endregion
    #region Camera
    public void ReceiveTrackingData(int id)
    {
        if (state != camRun)
            return;
        //TODO bewegt sich in die Falsche richtung
        Vector3 cubeToCam = realsense.position - calibCube[id].position;
        Quaternion cubeRot = calibCubeShould[id].rotation * Quaternion.Inverse(calibCube[id].rotation);


        realsense.rotation = cubeRot * realsense.rotation;
        realsense.position = calibCubeShould[id].position + cubeRot * cubeToCam;

        camCalibRotValues.Add(realsense.rotation);
        camCalibPosValues.Add(realsense.position);
        lr.positionCount++;
        lr.SetPosition(lr.positionCount - 1, realsense.position);
        if ((lastCubePos[id] - calibCube[id].position).magnitude > camPosSensivity || Quaternion.Angle(lastCubeRot[id], calibCube[id].rotation) > camRotSensivity)
        {
            stateTimer = 0;
            ClearCamData();
        }
        lastCubePos[id] = calibCube[id].position;
        lastCubeRot[id] = calibCube[id].rotation;
    }
   
    public void SaveCam()
    {
        if (camCalibPosValues.Count > 0)
        {
            realsense.position = Avg_v(camCalibPosValues);
            realsense.rotation = Avg_q(camCalibRotValues);
        }
        MenuSceneLoader.calibPosition_Camera = realsense.position;
        MenuSceneLoader.calibRotation_Camera = realsense.rotation;

        PlayerPrefs.SetFloat("calibration_pos_x", realsense.position.x);
        PlayerPrefs.SetFloat("calibration_pos_y", realsense.position.y);
        PlayerPrefs.SetFloat("calibration_pos_z", realsense.position.z);
        PlayerPrefs.SetFloat("calibration_rot_x", realsense.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("calibration_rot_y", realsense.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("calibration_rot_z", realsense.rotation.eulerAngles.z);
        PlayerPrefs.Save();
    }
    public void ClearCamData()
    {
        lr.positionCount = 0;
        camCalibPosValues = new List<Vector3>();
        camCalibRotValues = new List<Quaternion>();
    }
    public void CalibrationFinished()
    {
        SceneManager.LoadScene("Main");
    }
    public void SetActiveCalibCubes(bool active)
    {
        for (int i = 0; i < calibCube.Length; i++)
        {
            for (int j = 0; j < calibCube[i].childCount; j++)
            {
                calibCube[i].GetChild(j).gameObject.SetActive(active);
            }
        }
    }
    

    
    #endregion

}
