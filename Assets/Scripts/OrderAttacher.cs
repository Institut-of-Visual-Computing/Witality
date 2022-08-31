using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrderAttacher : MonoBehaviour
{
    public static Rangordnung rang;
    public int chosenInt = -1;
    public float minApllyRadius = 0.01f;
    public Transform handL, handR;
    public bool showing;
    public TextMeshProUGUI text;
    public void activated()
    {
        if (showing)
        {
            deactivate();
            return;
        }
        showing = true;
        handL = Calibration.getFingerTip(handL);
        handR = Calibration.getFingerTip(handR);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i <= rang.obj_parent.childCount);
        }
    }
    
    public void setChosenInt(int i)
    {
        chosenInt = i;
        text.text = "Wähle das " + (i+1) + ". Element!";
    }
    private void Update()
    {
        if(chosenInt != -1)
        {
            
            int index = 0;
            float distance = Mathf.Min(
                Vector3.Distance(rang.obj_parent.GetChild(index).position,handL.position), 
                Vector3.Distance(rang.obj_parent.GetChild(index).position, handR.position));

            for (int i = 1; i < rang.obj_parent.childCount; i++)
            {
                float d = Mathf.Min(
                Vector3.Distance(rang.obj_parent.GetChild(i).position, handL.position),
                Vector3.Distance(rang.obj_parent.GetChild(i).position, handR.position));

                if (d < distance)
                {
                    distance = d;
                    index = i;
                }
            }

            if(distance < minApllyRadius)
            {
                rang.set_order(index, chosenInt);
                chosenInt = -1;
                text.text = "";
            }
        }
    }

    void deactivate()
    {
        chosenInt = -1;
        gameObject.SetActive(false);
        showing = false;
        text.text = "";
    }

}
