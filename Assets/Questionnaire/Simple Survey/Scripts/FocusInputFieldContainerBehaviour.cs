using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusInputFieldContainerBehaviour : MonoBehaviour {
    public FocusInputFieldBehaviour FieldBehaviour;
    [SerializeField] Text TextMessage;
    [SerializeField] RectTransform Container;

	public void SetUpContainer(Transform inputField, string textMessage)
    {
        inputField.SetParent(Container, false);
        TextMessage.text = textMessage;
    }

    public void Close()
    {
        FieldBehaviour.Reattach();
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
