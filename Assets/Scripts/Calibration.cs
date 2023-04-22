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
    public Transform realsense;
    public TextMeshProUGUI info;
    public CalibState state;
    float timer;
    Vector3[] lastCubePos;
    Quaternion[] lastCubeRot;
    public float camPosSensivity, camRotSensivity;
    public bool tableDone = false, camDone = false;
    public GameObject continueButton, cameraVisual;
    public static bool needCalibration = true;
    public int timer_step1, timer_step2, timer_step3;
    List<Vector3> camCalibPosValues;
    List<Quaternion> camCalibRotValues;
    public GameObject blackScreen;
    public GameObject[] calibTutorials;
    
    public enum CalibState
    {
        Idle,
        toIdle,
        Cam1,
        Cam2,
        ready
    }
    private void Start()
    {
        lastCubePos = new Vector3[calibCube.Length];
        lastCubeRot = new Quaternion[calibCube.Length];
        setActiveCalibCubes(false);
        clearCamData();
    }

    private void Update()
    {
     
        timer += Time.deltaTime;
        switch (state)
        {
            case Idle:
                return;

            case Cam1:
                cameraVisual.SetActive(true);
                setActiveCalibCubes(true);
                info.text = CamCalib(false);
                info.text += "\n" + (int)(1 + timer_step3 - timer);

                calibTutorials[2].SetActive(true);
                for (int i = 0; i < calibCube.Length; i++)
                {
                    if ((lastCubePos[i] - calibCube[i].position).magnitude > camPosSensivity || Quaternion.Angle(lastCubeRot[i], calibCube[i].rotation) > camRotSensivity)
                    {
                        timer = 0;
                        clearCamData();
                    }
                    lastCubePos[i] = calibCube[i].position;
                    lastCubeRot[i] = calibCube[i].rotation;
                }
                
                if (timer >= timer_step3)
                    SwitchState(Cam2);
                break;

            case Cam2:
                info.text = CamCalib(true);
                SwitchState(toIdle);

                calibTutorials[2].SetActive(false);
                break;

            case toIdle:
                if (timer >= 2)
                {
                    SwitchState(Idle);
                    info.text = "Kalibriere den Tisch und die Kamera solange bis es mit der physischen Welt übereinstimmt.";
                    if (tableDone && (camDone || MenuSceneLoader.task == 0 || !needCalibration))
                        continueButton.SetActive(true);
                }
                break;
            case ready:
                blackScreen.SetActive(true);
                if (timer >= 0.5f)
                    calibrationFinished();
                break;
            default:
                return;
        }
    }
    #region Camera Calibration
    public void clearCamData()
    {
        camCalibPosValues = new List<Vector3>();
        camCalibRotValues = new List<Quaternion>();
    }
    public string CamCalib(bool save)
    {
        if (save)
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
            camDone = true;
            return "Kamera kalibriert!";
        }

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

   

    public void showHints(Transform t, bool a)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            t.GetChild(i).gameObject.SetActive(a);
        }
    }

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
        blackScreen.SetActive(true);
        SceneManager.LoadScene("Main");
        //SceneManager.UnloadSceneAsync("Calibration");
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
