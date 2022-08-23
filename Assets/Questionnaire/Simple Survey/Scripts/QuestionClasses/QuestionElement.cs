using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class QuestionElement {
    /// <summary>
    /// The Index of this Element in the question
    /// </summary>
    public int IndexNumber;


    /// <summary>
    /// additional text input element for example for "others"
    /// </summary>
    public bool HasTextInput = false;
    /// <summary>
    /// The Text Message accompining the input field
    /// </summary>
    public string TextInputMessage;
    /// <summary>
    /// Text Message typed in by the user
    /// </summary>
    public string TextInputValue;

    /// <summary>
    /// Main Question/task of this element
    /// </summary>
    public string TextMessage;
    public bool IsSelected;

    public int SetValue(bool value)
    {
        IsSelected = true;
        return IndexNumber;
    }

    public bool GetValue()
    {
        return IsSelected;
    }

    public int SetValue(string Text)
    {
        TextInputValue = Text;
        return IndexNumber;
    }

    public string GetTextValue()
    {
       return  TextInputValue;
    }


}
