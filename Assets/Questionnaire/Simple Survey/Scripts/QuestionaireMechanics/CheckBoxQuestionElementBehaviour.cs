using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBoxQuestionElementBehaviour : AbstractQuestionElementBehaviour {

    [SerializeField] GameObject CheckedSign;

    protected override void SetUpElement()
    {
        base.SetUpElement();
        CheckedSign.SetActive(element.IsSelected);
    }


    public override void SelectElement()
    {
        //set Value (reset, if selected)
        if (!element.IsSelected)
        {
            element.IsSelected = true;
            //Open Text Input if necessary
            if (element.HasTextInput)
            {
                GameObject textInput = Instantiate(AdditionalTextInputPrefab);
                AdditionalTextInputBehaviour inputBehaviour = textInput.GetComponentInChildren<AdditionalTextInputBehaviour>();
                inputBehaviour.Element = element;
                inputBehaviour.questionElementBehaviour = this;
            }
        }
        else
        {
            ResetElement();
        }
        CheckedSign.SetActive(element.IsSelected);

    }
}
