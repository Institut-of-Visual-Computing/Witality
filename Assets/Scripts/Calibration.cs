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
    }

    private void Update()
    {
        //Notfall Tastatur Input wenn Offset zu gro� ist
        if (Input.GetKeyDown(KeyCode.C))
            SwitchState(Table1, true);
        
        timer += Time.deltaTime;
        switch (state)
        {
            case Idle:
                return;

            case Table1:
                info.text = "Bitte beide Zeigefinger an die Tischkante legen!\n";
                info.text += (int)(6 - timer);
                fingerHint.gameObject.SetActive(true);
                if (timer >= 5)
                    SwitchState(Table2);
                break;

            case Table2:
                info.text = TableCalib();
                SwitchState(toIdle);
                fingerHint.gameObject.SetActive(false);
                break;
           
            case Cam1:
                info.text = "Kalibriere Kamera.\nBitte nichts ver�ndern!\n";
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
                    info.text = "";
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

        realsense.rotation = avg_q3(cubeRot[0], cubeRot[1], cubeRot[2]) * realsense.rotation;   //must change if not taking 3 cubes
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
    Quaternion avg_q3(Quaternion q1, Quaternion q2, Quaternion q3)
    {
        return Quaternion.Lerp( Quaternion.Lerp(q1,q2,0.5f), q3, 2f/3f);
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
}
