using Oculus.Interaction.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LineCalibration.CalibState;
public class LineCalibration : MonoBehaviour
{
    [Header("Table")]
    public float lineResolution = 0.1f, minLineLength = 0.3f;
    public OVRHand handL, handR;
    public Transform ovrRig, tableEdge;
    public GameObject tableButton, Desk;
    public Material calibLineMatGood, calibLineMatBad;
    public int latency = 50;
    public float fillRate = 0.01f;
    float fillRateTimer = 0;
    LineRenderer lr;
    Vector3 tablePos, tableRight;
    Vector3 pointL, pointR, medianL,medianR;
    float tableWidth = 0.75f;
    float tableDepth = 0.8f;
    bool adjusted = true;
    List<Vector3> pointsL;
    List<Vector3> pointsR;


    [Header("Camera")]
    public Transform realsense;
    public Transform[] calibCube, calibCubeShould;
    public GameObject cameraVisual;
    public float CamPosSensivity = 0.05f, CamRotSensivity = 15, calibTime;
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
                clearCamData();
                break;

            case camSetup:
                cameraVisual.SetActive(true);
                setActiveCalibCubes(true);
                SwitchState(camRun);
                break;

            case camRun:
                CamCalib();
                checkForSensivity();

                if (stateTimer > calibTime)
                {
                    info.text = "Kalibrierung abgeschlossen!\nStudie wird gestartet.";
                    SwitchState(ready);
                }
                break;

            case ready:
                saveCam();
                if (stateTimer >= 2.5f)
                    blackScreen.SetActive(true);
                if (stateTimer >= 3f)
                    calibrationFinished();
                break;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            calibrationFinished();
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


        bool pinchL = handL.GetFingerIsPinching(OVRHand.HandFinger.Thumb);
        bool pinchR = handR.GetFingerIsPinching(OVRHand.HandFinger.Thumb);
        pointL = Pointer(handL);
        pointR = Pointer(handR);
        fillRateTimer += Time.deltaTime;
        if(fillRateTimer > fillRate)
        {
            pointsL.Add(pointL);
            pointsR.Add(pointR);
            if (pointsL.Count >= latency)
                pointsL.RemoveAt(0);
            if (pointsR.Count >= latency)
                pointsR.RemoveAt(0);
            medianL = median_v(pointsL);
            medianR = median_v(pointsR);
        }

