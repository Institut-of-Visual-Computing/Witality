using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FarbordnungBehaviour : MonoBehaviour
{
    Vector3[] startPositions;
    Transform proben;
    public bool randomize = true;
    string[] nr_gelb = { "389", "855", "622", "334", "088", "211" }, nr_rose = { "923", "567", "690", "457", "868", "224" }, nr_rot = {"101", "745", "978", "156", "512", "279"};
    string current_order;
    public string start_order;
    int color; // 0 = gelb, 1 = rose, 2 = rot
    // Start is called before the first frame update
    void Start()
    {
        createStartPositions();
        read_order();
        start_order = current_order;
    }

    private void Update()
    {
        read_order();
        for (int i = 0; i < 6; i++)
        {
            if (Vector3.Distance(startPositions[i], proben.GetChild(i).position) > 1)
                resetPos(i);

        }
    }

    
   
    void createStartPositions()
    {
        proben = transform.Find("Proben");
        startPositions = new Vector3[6];
        load_mats_farbordnung();
        startPositions = savePositions();

        if (randomize)
        {
            
            for (int i = 0; i < 6; i++)
            {
                int r = Random.Range(0, 6);
                Vector3 temp = startPositions[i % 6];
                startPositions[i % 6] = startPositions[r];
                startPositions[r] = temp;
            }
            resetPos();
        }
        
    }
    void load_mats_farbordnung()
    {
        color = (int)TaskChanger.instance.task_f;
        for (int x = 0; x < 6; x++)
        {
            string c = color == 0 ? "Gelb" : color ==  1 ? "Rose" : "Rot";
            proben.GetChild(x).Find("color_probe_glass_fill").GetComponent<Renderer>().material = Resources.Load("Farb Rangordnung Mats/" + c + "/" + x, typeof(Material)) as Material;
            proben.GetChild(x).Find("Canvas/text").GetComponent<TextMeshProUGUI>().text = color == 0 ? nr_gelb[x] : color == 1 ? nr_rose[x] : nr_rot[x];
        }
    }
    Vector3[] savePositions()
    {
        Vector3[] res = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            res[i] = proben.GetChild(i).position;
        }
        return res;
    }
    public void resetPos()
    {
        for (int i = 0; i < 6; i++)
        {
            resetPos(i);
        }
    }
    public void resetPos(int i)
    {
        proben.GetChild(i).position = startPositions[i];
        proben.GetChild(i).rotation = new Quaternion();
        proben.GetChild(i).GetComponent<Rigidbody>().velocity = Vector3.zero;
        proben.GetChild(i).GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void read_order()
    {
        current_order = "";

        List<int> id = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            id.Add(i);
        }
        
        for (int i = 0; i < 6; i++)
        {
            int smallest_x = id[0];

            for (int x = 0; x < id.Count; x++)
            {
                if(proben.GetChild(id[x]).position.x < proben.GetChild(smallest_x).position.x)
                {
                    smallest_x = id[x];
                }
            }
            
            current_order += proben.GetChild(smallest_x).Find("Canvas/text").GetComponent<TextMeshProUGUI>().text + "-";
            id.Remove(smallest_x);
        }
        current_order = current_order.Substring(0, current_order.Length - 1);

        transform.Find("Canvas/order").GetComponent<TextMeshProUGUI>().text = current_order;
    }

    public void save()
    {
        //TODO start_order & current_order
    }
}
