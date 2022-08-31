using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionnaireBehaviour : MonoBehaviour
{
    public int probandID;
    public bool show_questionnaire;
    public Toggle_Gameobject RayL, RayR;
    
    private void Start()
    {
        probandID = MenuSceneLoader.probandID;
        apply();
    }

    public void toggle()
    {
        show_questionnaire = !show_questionnaire;
        apply();
    }

    void apply()
    {
        RayL.toggle(show_questionnaire);
        RayR.toggle(show_questionnaire);
        gameObject.SetActive(show_questionnaire);
    }
}
