using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestionaireImages : MonoBehaviour
{
    public MainMenuBehaviour menu;

    public GameObject[] images;
    public int[] indices;


    public static bool updated = false;

    // Update is called once per frame
    void Update()
    {
        if (!updated)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].SetActive(menu.QuestionIndex == indices[i]);
            }
            updated = true;
        }
    }
}
