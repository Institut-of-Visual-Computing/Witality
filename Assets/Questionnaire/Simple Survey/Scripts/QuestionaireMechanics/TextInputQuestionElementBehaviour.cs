using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInputQuestionElementBehaviour : AbstractQuestionElementBehaviour {

    [SerializeField] InputField TextInput;

    protected override void SetUpElement()
    {
        base.SetUpElement();
        TextInput.text = element.TextInputValue;


    }

   public void IfValueChanged () {
        element.TextInputValue = TextInput.text;
    }
}
