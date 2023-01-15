using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Globalization;

public class SubmittingBehaviour : UI_AbstractMenuBehaviour {

    [SerializeField] Text InfoText;
    [SerializeField] Text CloseButtonText;

    public MainMenuBehaviour MainBehaviour;

	// Use this for initialization
	public void Save (MainMenuBehaviour mb) {
        MainBehaviour = mb;
        CloseButtonText.text = MainMenuBehaviour.CurrentDictionary.GetKeyValue("cancel");
        //activate ths, iif you want to send  data
        InfoText.text = MainMenuBehaviour.CurrentDictionary.GetKeyValue("saved_locally", false);
        Questionaire questionaire = MainBehaviour.GetCurrentQuestionaire();
        SaveSurveyToJson saveSurvey = new SaveSurveyToJson(questionaire);
        string survey = JsonUtility.ToJson(saveSurvey);

        #region Witality addons
        //custom additions for Witality:
        survey = survey.Substring(0, survey.Length - 2) + ","; //remove closing brackets add last ,
        survey += "{\"Questions\":[\"Proband ID\"],\"Answers\":[\"" + MenuSceneLoader.probandID + "\"]},";
        survey += "{\"Questions\":[\"Studie\"],\"Answers\":[\"" +  MenuSceneLoader.task + "\"]},";
        survey += "{\"Questions\":[\"Studie Variante\"],\"Answers\":[\"" + System.Enum.GetName(TaskChanger.Task2Subtask(MenuSceneLoader.task) , MenuSceneLoader.subtask) + "\"]},";
        survey += "{\"Questions\":[\"Umgebung\"],\"Answers\":[\"" + ToggleEnvironments.id2name(MenuSceneLoader.environment)+ "\"]},";

        if(Rangordnung.instance != null)
        {
            survey += "{\"Questions\":[\"Rangordnung - Korrekt\"],\"Answers\":[\"";
            survey += Rangordnung.instance.order_string(Rangordnung.instance.order_correct);
            survey += "\"]},";

            survey += "{\"Questions\":[\"Rangordnung - Abgabe\"],\"Answers\":[\"";
            survey += Rangordnung.instance.order_string(Rangordnung.instance.order);
            survey += "\"]},";
        }


        survey = survey.Substring(0, survey.Length - 1) + "]}"; //remove last ,
        #endregion

        //how many saves so far?
        int savedCount = 0;
        if (PlayerPrefs.HasKey("SavedCount"))
        {
            savedCount = PlayerPrefs.GetInt("SavedCount");
        }
        SaveLocal(survey, savedCount);
        //TODO
        //Activate this, if you want to send the data to a server
      /*  StartCoroutine( PushToServer(survey,savedCount));*/
        savedCount++;
        PlayerPrefs.SetInt("SavedCount", savedCount);
    }
    /// <summary>
    /// Save results as a json file  in persistant data path
    /// </summary>
    /// <param name="survey">Survey.</param>
    /// <param name="id">Identifier.</param>
    /// 

    void SaveLocal(string survey, int id)
    {
        //save to disk 
        string sub = survey.Substring(63, 4);
        string fileName = "";
        /*
        if (sub.Length != 4 || sub == "")
        {
            fileName = "results" + id.ToString("D4");
        }
        else
        {
            fileName = sub;
        }
        */
        fileName = System.DateTime.Today.ToString("d", CultureInfo.GetCultureInfo("de-DE")) + "_" + MenuSceneLoader.probandID.ToString("000.") + "_"; //fr -> 31/10/2022      de -> 31.10.2022
        if (MenuSceneLoader.demographic)
            fileName += "demographic";
        else if (MainMenuBehaviour.currentlyDoingIPQ)
            fileName += "ipq";
        else
            fileName += MenuSceneLoader.task + "-" + System.Enum.GetName(TaskChanger.Task2Subtask(MenuSceneLoader.task), MenuSceneLoader.subtask) + "_" + ToggleEnvironments.id2name(MenuSceneLoader.environment);

        string path = Application.dataPath + "/" + "Logs";

        if (!Directory.Exists(path))
        {
            var folder = Directory.CreateDirectory(path);
        }
        while (File.Exists(path + "/" + fileName + ".json"))
        {
            if (fileName[fileName.Length - 1] == ')')
            {
                fileName = fileName.Substring(0, fileName.Length - 2) + (int.Parse("" + fileName[fileName.Length - 2])+1).ToString() + ")";
            }
            else
                fileName += "(1)";
        }

        string filePath = path + "/" + fileName + ".json";//Application.persistentDataPath + "/"+fileName + ".json";
        byte[] JsonStringBytes = Encoding.UTF8.GetBytes(survey);
        File.WriteAllBytes(filePath, JsonStringBytes);
        
        

        InfoText.text += "\n\n" + MainMenuBehaviour.CurrentDictionary.GetKeyValue("File saved.", false);
        //MainBehaviour.ShowStartScreen();
    }

    /// <summary>
    /// enumerator, that pushes the data  to a server
    /// you need to configure that on your own, activate the coroutine in the start function
    /// </summary>
    /// <returns>The to server.</returns>
    /// <param name="survey">Survey.</param>
    /// <param name="id">Identifier.</param>
   IEnumerator PushToServer(string survey, int id)
    {
        survey = System.Uri.EscapeUriString(survey);
        WWWForm form = new WWWForm();
        form.AddField("key", "your authetication key");
        form.AddField("id", id);
        form.AddField("data", survey);

        string url = "http://your.webserver.com/rest";
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();
        print("response: " + www.downloadHandler.text);
        if (www.downloadHandler.text.Contains("success"))
        {
            InfoText.text += "\n\n" + MainMenuBehaviour.CurrentDictionary.GetKeyValue("sent_to_server", false);
            CloseButtonText.text = MainMenuBehaviour.CurrentDictionary.GetKeyValue("OK");
        }
        else
        {
            InfoText.text += "\n\n Sending Error"; 
        }

        MainBehaviour.ShowStartScreen();
    } 
   
}
