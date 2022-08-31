
using UnityEngine;
using TMPro;
public class ToggleEnvironments : MonoBehaviour
{


    public GameObject Weinkeller, Sensoriklabor, Konferenzraum, Vinothek;
    GameObject[] rooms;
    [Space(15)]
    public int active = 1;
    private void Start()
    {
        
        rooms = new GameObject[] { Weinkeller, Sensoriklabor, Konferenzraum, Vinothek};
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
        for (int i = 0; i < rooms.Length; i++)
        {
            if(rooms[i] != null)
                rooms[i].SetActive(i == active);
        }
    }
    
    public void activate(int i)
    {
        active = i;
        set();
    }
}
