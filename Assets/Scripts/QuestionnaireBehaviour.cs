using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionnaireBehaviour : MonoBehaviour
{
    public int probandID;
    public bool show_questionnaire;
    public Toggle_Gameobject RayL, RayR;
    [Header("0 = Farbe | 1 = Geschmack | 2 = Aroma_r | 3 = Aroma_e | 4 = CATA")]
    public TextAsset[] jsons;
    public MainMenuBehaviour mainQuestionnaire;
    private void Start()
    {
        probandID = MenuSceneLoader.probandID;
        mainQuestionnaire.setQuestionaire(jsons[(int)MenuSceneLoader.task]);
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
