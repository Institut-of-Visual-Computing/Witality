using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneLoader : MonoBehaviour
{
    public static TaskChanger.Task task = TaskChanger.Task.Farbe_Rangordnung;
    public static int subtask = 0;
    public static int environment = 1;
    public static int probandID = 0;
    public static Vector3 calibPosition_Hmd, calibPosition_Camera;
    public static Quaternion calibRotation_Hmd, calibRotation_Camera;
    public static int[] codes;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);   
    }

    
}
