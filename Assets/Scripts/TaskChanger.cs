using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class TaskChanger : MonoBehaviour
{
    public enum Task
    {
        Farbe_Rangordnung,
        Geschmack_Rangordnung,
        Aroma_Rangordnung,
        Aroma_Erkennung,
        CATA
    }
    public enum Task_variante_F
    {
        Weiﬂ,
        Rose,
        Rot
    }
    public enum Task_variante_G
    {
        S¸ﬂ,
        Sauer,
        Bitter,
        Salzig
    }
    public enum Task_variante_E
    {
        Alle_Aromen
    }
    public enum Task_variante_A
    {
        Pilz,
        Nelke,
        Eisbonbon,
        Kokos,
        Pfirsich,
        Blume,
        Veilchen,
        Ananas,
        Orange,
        Banane,
        Vanille,
        Erde
    }
    public enum Task_variante_C
    {
        Weiﬂwein_Riesling,
        Weiﬂwein_Gew¸rztraminer,
        Weiﬂwein_Grauburgunder,
        Rotwein_A,
        Rotwein_B
    }
    
    public Task current_task;
    public int subtask;

    
    //x: 1= farbe 2= geschmack 3 = aroma 4 = aroma 5 = cata
    //y: variante
    [Header("0 = Farbe | 1 = Geschmack | 2 = Aroma_r | 3 = Aroma_e | 4 = CATA")]
    public GameObject[] objects;
    public GameObject additional_obj_task_1, additional_obj_task_2;
    public static TaskChanger instance { get; private set;}
    
    
    
    // Start is called before the first frame update
    private void Awake()
    {
        if(instance != null && instance!= this)
            Destroy(this);
        else
            instance = this;

        current_task = MenuSceneLoader.task;
        subtask = MenuSceneLoader.subtask;

    }
    void Start()
    {
        activate(current_task);
    }

    public void activate(Task t)
    {
        for (int x = 0; x < objects.Length; x++)
        {
            objects[x].SetActive((int)t == x);
        }
        additional_obj_task_1.SetActive((int)t == 1);
        additional_obj_task_2.SetActive((int)t == 2);
        
    }
    public void activate()
    {
        activate(current_task);
    }
    public static System.Type Task2Subtask(Task t)
    {
        switch ((int)t)
        {
            case 0:
                return typeof(Task_variante_F);
            case 1:
                return typeof(Task_variante_G);
            case 2:
                return typeof(Task_variante_E);
            case 3:
                return typeof(Task_variante_A);
            case 4:
                return typeof(Task_variante_C);
            default:
                return null;
        }
    }
  

}

[CustomEditor(typeof(TaskChanger))]
public class TaskChangerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TaskChanger t = (TaskChanger)target;

        if(GUILayout.Button("Apply Task"))
        {
            t.activate();
        }
    }
}
