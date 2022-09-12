using System.Collections;
using System.Collections.Generic;
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
        Weiﬂwein_A,
        Weiﬂwein_B,
        Weiﬂwein_C,
        Rotwein_A,
        Rotwein_B
    }
    
    public Task current_task;
    public int subtask;

    
    //x: 1= farbe 2= geschmack 3 = aroma 4 = aroma 5 = cata
    //y: variante
    [Header("0 = Farbe | 1 = Geschmack | 2 = Aroma_r | 3 = Aroma_e | 4 = CATA")]
    public MultiArray[] objects;


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

    void activate(Task t)
    {
        for (int x = 0; x < objects.Length; x++)
        {
           
            for (int y = 0; y < objects[x].Elements.Length; y++)
            {
                GameObject go = objects[x].Elements[y];
                go.SetActive((int) t == x);
            }
        }
        switch (t)
        {
            case Task.Farbe_Rangordnung:
                break;
            case Task.Geschmack_Rangordnung:
                break;
            case Task.Aroma_Rangordnung:
                break;
            case Task.Aroma_Erkennung:
                break;
            case Task.CATA:
                break;
        }
        
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
            case 3:
                return typeof(Task_variante_A);
            case 4:
                return typeof(Task_variante_C);
            default:
                return null;
        }
    }
  
}
