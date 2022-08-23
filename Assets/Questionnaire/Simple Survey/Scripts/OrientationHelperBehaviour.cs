using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Changes elements arrangement wen Device rotates
/// </summary>
public class OrientationHelperBehaviour : MonoBehaviour {
    [SerializeField] RectTransform VerticalPanel;
    [SerializeField] RectTransform HorizontalPanel;
    [SerializeField] RectTransform HorizontalSubPanel1;
    [SerializeField] RectTransform HorizontalSubPanel2;

    [SerializeField] LayoutElement HorizontalPanelLayout;
    ScreenOrientation CurrentScreenOrientation = ScreenOrientation.LandscapeLeft;

    public   List<AbstractQuestionElementBehaviour> QuestionElementBehaviours = new List<AbstractQuestionElementBehaviour>();

    public Question question;

    public void SetBehaviours(List<AbstractQuestionElementBehaviour> questionElementBehaviours)
    {
        QuestionElementBehaviours = questionElementBehaviours;
        ChangeOrientation();
    }

    // Use this for initialization
    void Update () {
     
        if(CurrentScreenOrientation != Screen.orientation)
        {
            ChangeOrientation();
        }
    }
	
	public void ChangeOrientation()
    {
        if( Screen.orientation == ScreenOrientation.LandscapeLeft && QuestionElementBehaviours.Count>1 && question.QuestionType!= QuestionTypes.TEXT_INPUT)
        {
            for (int i = 0; i < QuestionElementBehaviours.Count; i++) { 
            if(i%2 == 0)
                {
                    QuestionElementBehaviours[i].transform.SetParent(HorizontalSubPanel1, false);
                }
                else
                {
                    QuestionElementBehaviours[i].transform.SetParent(HorizontalSubPanel2, false);
                }
            }
            //adjust content size fitter
            float elementHeight = QuestionElementBehaviours[0].GetComponentInChildren<LayoutElement>().preferredHeight;
            HorizontalPanelLayout.preferredHeight = elementHeight * Mathf.CeilToInt((float)QuestionElementBehaviours.Count / 2f);
            HorizontalPanel.gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < QuestionElementBehaviours.Count; i++)
            {
                   QuestionElementBehaviours[i].transform.SetParent(VerticalPanel,false);
            }
            HorizontalPanel.gameObject.SetActive(false);
        }
        CurrentScreenOrientation = Screen.orientation;
    }
}
