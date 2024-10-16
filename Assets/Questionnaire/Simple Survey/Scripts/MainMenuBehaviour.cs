﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour {

    public QuestionnaireBehaviour questionnaireBehaviour;
    public static bool currentlyDoingIPQ;
    /// <summary>
    /// The two questionaires as Text asset in json
    /// </summary>
    [SerializeField] TextAsset MainQuestionaire;

    [SerializeField] public TextAsset LanguageDictionary;

    //Containers to put the questins in
    /// <summary>
    /// Put in the QuestionTask here
    /// </summary>
        [SerializeField] Text QuestionTaskTextElement;
        /// <summary>
        /// put all elements in here
        /// </summary>
    [SerializeField] RectTransform QuestionElementsContainer;

    //Question Buttons
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject PreviousButton;
    [SerializeField] GameObject SubmitButton;
    [SerializeField] GameObject CancelButton;
    [SerializeField] GameObject MainCancelButton;

    // Panels
    [SerializeField] GameObject MainPanel;
    [SerializeField] GameObject QuestionPanel;

    [SerializeField] QuestionBehaviour questionBehaviour;

    [SerializeField] GameObject SubmitPanelPrefab;
    [SerializeField] GameObject QuestionNotAnsweredPanelPrefab;

    public GameObject QuestPointer;
    public Camera CanvasCamera;
    public Transform QuestionnaireCanvas;
    Questionaire CurrentQuestionaire = new Questionaire();
    public GameObject StudyCompleteScreen;
    public static LanguageDictionary CurrentDictionary;
    public static bool taskSubmitted = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            QuestionIndex++;
            LoadQuestion(QuestionIndex);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            QuestionIndex--;
            LoadQuestion(QuestionIndex);
        }
    }
    public Questionaire GetCurrentQuestionaire()
    {
        return CurrentQuestionaire;
    }
    public int QuestionIndex;
    public int automatedTurnOff = -1;

    // Use this for initialization
    void Start () {
        currentlyDoingIPQ = false;
        SetLanguage(LanguageDictionary.text);
        taskSubmitted = false;
        //ShowStartScreen();

        //SubmitPanelPrefab.GetComponent<OVRRaycaster>().pointer = QuestionNotAnsweredPanelPrefab.GetComponent<OVRRaycaster>().pointer = QuestPointer;
        //SubmitPanelPrefab.GetComponent<Canvas>().worldCamera = QuestionNotAnsweredPanelPrefab.GetComponent<Canvas>().worldCamera = CanvasCamera;
        if(MenuSceneLoader.pieroth)
            StartSurvey();
    }

    public void ShowStartScreen()
    {
        //MainPanel.SetActive(true);
        //QuestionPanel.SetActive(false);
        //MainCancelButton.SetActive(false);
    }

    void StartQuestionaire()
    {
        //get Questionaire through serializer
        CurrentQuestionaire = JsonUtility.FromJson<Questionaire>(MainQuestionaire.text);
        ////Set question Index to zero
        QuestionIndex = 0;

        //Load first Question
        LoadQuestion(0);

        //Switch visibility of Panels

        MainPanel.SetActive(false);
        QuestionPanel.SetActive(true);
        MainCancelButton.SetActive(true);
    }

    void LoadQuestion(int index)
    {
   
        Question question= CurrentQuestionaire.GetQuestion(index);
        if (question!= null) {
            questionBehaviour.LoadQuestion(question);
        }
        else
        {
            print("Question not found! " + index);
            ShowStartScreen();
        }
        ButtonVisibility();
        if (index == automatedTurnOff && !taskSubmitted)
        {
            if(MenuSceneLoader.pieroth)
                transform.parent.parent.parent.parent.gameObject.SetActive(false);
            else
                questionnaireBehaviour.Set(false);
        
        }
        QuestionaireImages.updated = false;
    }

    public void NextQuestion()
    {
        if (CheckQuestionSolved(CurrentQuestionaire.GetQuestion(QuestionIndex)))
        {
            QuestionIndex++;
           LoadQuestion(QuestionIndex);
        }
        else
        {
            //Instantiate(QuestionNotAnsweredPanelPrefab, QuestionnaireCanvas);
        }
    }

    bool CheckQuestionSolved(Question question)
    {
        return question.IsQuestionSolved();
    }

    public void PreviousQuestion()
    {
        QuestionIndex--;
        LoadQuestion(QuestionIndex);
    }

    void ButtonVisibility()
    {
        //Check if this is the last question, then dispay submit button!
        bool chancelQuestionaire = QuestionIndex == 0;
        CancelButton.SetActive(false);//chancelQuestionaire);
        PreviousButton.SetActive(!chancelQuestionaire);

        //Check if this is the last question, then dispay submit button!
        bool submit = QuestionIndex == CurrentQuestionaire.GetQuestionCount() - 1;
        SubmitButton.SetActive(submit);
        NextButton.SetActive(!submit);
    }

    public void ChancelQuestionaire()
    {
        ShowStartScreen();
    }

    public void SubmitQuestionaire()
    {
        if (CheckQuestionSolved(CurrentQuestionaire.GetQuestion(QuestionIndex)))
        {
            GameObject SubmitPanel = Instantiate(SubmitPanelPrefab, QuestionnaireCanvas);
            SubmittingBehaviour submittingBehaviour = SubmitPanel.GetComponentInChildren<SubmittingBehaviour>();
            submittingBehaviour.Save(this);
            if (MenuSceneLoader.demographic)
            {
                MenuSceneLoader.demographic = false;
                questionnaireBehaviour.StartQuestionnaire();
                //ShowStartScreen();
            }
            else if (MenuSceneLoader.ipq && !currentlyDoingIPQ)
            {
                setQuestionaire((MenuSceneLoader.english ? questionnaireBehaviour.ipq_ENG : questionnaireBehaviour.ipq_DEU), -1);
                Debug.Log("Started Questionnaire IPQ");
                currentlyDoingIPQ = true;
                //ShowStartScreen();
            }
            else
            {
                if (!MenuSceneLoader.pieroth)
                {
                    questionnaireBehaviour.Set(false);
                    StudyCompleteScreen.SetActive(true);
                    MainPanel.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                //ShowStartScreen();
            }
        }
        else
        {
            Instantiate(QuestionNotAnsweredPanelPrefab, QuestionnaireCanvas);
        }
    }

    public  void StartSurvey()
    {
        StartQuestionaire();
    }

    public void SetLanguage(string languageDictionary)
    {
        CurrentDictionary = JsonUtility.FromJson<LanguageDictionary>(languageDictionary);
        SubmitButton.GetComponentInChildren<Text>().text = CurrentDictionary.GetKeyValue("submit");
        NextButton.GetComponentInChildren<Text>().text = CurrentDictionary.GetKeyValue("next");
        PreviousButton.GetComponentInChildren<Text>().text = CurrentDictionary.GetKeyValue("previous");
        CancelButton.GetComponentInChildren<Text>().text = CurrentDictionary.GetKeyValue("cancel");
    }
    public void setQuestionaire(TextAsset q, int autoTurnOff)
    {
        automatedTurnOff = autoTurnOff;
        MainQuestionaire = q;
        StartQuestionaire();
    }
    public void SetQuestionaire(TextAsset q)
    {
        MainQuestionaire = q;
        StartQuestionaire();
    }
    public void SetSubmitted(bool submitted)
    {
        taskSubmitted = submitted;
    }
    public void SetTurnOff(int i)
    {
        automatedTurnOff = i;
    }
    
}
