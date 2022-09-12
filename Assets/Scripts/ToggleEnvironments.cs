
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


public class ToggleEnvironments : MonoBehaviour
{


    public GameObject Weinkeller, Sensoriklabor, Konferenzraum, Vinothek;
    [Space(15)]
    public int active = 1;
    private void Start()
    {
        active = MenuSceneLoader.environment;
        activate(active);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            activate(1);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            activate(0);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            activate(2);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            activate(3);
        }
    }

    void set()
    {
        Weinkeller.SetActive(active == 0);
        Sensoriklabor.SetActive(active == 1);
        Konferenzraum.SetActive(active == 2);
        Vinothek.SetActive(active == 3);
    }
    
    public void activate(int i)
    {
        active = i;
        set();
    }

}

