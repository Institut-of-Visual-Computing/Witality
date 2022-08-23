using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionBehaviour : MonoBehaviour {
    Question CurrentQuestion;

    //Prefabs
    [SerializeField] GameObject RadioButtonElementPrefab;
    [SerializeField] GameObject CheckboxElementPrefab;
    [SerializeField] GameObject TextInputElementPrefab;

    [SerializeField] RectTransform QuestionElementsContainer;
    [SerializeField] Text QuestionTask;


    [SerializeField] OrientationHelperBehaviour OrientationHelper;

    List<AbstractQuestionElementBehaviour> QuestionElementBehaviours = new List<AbstractQuestionElementBehaviour>();
	// Use this for initialization
	void Start () {
		
	}
	
	public void LoadQuestion(Question question)
    {
        CurrentQuestion = question;
        //delete possible old elements
        DeleteQuestion();
        //set question Text
        QuestionTask.text = CurrentQuestion.QuestionTask;
        //set question Elements
        for (int i = 0; i < CurrentQuestion.GetQuestionCount(); i++)
        {
            GameObject QuestionElement;
            if(CurrentQuestion.QuestionType == QuestionTypes.CHECKBOX)
            {
                QuestionElement = Instantiate(CheckboxElementPrefab);
            }else if(CurrentQuestion.QuestionType == QuestionTypes.RADIO_BUTTON)
            {
                QuestionElement = Instantiate(RadioButtonElementPrefab);
            }
            else
            {
                QuestionElement = Instantiate(TextInputElementPrefab);
            }

            AbstractQuestionElementBehaviour elementBehaviour = QuestionElement.GetComponent<AbstractQuestionElementBehaviour>();
            elementBehaviour.element = CurrentQuestion.GetQuestionElement(i);
            elementBehaviour.CurrentQuestionBehaviour = this;
            QuestionElementBehaviours.Add(elementBehaviour);
            //QuestionElement.transform.SetParent(QuestionElementsContainer, false);
        }

        OrientationHelper.question = CurrentQuestion;
        OrientationHelper.SetBehaviours(QuestionElementBehaviours);
    }

    void DeleteQuestion()
    {
        //delete all previous question elements
        foreach (AbstractQuestionElementBehaviour child in QuestionElementBehaviours)
        {
            Destroy(child.gameObject);
        }
        QuestionElementBehaviours.Clear();
        OrientationHelper.QuestionElementBehaviours.Clear();
        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// Resets all Elements for example in a SingleChoice Question
    /// </summary>
    public void ResetElements()
    {
        //check if single choice question
        foreach (AbstractQuestionElementBehaviour element in QuestionElementBehaviours)
        {
            element.ResetElement();
        }
    }
}
