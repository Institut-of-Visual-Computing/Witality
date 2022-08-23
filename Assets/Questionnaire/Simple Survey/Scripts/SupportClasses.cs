using System;
using System.Collections.Generic;
using UnityEngine;

public class SupportClasses {

}
[Serializable]
public class LanguageDictionary{
    public string Language = "Deutsch";
    public List<LanguageKeyPair> LanguageKeys = new List<LanguageKeyPair>();

    public string GetKeyValue(string key, bool uppercase = true)
    {
        string returnValue = "Unknown";
        foreach(LanguageKeyPair keyPair in LanguageKeys)
        {
            if(keyPair.Key == key)
            {
                returnValue= keyPair.Value;
                break;
            }
        }
        if (uppercase)
        {
            returnValue = returnValue.ToUpper();
        }
        return returnValue;
    }
}

[Serializable]
public class LanguageKeyPair{
public string Key;
   public string Value;
    public LanguageKeyPair(string key, string value)
    {
        Key = key;
        Value = value;
    }
}

[Serializable]
public class SaveSurveyToJson {
    public string Language = "Deutsch";
   [SerializeField] List<Result> ResultList = new List<Result>();

    public void AddResult(Result result)
    {
        ResultList.Add(result);
    }

    public SaveSurveyToJson(Questionaire questionaire)
    {
        ParseQuestionaire(questionaire);
    }

    public void ParseQuestionaire(Questionaire questionaire)
    {
        Language = questionaire.Language;
        for (int i = 0; i < questionaire.GetQuestionCount(); i++)
        {
            if (questionaire.GetQuestion(i).QuestionType == QuestionTypes.TEXT_INPUT)
            {
                ResultList.Add( ParseTextInputQuestion(questionaire.GetQuestion(i), i));
            }
            else
            {
                ResultList.Add(ParseMultipleChoiceQuestion(questionaire.GetQuestion(i), i));
            }
        }
    }

    Result ParseTextInputQuestion(Question question,int index)
    {
        List<int> SelectedAnswerIndexes = new List<int>();
        List<string> SelectedAnswers = new List<string>();
        for (int i = 0; i< question.GetQuestionCount(); i++)
        {
            //if (question.GetQuestionElement(i).IsSelected || question.QuestionType == QuestionTypes.TEXT_INPUT)
            //{
                SelectedAnswerIndexes.Add(i);
                SelectedAnswers.Add(question.GetQuestionElement(i).TextInputValue);
            //}
        }

        Result result = new Result(index, SelectedAnswerIndexes, SelectedAnswers);
        return result;
    }

    Result ParseMultipleChoiceQuestion(Question question, int index)
    {
        List<int> SelectedAnswerIndexes = new List<int>();
        List<string> SelectedAnswers = new List<string>();
        for (int i = 0; i < question.GetQuestionCount(); i++)
        {
            if (question.GetQuestionElement(i).IsSelected)
            {
                SelectedAnswerIndexes.Add(i);
                string answerString = question.GetQuestionElement(i).TextMessage;
                if (question.GetQuestionElement(i).HasTextInput)
                {
                 answerString += ": "+   question.GetQuestionElement(i).TextInputValue;
                }
                SelectedAnswers.Add(answerString);
            }
        }

        Result result = new Result(index, SelectedAnswerIndexes, SelectedAnswers);
        return result;
    }
}
[Serializable]
public class Result
{
    int QuestionIndex;
  [SerializeField]  List<int> Indexes = new List<int>();
   [SerializeField] List<string> Answers = new List<string>();

    public Result(int questionIndex,List<int> answerIndexes, List<string> answers)
    {
        QuestionIndex = questionIndex;
        Indexes = answerIndexes;
        Answers = answers;
    }
}
