using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioButtonQuestionElementBehaviour : AbstractQuestionElementBehaviour {

    [SerializeField] GameObject SelectedSign;

    protected override void SetUpElement()
    {
        base.SetUpElement();
        SelectedSign.SetActive(element.IsSelected);
    }


    public override void SelectElement()
    {
        //reset all other elements first, since this is a SingleChoice Question
        if (!element.IsSelected)
        {
            CurrentQuestionBehaviour.ResetElements();
        }
        //set Value (reset, if selected)
        element.IsSelected = true;
        SelectedSign.SetActive(element.IsSelected);

        //Open Text Input if necessary
        if (element.HasTextInput)
        {
            GameObject textInput = Instantiate(AdditionalTextInputPrefab);
            AdditionalTextInputBehaviour inputBehaviour = textInput.GetComponentInChildren<AdditionalTextInputBehaviour>();
            inputBehaviour.Element = element;
            inputBehaviour.questionElementBehaviour = this;
        }
    }

    public override void ResetElement()
    {
        base.ResetElement();
        SelectedSign.SetActive(element.IsSelected);
    }
}
