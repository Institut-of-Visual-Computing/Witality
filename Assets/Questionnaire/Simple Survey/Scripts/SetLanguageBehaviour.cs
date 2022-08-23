using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLanguageBehaviour : MonoBehaviour {
    [SerializeField] bool Uppercase = true;
    [SerializeField] string Key;
    [SerializeField] Text TextFieldToChange;

	// Use this for initialization
	void Start () {
        TextFieldToChange.text= MainMenuBehaviour.CurrentDictionary.GetKeyValue(Key, Uppercase);
	}
	
}
