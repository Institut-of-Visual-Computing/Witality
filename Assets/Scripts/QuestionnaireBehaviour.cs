using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionnaireBehaviour : MonoBehaviour
{
    public int probandID;
    public bool show_questionnaire;
    public Toggle_Gameobject RayL, RayR;
    [Header(" 0 = Farbe \n 1 = Geschmack \n 2 = Aroma - Rangordnung \n 3 = Aroma - Erkennung \n 4 = CATA - weiß \n 5 = CATA - rot")]
    public TextAsset[] jsons;
    public MainMenuBehaviour mainQuestionnaire;
    private void Start()
    {
        probandID = MenuSceneLoader.probandID;
        int id = (int)MenuSceneLoader.task;
        if (id == 4 && MenuSceneLoader.subtask >= 1)
            id = 5;
        Debug.Log("Started Questionnaire " + jsons[id].name);
        mainQuestionnaire.setQuestionaire(jsons[id]);
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
