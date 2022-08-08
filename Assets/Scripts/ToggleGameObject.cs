
using UnityEngine;
using TMPro;
public class ToggleGameObject : MonoBehaviour
{


    public GameObject Weinkeller, Sensoriklabor, Konferenzraum, Vinothek, Questionnaire;
    GameObject[] rooms;
    [Space(15)]
    public int active = 1;
    public int lastActiveRoom;

    public TextMeshProUGUI text;
    private void Start()
    {
        
        rooms = new GameObject[] { Weinkeller, Sensoriklabor, Konferenzraum, Vinothek, Questionnaire};
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
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            activate(4);
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
        lastActiveRoom = i == 4 ? lastActiveRoom : i;
        active = i;
        set();
    }

    public void updateText()
    {
        text.text = "Wechsel\nzu\n" + (active == 4 ? int2Name(lastActiveRoom) : "Fragebogen");
    }

    string int2Name(int i)
    {
        return rooms[i].name;
    }

    public void cubeActivation()
    {
        if (active == 4)
        {
            activate(lastActiveRoom);

        }
        else
        {
            activate(4);
        }
    }
}
