using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EnglishToggle : MonoBehaviour
{

    public TextMeshProUGUI studyComplete;
    public string deutsch_studyComplete, englisch_studyComplete;
    public TextAsset deutsch_question, english_question;
    public MainMenuBehaviour mainQuestion;

    private void Awake()
    {
        mainQuestion.LanguageDictionary = MenuSceneLoader.english ? english_question : deutsch_question;
        mainQuestion.SetLanguage((MenuSceneLoader.english ? english_question : deutsch_question).text);
        studyComplete.text = (MenuSceneLoader.english ? englisch_studyComplete : deutsch_studyComplete);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
