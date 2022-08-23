using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbstractQuestionElementBehaviour : MonoBehaviour {
    public QuestionBehaviour CurrentQuestionBehaviour;
    public QuestionElement element;

    [SerializeField] Text AdditionalTextInput;

    [SerializeField] protected GameObject AdditionalTextInputPrefab;

    [SerializeField] Text ElementMessage;

    private void Start()
    {
        SetUpElement();
    }

    protected virtual void SetUpElement()
    {
        ElementMessage.text = element.TextMessage;
        if (element.HasTextInput)
        {
            UpdateAdditionalTextInput();
        }
    }

    public virtual void SelectElement()
    {
        //set Value (reset, if selected)

        //Open Text Input if necessary

        //reset all other elements (necessity determinde by Question behaviouur)
    }

    public void UpdateAdditionalTextInput()
    {
        if(AdditionalTextInput != null)
        {
            if(element.TextInputValue == "")
            {
                AdditionalTextInput.gameObject.SetActive(false);
            }
            else{
                AdditionalTextInput.gameObject.SetActive(true);
            }
            AdditionalTextInput.text = element.TextInputValue;
        }
    }
    public virtual void ResetElement()
    {
        element.IsSelected = false;
        if (element.HasTextInput)
        {
            element.TextInputValue = "";
        }
        UpdateAdditionalTextInput();
    }

}
