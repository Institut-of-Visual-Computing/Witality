using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
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

        if (MenuSceneLoader.demographic)
        {
            mainQuestionnaire.setQuestionaire(demographic, -1);
            Debug.Log("Started Questionnaire Demographic");
        }
        else
        {

            string json = jsons[id].ToString();
            string original = jsons[id].ToString();
            int id_offset = 0, code_idx = 0;
            int idx = json.IndexOf("***");
            int[] codes = randomOrder(MenuSceneLoader.codes);

            while (idx != -1 && code_idx < codes.Length)
            {
                if (codes[code_idx] == 0)
                    code_idx++;
                original = original.Substring(0, id_offset + idx) + codes[code_idx].ToString("000.") + original.Substring(id_offset + idx + 3);

                json = json.Substring(idx + 3);
                id_offset += idx + 3;
                idx = json.IndexOf("***");
                code_idx++;
            }
            TextAsset newJson = new TextAsset(original);
            mainQuestionnaire.setQuestionaire(newJson, autoTurnOff[id]);
            Debug.Log("Started Questionnaire " + jsons[id].name);
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
