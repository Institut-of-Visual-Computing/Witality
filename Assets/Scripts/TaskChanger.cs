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
        Gelb,
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
        Blumig,
        Pfirsich,
        Kokos
    }
    public enum Task_variante_C
    {
        Weiﬂwein_A,
        Weiﬂwein_B,
        Weiﬂwein_C,
        Rotwein_A,
        Rotwein_B
    }
    
    [HideInInspector] public Task current_task;
    [HideInInspector] public Task_variante_A task_a;  //aroma
    [HideInInspector] public Task_variante_C task_c;  //cata
    [HideInInspector] public Task_variante_F task_f;  //farbe
    [HideInInspector] public Task_variante_G task_g;  //geschmack

    
    //x: 1= farbe 2= geschmack 3 = aroma 4 = aroma 5 = cata
    //y: variante
    [Header("0 = Farbe | 1 = Geschmack | 2 = Aroma_r | 3 = Aroma_e | 4 = CATA")]
    public MultiArray[] objects;
    public OpenCVTransRotMapper tracker;


    public static TaskChanger instance { get; private set;}
    // Start is called before the first frame update
    private void Awake()
    {
        if(instance != null && instance!= this)
            Destroy(this);
        else
            instance = this;
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
                TrackingObjects trackingObj = objects[x].Elements[y].GetComponent<TrackingObjects>();
                if ((int)t == x && trackingObj)
                {
                    tracker.objects_parent = trackingObj.transform;
                    tracker.Init();
                }
            }
        }
        switch (t)
        {
            case Task.Farbe_Rangordnung:
                GameObject.Find("SceneChanger").SetActive(false);
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

  
}
