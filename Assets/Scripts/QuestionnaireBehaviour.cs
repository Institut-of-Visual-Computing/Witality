using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionnaireBehaviour : MonoBehaviour
{
    public int probandID;

    private void Start()
    {
        probandID = MenuSceneLoader.probandID;
    }
}