        if (pinchL&&pinchR)
        {
            adjusted = false;
            tableButton.SetActive(false);


            Desk.SetActive(false);

            Vector3 mid = (medianL + medianR) / 2;
            Vector3 r = medianR - mid;
            r.y = 0;
            r = r.normalized * tableWidth;
            Vector3 fwd = -Vector3.Cross(Vector3.up, r).normalized * tableDepth;
            lr.positionCount = 5;
            lr.SetPosition(0, pointL);
            lr.SetPosition(1, pointR);
            lr.SetPosition(2, pointR + fwd);
            lr.SetPosition(3, pointL + fwd);
            lr.SetPosition(4, pointL);

            tablePos = mid;
            tableRight = r;

            lr.material = (Vector3.Distance(pointL, pointR) > 0.4f && Vector3.Distance(pointL, pointR) < 0.7f) ? calibLineMatGood : calibLineMatBad;
        }
        else if (!adjusted)
        {
            Desk.SetActive(true);
            Adjust();
        }

    }
    Vector3 Pointer(OVRHand h)
    {
        return (getThumb(h.transform).position + getIndex(h.transform).position) / 2;
    }
    public void Adjust()
    {
        adjusted = true;
        ovrRig.position += tableEdge.position - tablePos;
        ovrRig.Rotate(tableEdge.up, Vector3.SignedAngle(tableRight, tableEdge.right, tableEdge.up));
        Desk.SetActive(true);
        tablePos = tableEdge.position;
        tableRight = tableEdge.right;
        lr.positionCount = 0;


        MenuSceneLoader.calibPosition_Hmd = ovrRig.position;
        MenuSceneLoader.calibRotation_Hmd = ovrRig.rotation;
        tableButton.SetActive(true);
    }
    Vector3 avg_v(List<Vector3> v)
    {
        if (v.Count == 0)
            return Vector3.zero;
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < v.Count; i++)
        {
            sum += v[i];
        }
        return sum / v.Count;
    }
    Vector3 median_v(List<Vector3> v)
    {
        if (v.Count == 0)
            return Vector3.zero;
        Vector3 avg = avg_v(v);
        float d = float.MaxValue;
        Vector3 min = Vector3.zero;
        for (int i = 0; i < v.Count; i++)
        {
            float dis = Vector3.Distance(v[i], avg);
            if ( dis < d)
            {
                d = dis;
                min = v[i];
            }
        }
        return min;
    }
    public static Transform getThumb(Transform hand)
    {
        Transform tip = hand.Find("Bones/Hand_WristRoot/Hand_Thumb0/Hand_Thumb1/Hand_Thumb2/Hand_Thumb3/Hand_ThumbTip");
        if (tip == null)
            return hand;
        while (tip.childCount > 0)
            tip = tip.GetChild(0);
        return tip;
    }
    public static Transform getIndex(Transform hand)
    {
        Transform tip = hand.Find("Bones/Hand_WristRoot/Hand_Index1/Hand_Index2/Hand_Index3/Hand_IndexTip");
        if (tip == null)
            return hand;
        // hand > Bones > Hand_WristRoot > Hand_Index[1,2,3,Tip]
        while (tip.childCount > 0)
            tip = tip.GetChild(0);
        return tip;
    }
    #endregion
    #region Camera
    void checkForSensivity()
    {
        for (int i = 0; i < calibCube.Length; i++)
        {
            if ((lastCubePos[i] - calibCube[i].position).magnitude > CamPosSensivity || Quaternion.Angle(lastCubeRot[i], calibCube[i].rotation) > CamRotSensivity)
            {
                stateTimer = 0;
                clearCamData();
            }
            lastCubePos[i] = calibCube[i].position;
            lastCubeRot[i] = calibCube[i].rotation;
        }
    }
    public string CamCalib()
    {

        info.text = "Kalibriere Kamera...\n" + (1 + calibTime - stateTimer).ToString("0.0");
        Quaternion[] cubeRot = new Quaternion[calibCube.Length];
        Vector3[] cubeMove = new Vector3[calibCube.Length];
        for (int i = 0; i < calibCube.Length; i++)
        {
            cubeRot[i] = calibCubeShould[i].rotation * Quaternion.Inverse(calibCube[i].rotation);
            cubeMove[i] = calibCubeShould[i].position - calibCube[i].position;
        }

        realsense.rotation = avg_q(cubeRot) * realsense.rotation;
        realsense.position += avg_v(cubeMove);

        camCalibRotValues.Add(realsense.rotation);
        camCalibPosValues.Add(realsense.position);
        return "Kalibriere...";

    }
    public void saveCam()
    {
        realsense.position = avg_v(camCalibPosValues.ToArray());
        realsense.rotation = avg_q(camCalibRotValues.ToArray());

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
    public void clearCamData()
    {
        camCalibPosValues = new List<Vector3>();
        camCalibRotValues = new List<Quaternion>();
    }
    public void calibrationFinished()
    {
        SceneManager.LoadScene("Main");
    }
    public void setActiveCalibCubes(bool active)
    {
        for (int i = 0; i < calibCube.Length; i++)
        {
            for (int j = 0; j < calibCube[i].childCount; j++)
            {
                calibCube[i].GetChild(j).gameObject.SetActive(active);
            }
        }
    }
    Vector3 avg_v(Vector3[] v)
    {
        Vector3 sum = Vector3.zero;
        for (int i = 0; i < v.Length; i++)
        {
            sum += v[i];
        }
        return sum / v.Length;
    }
    Quaternion avg_q(Quaternion[] q)
    {
        if (q == null || q.Length == 0)
            return new Quaternion();
        Quaternion avg = q[0];
        for (int i = 1; i < q.Length; i++)
        {
            avg = Quaternion.Lerp(q[i], avg, 1 / (i + 1));
        }
        return avg;
    }
    #endregion

}
