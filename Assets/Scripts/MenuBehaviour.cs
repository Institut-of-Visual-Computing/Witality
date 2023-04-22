using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour
{
    public TMP_InputField probandID;
    public ToggleGroup sceneToggles;
    public TMP_Dropdown task, subtask;
    public TextMeshProUGUI error;
    public Transform codes;
    public Toggle demo, ipq, english;
    [Header("Options")]
    public TMP_InputField pinchDistance;
    public TMP_InputField posSensivity, rotSensivity;

    private void Start()
    {
        task.options.Clear();
        for (int i = 0; i < System.Enum.GetNames(typeof(TaskChanger.Task)).Length; i++)
        {
            task.options.Add(new TMP_Dropdown.OptionData(((TaskChanger.Task)i).ToString().Replace("_"," ").Replace("Rangordnung","R").Replace("Erkennung","E")));
        }
        LoadData();
        UpdateSubTask();
    }

    public void UpdateSubTask()
    {   
        subtask.options.Clear();
        System.Type t = TaskChanger.Task2Subtask((TaskChanger.Task)task.value);
        int length = System.Enum.GetNames(t).Length;
        for (int i = 0; i < length; i++)
        {
            subtask.options.Add(new TMP_Dropdown.OptionData(System.Enum.GetName(t,i)));
        }
        subtask.RefreshShownValue();
        ResetCodes();


    }
    public void StartPressed(bool withKalibration)
    {
        if(probandID.text == "" ||probandID.text == null)
        {
            error.text = "ProbandID fehlerhaft!";
            return;
        }
        MenuSceneLoader.probandID = int.Parse(probandID.text);
        MenuSceneLoader.task = (TaskChanger.Task) task.value;
        MenuSceneLoader.subtask = subtask.value;
        MenuSceneLoader.demographic = demo.isOn;
        MenuSceneLoader.ipq = ipq.isOn;
        MenuSceneLoader.english = english.isOn;
        Calibration.needCalibration = withKalibration;
        SceneManager.LoadScene("Calibration");
        //SceneManager.UnloadSceneAsync("Menu");
    }
    public void Toggled(int i)
        {
            MenuSceneLoader.environment = i;
        }
    public void UpdateCodes()
    {
        
        for (int i = 0; i < codes.childCount; i++)
        {
            GameObject g = codes.GetChild(i).gameObject;
            if(i < MenuSceneLoader.codes.Length)
            {
                g.SetActive(true);
                g.GetComponent<TMP_InputField>().text = MenuSceneLoader.codes[i].ToString("000.");
            }
            else
            {
                g.SetActive(false);
            }
        }
    }
    public void CopyInputFieldToCode()
    {
        for (int i = 0; i < MenuSceneLoader.codes.Length; i++)
        {
            GameObject g = codes.GetChild(i).gameObject;
            int v = int.Parse(g.GetComponent<TMP_InputField>().text);
            MenuSceneLoader.codes[i] = v;
        }
    }
    public void RandomizeCode()
    {
        for (int i = 0; i < MenuSceneLoader.codes.Length; i++)
        {
            int r = Random.Range(0, MenuSceneLoader.codes.Length);
            int tmp = MenuSceneLoader.codes[r];
            MenuSceneLoader.codes[r] = MenuSceneLoader.codes[i];
            MenuSceneLoader.codes[i] = tmp;
        }
        UpdateCodes();
    }
    public void ResetCodes()
    {
        MenuSceneLoader.codes = getCodes();
        UpdateCodes();
    }
    int[] getCodes()
    {
        switch (task.value)
        {
            case 0: //Farbe
                switch (subtask.value)
                {
                    case 0: //wei�
                        return new int[] { 389, 855, 622, 334, 088, 211 };
                    case 1: //rose
                        return new int[] { 923, 567, 690, 457, 868, 224 };
                    case 2: //rot
                        return new int[] { 101, 745, 978, 156, 512, 279 };
                    default:
                        return new int[] { 0, 0, 0, 0, 0, 0 };
                }
            case 1: //Geschmack
                switch (subtask.value)
                {
                    case 0: //s��
                        return new int[] { 0, 630, 871, 778, 575, 112, 334 };
                    case 1: //Sauer
                        return new int[] { 0, 537, 704, 148, 241, 093, 482 };
                    case 2: //Bitter
                    case 3: //Salzig
                    default:
                        return new int[] { 0, 1, 2, 3, 4, 5, 6 };
                }
            case 2: //aroma r
                switch (subtask.value)
                {
                    case 0: //Pilz
                        return new int[] { 139, 750, 991, 602, 213 };
                    case 1: //Nelke
                        return new int[] { 972, 361, 065, 676, 528 };
                    case 2: //Eisbonbon
                    case 3: //Blumig
                    case 4: //Pfirsich
                        return new int[] { 731, 342, 953, 120, 509 };
                    case 5: //Kokos
                        return new int[] { 898, 287, 046, 435, 583 };
                    default:
                        return new int[] { 1, 2, 3, 4, 5, 6 };
                }
            case 3: //aroma e
                switch (subtask.value)
                {
                    case 0: //Alle
                        return new int[] { 526, 431, 364, 107, 783, 850 };
                    default:
                        return new int[] { 0, 1, 2, 3, 4, 5 };
                }
            case 4: //Cata
                switch (subtask.value)
                {
                    case 0: //Wei�
                        return new int[] { 0, 131, 364, 720 };
                    case 1: //Rot
                        return new int[] { 0, 983, 644};
                    default:
                        return new int[] { 0, 1 };
                }
            default:
                return new int[] { 0, 0, 0, 0, 0, 0 };
        }
    }
    public static bool CATA_isJoker(int i)
    {
        return i == 597 || i == 322;
    }
    public void saveOptions()
    {
        PlayerPrefs.SetFloat("pos_sensivity_cm", float.Parse(posSensivity.text.Replace(",", ".")));
        PlayerPrefs.SetInt("rot_sensivity", int.Parse(rotSensivity.text));
        PlayerPrefs.SetFloat("pinch_distance", float.Parse(pinchDistance.text.Replace(",", ".")));
        
        PlayerPrefs.Save();
    }
    public void loadOptionValues()
    {
        if (!PlayerPrefs.HasKey("pos_sensivity"))
            return;

        posSensivity.text = PlayerPrefs.GetFloat("pos_sensivity_cm").ToString();
        rotSensivity.text = PlayerPrefs.GetInt("rot_sensivity").ToString();
        pinchDistance.text = PlayerPrefs.GetFloat("pinch_distance").ToString();
    }

    public void LoadData()
    {
        probandID.text = PlayerPrefs.GetInt("player_id").ToString();
        english.isOn = PlayerPrefs.GetInt("english") == 1;
        task.value = PlayerPrefs.GetInt("task");
        subtask.value = PlayerPrefs.GetInt("subtask");
    }
    public void saveData()
    {
        PlayerPrefs.SetInt("player_id", int.Parse(probandID.text));
        PlayerPrefs.SetInt("english", english.isOn ? 1 : 0);
        PlayerPrefs.SetInt("task", task.value);
        PlayerPrefs.SetInt("subtask", subtask.value);
        PlayerPrefs.Save();
    }
}
