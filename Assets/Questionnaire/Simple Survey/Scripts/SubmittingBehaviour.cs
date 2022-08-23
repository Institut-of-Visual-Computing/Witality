using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SubmittingBehaviour : UI_AbstractMenuBehaviour {

    [SerializeField] Text InfoText;
    [SerializeField] Text CloseButtonText;

    public MainMenuBehaviour MainBehaviour;

	// Use this for initialization
	void Start () {
        CloseButtonText.text = MainMenuBehaviour.CurrentDictionary.GetKeyValue("cancel");
        //activate ths, iif you want to send  data
        InfoText.text = MainMenuBehaviour.CurrentDictionary.GetKeyValue("saved_locally", false);
        Questionaire questionaire = MainBehaviour.GetCurrentQuestionaire();
        SaveSurveyToJson saveSurvey = new SaveSurveyToJson(questionaire);
        string survey = JsonUtility.ToJson(saveSurvey);

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
        print(sub);
        string fileName = "";
        if (sub.Length != 4 || sub == "")
        {
            fileName = "results" + id.ToString("D4");
        }
        else
        {
            fileName = sub;
        }

        string path = Application.dataPath + "/" + "Logs";

        if (!Directory.Exists(path))
        {
            var folder = Directory.CreateDirectory(path);
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
