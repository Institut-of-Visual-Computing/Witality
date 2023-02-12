using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopBehaviour : MonoBehaviour
{
    public KeyCode toggleQuestionnaire;
    public QuestionnaireBehaviour questionnaire;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleQuestionnaire))
        {
            questionnaire.Toggle();
        }
    }
}
