using UnityEngine;
using System.Collections;

namespace Utils.Unity.Animation
{

public class UIAnimationZoomBehaviour : AbstractUIAnimationBehaviour
{
	private Vector3 TargetScale;

	public Transform TransformToAnimate;

	public override void Init ()
	{
		if (TransformToAnimate == null) {
			TransformToAnimate = ObjectToAnimate.transform;
		}
		if (RandomMoveSpeed) {
			Speed = Random.Range (5f, 15f);
		}
		if (InOrOutMovement == InOrOut.OUT) {
			FadeOutAnimation (DontDestroyAfterAnim);
		} else {
			FadeInAnimation (DontDestroyAfterAnim);
		}
	}

	public override void FadeOutContent ()
	{
		TargetScale = Vector3.zero;
		TransformToAnimate.localScale = Vector3.one;
	}

	public override void FadeInContent ()
	{
		TargetScale = Vector3.one;
		TransformToAnimate.localScale = Vector3.zero;
	}

	// Update is called once per frame
	void Update ()
	{
		if (AnimationRunning) {
			TransformToAnimate.localScale = Vector3.Slerp (TransformToAnimate.localScale, TargetScale, Time.deltaTime * Speed);

			//Abbruchbedingung

			if (Vector3.Distance (TransformToAnimate.localScale, TargetScale) < 0.01) {
				AnimationRunning = false;
				if (InOrOutMovement == InOrOut.IN) {
					TransformToAnimate.localScale = Vector3.one;
				}
//				print ("Fade in Beendet: " + gameObject.name + " nicht zerstören? " + DontDestroyAfterAnim + " movement: " + InOrOutMovement);
				DestroyAnimation ();
			}
		}
	}

}
}