using UnityEngine;

[RequireComponent(typeof(Rangordnung))]
public class FarbordnungBehaviour : MonoBehaviour
{
    public Vector3[] startPositions;
    public Transform proben;
    [HideInInspector]
    public int[] nr_weiﬂ = { 389, 855, 622, 334, 088, 211 }, nr_rose = { 923, 567, 690, 457, 868, 224 }, nr_rot = {101, 745, 978, 156, 512, 279};
    Rangordnung rang;
    public Transform orderAttacherCanvas;

    void Start()
    {
        rang = GetComponent<Rangordnung>();
        if (rang.order_correct == null)
            rang.order_correct = TaskChanger.instance.subtask == 0 ? nr_weiﬂ : TaskChanger.instance.subtask == 1 ? nr_rose : nr_rot;
        createStartPositions();

    }

    private void Update()
    {
        
        for (int i = 0; i < proben.childCount; i++)
        {
            if (Vector3.Distance(startPositions[i], proben.GetChild(i).position) > 1)
                resetPos(i);

        }
    }

    
   
    void createStartPositions()
    {
        proben = rang.obj_parent;
        load_mats_farbordnung();
        startPositions = new Vector3[proben.childCount];
        startPositions = savePositions();

        for (int i = 0; i < proben.childCount; i++)
        {
            for (int x = 0; x < proben.childCount; x++)
            {
                int r = Random.Range(0, 6);
                Vector3 temp = startPositions[i % 6];
                startPositions[i % 6] = startPositions[r];
                startPositions[r] = temp;
                
            }
        }

        resetPos();
        
    }
    
    public void load_mats_farbordnung(int color = -1)
    {
        if(proben == null)
            proben = transform.Find("Proben");
        if (rang == null)
            rang = GetComponent<Rangordnung>();
        if(color == -1)
            color = TaskChanger.instance.subtask;
        string c = color == 0 ? "Weiﬂ" : color == 1 ? "Rose" : "Rot";
        for (int x = 0; x < proben.childCount; x++)
        {
            proben.GetChild(x).Find("color_probe_glass_fill").GetComponent<Renderer>().material = Resources.Load("Farb Rangordnung Mats/" + c + "/" + x, typeof(Material)) as Material;
        }
        rang.apply_text();
        rang.apply_order_text();
    }
    Vector3[] savePositions()
    {
        Vector3[] res = new Vector3[proben.childCount];
        for (int i = 0; i < proben.childCount; i++)
        {
            res[i] = proben.GetChild(i).position;
        }
        return res;
    }
    public void resetPos()
    {
        for (int i = 0; i < proben.childCount; i++)
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

    public void resetRotation()
    {
        for (int i = 0; i < proben.childCount; i++)
        {
            proben.GetChild(i).rotation = new Quaternion();
            proben.GetChild(i).GetComponent<Rigidbody>().velocity = Vector3.zero;
            proben.GetChild(i).GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}
