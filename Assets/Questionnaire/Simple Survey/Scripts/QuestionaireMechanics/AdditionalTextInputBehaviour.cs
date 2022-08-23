using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdditionalTextInputBehaviour : UI_AbstractMenuBehaviour {

    [SerializeField] InputField TextInput;
    [SerializeField] Text TextMessage;
    [SerializeField] GameObject TextMessagePrefab;
    public QuestionElement Element;
    public AbstractQuestionElementBehaviour questionElementBehaviour;

    private void Start()
    {
        TextMessage.text = Element.TextInputMessage;
        TextInput.text = Element.TextInputValue;
    }

    public void SecureClose()
    {
        if (TextInput.text == "")
        {
            Instantiate(TextMessagePrefab);
        }
        else {
            Element.TextInputValue = TextInput.text;
            questionElementBehaviour.UpdateAdditionalTextInput();
            CloseMenu(); 
        }
    }

    protected override void DoWhileClosing()
    {
        base.DoWhileClosing();
        Element.TextInputValue = TextInput.text;
    }
}
