using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
public class MenuBehaviour : MonoBehaviour
{
    public TMP_InputField probandID;
    public ToggleGroup sceneToggles;
    public TMP_Dropdown task, subtask;
    public TextMeshProUGUI error;

    private void Start()
    {
        task.options.Clear();
        for (int i = 0; i < System.Enum.GetNames(typeof(TaskChanger.Task)).Length; i++)
        {
            task.options.Add(new TMP_Dropdown.OptionData(((TaskChanger.Task)i).ToString()));
        }
        updateSubTask();

    }

    public void updateSubTask()
    {   
        subtask.options.Clear();
        System.Type t = TaskChanger.Task2Subtask((TaskChanger.Task)task.value);
        int length = System.Enum.GetNames(t).Length;
        for (int i = 0; i < length; i++)
        {
            subtask.options.Add(new TMP_Dropdown.OptionData(System.Enum.GetName(t,i)));
        }
        subtask.RefreshShownValue();
    }

    public void startPressed()
    {
        if(probandID.text == "" ||probandID.text == null)
        {
            error.text = "ProbandID fehlerhaft!";
            return;
        }
        MenuSceneLoader.probandID = int.Parse(probandID.text);
        MenuSceneLoader.task = (TaskChanger.Task) task.value;
        MenuSceneLoader.subtask = subtask.value;

        
        SceneManager.LoadScene("Main");

        SceneManager.UnloadSceneAsync("Menu");
    }

    public void toggled(int i)
    {
        MenuSceneLoader.environment = i;
    }
}
