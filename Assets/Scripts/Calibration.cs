using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

using static Calibration.CalibState;
public class Calibration : MonoBehaviour
{
    public Transform calibCube, calibCubeShould, hmd, realsense, handR, handL, tableEdge;
    public TextMeshProUGUI info;
    public CalibState state;
    float timer;
    Vector3 lastCubePos;
    Quaternion lastCubeRot;
    public float CamPosSensivity, CamRotSensivity;
    public bool tableDone = false, camDone = false;
    public GameObject continueButton;

    public enum CalibState
    {
        Idle,
        Table1,
        Table2,
        toIdle,
        Cam1,
        Cam2,
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
                info.text += (int)(6 - timer);
                if (timer >= 5)
                    SwitchState(Table2);
                break;

            case Table2:
                info.text = TableCalib();
                SwitchState(toIdle);
                break;
           
            case Cam1:
                info.text = "Kalibriere Kamera.\nBitte nichts verändern!\n";
                info.text += (int)(4 - timer);
                if ((lastCubePos - calibCube.position).magnitude > CamPosSensivity || Quaternion.Angle(lastCubeRot,calibCube.rotation) > CamRotSensivity)
                    timer = 0;
                if (timer >= 3)
                    SwitchState(Cam2);
                lastCubePos = calibCube.position;
                lastCubeRot = calibCube.rotation;
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
                    if (tableDone && (camDone || MenuSceneLoader.task == 0))
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
        Quaternion cubeRot = calibCubeShould.rotation * Quaternion.Inverse(calibCube.rotation);
        realsense.rotation = cubeRot * realsense.rotation;

        Vector3 cubeMove = calibCubeShould.position - calibCube.position;
        realsense.position += cubeMove;

        MenuSceneLoader.calibPosition_Camera = realsense.position;
        MenuSceneLoader.calibRotation_Camera = realsense.rotation;
        camDone = true;
        return "Kamera kalibriert!";
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
