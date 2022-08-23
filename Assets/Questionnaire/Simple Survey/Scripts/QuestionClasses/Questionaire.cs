using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Questionaire {
    public string Language = "Deutsch";
   [SerializeField]  List<Question> Questions = new List<Question> ();

    public int GetQuestionCount()
    {
        return (Questions.Count);
    }

    public Question GetQuestion(int index)
    {
        if (index >= 0 && index < Questions.Count)
        {
            return Questions[index];
        }
        return null;
    }

}
