using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core;
using UnityEngine;
public class QuestionnaireBehaviour : MonoBehaviour
{
    public int probandID;
    public bool show_questionnaire;
    public Toggle_Gameobject RayL, RayR;
    [Header(" 0 = Farbe \n 1 = Geschmack \n 2 = Aroma - Rangordnung \n 3 = Aroma - Erkennung \n 4 = CATA - weiß \n 5 = CATA - rot")]
    public TextAsset[] jsons_DEU, jsons_ENG;
    public TextAsset demographic_DEU, ipq_DEU, demographic_ENG, ipq_ENG;
    public int[] autoTurnOff;
    public int[] repeatAmount;
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

        if (MenuSceneLoader.demographic)
        {
            mainQuestionnaire.setQuestionaire(MenuSceneLoader.english ? demographic_ENG : demographic_DEU, -1);
            Debug.Log("Started Questionnaire Demographic");
        }
        else
        {

            string json = (MenuSceneLoader.english ? jsons_ENG : jsons_DEU)[id].ToString();
            int code_idx = 0;
            int[] codes = randomOrder(MenuSceneLoader.codes);
            //CATA Questionnaire muss wiederholt werden
            if (repeatAmount[id] > 1)
            {
                int inner_start = 21, inner_end = 7;    //define manually
                string inner = ",\n" + json.Substring(inner_start, json.Length - inner_start - inner_end);
                for (int i = 0; i < repeatAmount[id] - 1; i++)
                {
                    json = json.Insert(json.Length - inner_end, inner);
                }
            }
            int idx = json.IndexOf("***");

            while (idx != -1 && code_idx < codes.Length)
            {
                if (codes[code_idx] != 0)
                {
                    json = json.Substring(0, idx) + codes[code_idx].ToString("000.") + json.Substring(idx + 3);
                    idx = json.IndexOf("***");
                }
                code_idx++;
            }
            TextAsset newJson = new TextAsset(json);
            mainQuestionnaire.setQuestionaire(newJson, autoTurnOff[id]);
            Debug.Log("Started Questionnaire " + (MenuSceneLoader.english ? jsons_ENG : jsons_DEU)[id].name);
        }
            
        Apply();

    }

    int[] randomOrder(int[] a)
    {
        int[] b = new int[a.Length];

        for (int i = 0; i < a.Length; i++)
        {
            b[i] = a[i];
        }
        for (int i = 0; i < a.Length; i++)
        {
            int tmp = b[i];
            int r = Random.Range(0, a.Length - 1);
            b[i] = b[r];
            b[r] = tmp;
        }
        return b;
    }

}
