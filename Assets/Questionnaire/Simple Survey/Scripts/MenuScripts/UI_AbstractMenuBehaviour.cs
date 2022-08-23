using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Unity.Animation;

public class UI_AbstractMenuBehaviour : MonoBehaviour
{
	public AbstractUIAnimationBehaviour.MoveDirection outMoveDirection = AbstractUIAnimationBehaviour.MoveDirection.DOWN;
	//	public GameObject ObjectToDestroy;
	//
	//	public GameObject ObjectToFadeOut;

	public AbstractUIAnimationBehaviour FadeOutAnimationBehaviour;

	public void CloseMenu (bool animate = true)
	{
		print("Closing menu: " + gameObject.name);
		DoWhileClosing ();
		if (animate) {
			if (FadeOutAnimationBehaviour == null) {
				FadeOutAnimationBehaviour = GetComponentInChildren<AbstractUIAnimationBehaviour> ();
			}
			if (FadeOutAnimationBehaviour == null) {
				FadeOutAnimationBehaviour = gameObject.AddComponent<UIAnimationZoomBehaviour> ();
			}
			FadeOutAnimationBehaviour.MoveDirectionMovement = outMoveDirection;
			FadeOutAnimationBehaviour.FadeOutAnimation (false);
//		FadeOutAnimationBehaviour.MoveDistanceFactor = 1;
		} else {
			print ("Lösche: " + name);
			Destroy (gameObject);
		}
	}



	/// <summary>
	/// Wird ausgeführt, wenn CloseMenu aufgerufen wird.
	/// </summary>
	protected virtual void DoWhileClosing ()
	{
		
	}

	//	public void Delete ()
	//	{
	//
	//		Destroy (gameObject);
	//	}
}
 