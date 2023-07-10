using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneLoaderOverwrite : MonoBehaviour
{

    public TaskChanger.Task task;
    public int subtask, environment, probandID;
    public int[] codes;
    public bool demographic, ipq, english, pieroth;

    // Start is called before the first frame update
    void Start()
    {
        MenuSceneLoader.task = task;
        MenuSceneLoader.subtask = subtask;
        MenuSceneLoader.environment = environment;
        MenuSceneLoader.ipq = ipq;
        MenuSceneLoader.english = english;
        MenuSceneLoader.demographic = demographic;
        MenuSceneLoader.pieroth = pieroth;
        MenuSceneLoader.codes = codes;
        MenuSceneLoader.probandID = probandID;
    }

}
