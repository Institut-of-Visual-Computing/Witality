using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Calibration.CalibState;
public class Calibration : MonoBehaviour
{
    public Transform[] calibCube, calibCubeShould;
    public Transform hmd, realsense, handR, handL, tableEdge;
    public TextMeshProUGUI info;
    public Image fingerHint;
    public CalibState state;
    float timer;
    Vector3[] lastCubePos;
    Quaternion[] lastCubeRot;
    public float CamPosSensivity, CamRotSensivity;
    public bool tableDone = false, camDone = false;
    public GameObject continueButton;
    public static bool needCalibration = true;
    public enum CalibState
    {
        Idle,
        Table1,
        Table2,
        toIdle,
        Cam1,
        Cam2,
    }
    private void Start()
    {
        lastCubePos = new Vector3[calibCube.Length];
        lastCubeRot = new Quaternion[calibCube.Length];
        setActiveCalibCubes(false);
    }

    private void Update()
    {
        //Notfall Tastatur Input wenn Offset zu groß ist
        if (Input.GetKeyDown(KeyCode.C))
            SwitchState(Table1, true);
        
        timer += Time.deltaTime;
        switch (state)
        {
            case Idle:
                return;

            case Table1:
                info.text = "Bitte beide Zeigefinger an die Tischkante legen!\n";
                info.text += (int)(16 - timer);
                fingerHint.gameObject.SetActive(true);
                if (timer >= 15)
                    SwitchState(Table2);
                break;

            case Table2:
                info.text = TableCalib();
                SwitchState(toIdle);
                fingerHint.gameObject.SetActive(false);
                break;
           
            case Cam1:
                info.text = "Kalibriere Kamera.\nBitte nichts verändern!\n";
                info.text += (int)(4 - timer);
                for (int i = 0; i < calibCube.Length; i++)
                {
                    if ((lastCubePos[i] - calibCube[i].position).magnitude > CamPosSensivity || Quaternion.Angle(lastCubeRot[i], calibCube[i].rotation) > CamRotSensivity)
                        timer = 0;

                    lastCubePos[i] = calibCube[i].position;
                    lastCubeRot[i] = calibCube[i].rotation;
                }
                
                if (timer >= 3)
                    SwitchState(Cam2);
                break;

            case Cam2:
                info.text = CamCalib();
                SwitchState(toIdle);
                break;

            case toIdle:
                if (timer >= 3)
                {
                    SwitchState(Idle);
                    info.text = "Kalibriere den Tisch und die Kamera solange bis es mit der physischen Welt übereinstimmt.";
                    if (tableDone && (camDone || MenuSceneLoader.task == 0 || !needCalibration))
                        continueButton.SetActive(true);
                }
                break;

            default:
                return;
        }
    }
    #region Camera Calibration
    public string CamCalib()
    {
        Quaternion[] cubeRot = new Quaternion[calibCube.Length];
        Vector3[] cubeMove = new Vector3[calibCube.Length];
        for (int i = 0; i < calibCube.Length; i++)
        {
            cubeRot[i] = calibCubeShould[i].rotation * Quaternion.Inverse(calibCube[i].rotation);
            cubeMove[i] = calibCubeShould[i].position - calibCube[i].position;
        }

        realsense.rotation = avg_q(cubeRot) * realsense.rotation;
        realsense.position += avg_v(cubeMove);

        MenuSceneLoader.calibPosition_Camera = realsense.position;
        MenuSceneLoader.calibRotation_Camera = realsense.rotation;
        PlayerPrefs.SetFloat("calibration_pos_x", realsense.position.x);
        PlayerPrefs.SetFloat("calibration_pos_y", realsense.position.y);
        PlayerPrefs.SetFloat("calibration_pos_z", realsense.position.z);
        PlayerPrefs.SetFloat("calibration_rot_x", realsense.rotation.eulerAngles.x);
        PlayerPrefs.SetFloat("calibration_rot_y", realsense.rotation.eulerAngles.y);
        PlayerPrefs.SetFloat("calibration_rot_z", realsense.rotation.eulerAngles.z);
        PlayerPrefs.Save();
        camDone = true;
        return "Kamera kalibriert!";
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

    #region Table Calibration
    public string TableCalib()
    {
        handL = getFingerTip(handL);
        handR = getFingerTip(handR);
        
        Vector3 l2r = handR.position - handL.position;
        Vector3 edge = tableEdge.forward;

        hmd.position += tableEdge.position - ((handL.position + handR.position) / 2);
        hmd.Rotate(tableEdge.up, Vector3.SignedAngle(l2r, edge, tableEdge.up));

        MenuSceneLoader.calibPosition_Hmd = hmd.position;
        MenuSceneLoader.calibRotation_Hmd = hmd.rotation;

        tableDone = true;
        return "Tisch kalibriert!";
    }
    public static Transform getFingerTip(Transform hand)
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

    public void SwitchState(int s)
    {
        SwitchState(s, true);
    }
    public void SwitchState(CalibState s, bool needIdle = false)
    {
        SwitchState((int)s, needIdle);
    }
    public void SwitchState(int s, bool needIdle = false)
    {
        if (needIdle && state != Idle)
            return;

        state = (CalibState)s;
        timer = 0;

    }
    public void calibrationFinished()
    {
        SceneManager.LoadScene("Main");
        SceneManager.UnloadSceneAsync("Calibration");
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
}
