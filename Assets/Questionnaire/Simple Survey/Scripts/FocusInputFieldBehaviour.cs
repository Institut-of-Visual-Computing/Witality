using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FocusInputFieldBehaviour : MonoBehaviour, ISelectHandler
{
    [SerializeField] GameObject FocusInputFieldCanvasPrefab;
    Transform OriginalParentTransform;
    [SerializeField] Text TextMessage;
    FocusInputFieldContainerBehaviour FocusFieldContainer;
    [SerializeField] InputField inputField;
    private TouchScreenKeyboard mobileKeys;

    public void OnSelect(BaseEventData data)
    {
        if (FocusFieldContainer == null) {
            OriginalParentTransform = transform.parent;
            GameObject focusField = Instantiate(FocusInputFieldCanvasPrefab);
            FocusFieldContainer = focusField.GetComponent<FocusInputFieldContainerBehaviour>();
            FocusFieldContainer.SetUpContainer(transform, TextMessage.text);
            FocusFieldContainer.FieldBehaviour = this;
            mobileKeys = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false);
        }
    }

   

    //public void OnInputEvent()
    //{
    //    print("InputEvent");
    //    //mobileKeys = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false);
    //}

    void Start()
    {
        inputField.onEndEdit.AddListener(val =>
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || mobileKeys.status == TouchScreenKeyboard.Status.Done)
            {
                FocusFieldContainer.Close();
            }
        });
    }

    public void Reattach()
    {
        transform.SetParent(OriginalParentTransform, false);
        if (!EventSystem.current.alreadySelecting) { 
        EventSystem.current.SetSelectedGameObject(OriginalParentTransform.gameObject);
        }
    }
}
