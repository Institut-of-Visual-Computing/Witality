using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Question {
    public bool AnswerRequired = false;


    /// <summary>
    /// Main Task of the Question
    /// </summary>
    public string QuestionTask;
    public QuestionTypes QuestionType;

    /// <summary>
    /// Set type of the question
    /// </summary>
    /// <param name="type">Type.</param>
    public void SetQuestionType(QuestionTypes type)
    {
        QuestionType = type;
    }

    /// <summary>
    /// All Question Elements e.g. Radio Buttons etc.
    /// </summary>
   [SerializeField] List<QuestionElement> QuestionElements = new List<QuestionElement>();

    public QuestionElement GetQuestionElement(int index)
    {
        if (index >=0 && index < QuestionElements.Count)
        {
            return QuestionElements[index];
        }
        return null;
    }

    public int GetQuestionCount()
    {
        return QuestionElements.Count;
    }
    public bool IsQuestionSolved()
    {
        if (!AnswerRequired)
        {
            return true;
        }

        if(QuestionType == QuestionTypes.TEXT_INPUT)
        {
            foreach (QuestionElement element in QuestionElements)
            {
                if(string.IsNullOrEmpty(element.TextInputValue))
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            bool isSolved = false;
            foreach (QuestionElement element in QuestionElements)
            {
                if (element.IsSelected)
                {
                    isSolved = true;
                    if (element.HasTextInput)
                    {
                        if(element.TextInputValue == "")
                        {
                            return false;
                        }
                    }
                }
            }
            return isSolved;
        }
    }
}
public enum QuestionTypes
{
    TEXT_INPUT,
    RADIO_BUTTON,
    CHECKBOX
}

