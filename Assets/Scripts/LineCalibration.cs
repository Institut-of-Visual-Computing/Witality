using Oculus.Interaction.Input;
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
    public float lineResolution = 0.1f;
    public OVRHand handL, handR;
    public Transform ovrRig, tableEdge;
    public GameObject tableButton, Desk;

    bool lastPinchL, lastPinchR;
    LineRenderer lr;
    Vector3 tablePos, tableRight;
    Transform thumb, index;


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
        lr = GetComponent<LineRenderer>();
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

        if (pinchL && !lastPinchL)
            StartPinch(true);
        if (pinchR && !lastPinchR)
            StartPinch(false);

        if ((pinchL && lastPinchL) || (pinchR && lastPinchR))
            HoldPinch();

        if ((!pinchL && lastPinchL) || (!pinchR && lastPinchR))
            EndPinch();

        lastPinchL = pinchL;
        lastPinchR = pinchR;
        Debug.DrawLine(tablePos, tablePos + tableRight, Color.red);
    }
    void StartPinch(bool left)
    {
        lr.positionCount = 1;
        thumb = getThumb((left ? handL : handR).transform);
        index = getIndex((left ? handL : handR).transform);
        lr.SetPosition(0, Pointer());
        Desk.gameObject.SetActive(false);
    }
    void HoldPinch()
    {
        Vector3 pos = Pointer();

        if (Vector3.Distance(pos, lr.GetPosition(lr.positionCount - 1)) > lineResolution)
        {
            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 1, pos);
        }
    }
    void EndPinch()
    {
        List<Vector3> nextVectors = new List<Vector3>();
        List<Vector3> positions = new List<Vector3>();
        positions.Add(lr.GetPosition(0));
        for (int i = 1; i < lr.positionCount; i++)
        {
            nextVectors.Add(lr.GetPosition(i) - lr.GetPosition(i-1));
            positions.Add(lr.GetPosition(i));
        }
        Vector3 median = median_v(nextVectors);
        Vector3 pos = avg_v(deletePeaks(positions,median));
        median.y = 0;


        float tableWidth = 0.75f;
        float tableDepth = 0.8f;
        Vector3 r = median.normalized * tableWidth;
        Vector3 fwd = -Vector3.Cross(Vector3.up, median).normalized * tableDepth;
        lr.positionCount = 5;
        lr.SetPosition(0, pos - r);
        lr.SetPosition(1, pos + r);
        lr.SetPosition(2, pos + r + fwd);
        lr.SetPosition(3, pos - r + fwd);
        lr.SetPosition(4, pos - r);
        tablePos = pos;
        tableRight = median;
        Adjust();
    }
    Vector3 Pointer()
    {
        return (thumb.position + index.position) / 2;
    }
    public void Adjust()
    {
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
        Vector3 m = v[0];
        for (int i = 1; i < v.Count; i++)
        {
            if (Vector3.Angle(m, avg) > Vector3.Angle(v[i], avg))
                m = v[i];
        }

        return m;
    }
    List<Vector3> deletePeaks(List<Vector3> v, Vector3 dir)
    {
        List<Vector3> smooth = new List<Vector3>();
        for (int i = 1; i < v.Count; i++)
        {
            if (Vector3.Angle(dir, v[i] - v[i-1]) < 30)
                smooth.Add(v[i]);
        }
        return smooth;
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
