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
    public TextAsset demographic, ipq;
    public int[] autoTurnOff;
    public MainMenuBehaviour mainQuestionnaire;

    private void Start()
    {
        StartQuestionnaire();
    }
    public void Toggle()
    {
        show_questionnaire = !show_questionnaire;
        Apply();
    }
    public void Set(bool active)
    {
        show_questionnaire = active;
        Apply();
    }

    void Apply()
    {
        RayL.toggle(show_questionnaire);
        RayR.toggle(show_questionnaire);
        gameObject.SetActive(show_questionnaire);
    }
    public void StartQuestionnaire()
    {
        probandID = MenuSceneLoader.probandID;
        int id = (int)MenuSceneLoader.task;
        if (id == 4 && MenuSceneLoader.subtask >= 1)
            id = 5;
        //Hier sollte die json template eingelesen werden und die codes dann durch MenuSceneLoader.codes ersetzen
        Debug.Log("Started Questionnaire " + jsons[id].name);
        if (MenuSceneLoader.demographic)
            mainQuestionnaire.setQuestionaire(demographic, -1);
        else
        {
            int offset = 127; //need to be verified
            string json = jsons[id].ToString();
            int code_idx = 0;
            int idx = json.IndexOf("Probe");
            while(idx != -1)
            {
                for (int i = 0; i < 3; i++)
                {
                    json = ReplaceAt(json, idx + offset + i, MenuSceneLoader.codes[code_idx].ToString()[i]);
                }
            }
            TextAsset newJson = new TextAsset(json);
            mainQuestionnaire.setQuestionaire(newJson, autoTurnOff[id]);
        }
            
        Apply();

    }

    string ReplaceAt(string s, int index, char c)
    {
        return s.Substring(0, index) + c + s.Substring(index + 1);
    }
}
