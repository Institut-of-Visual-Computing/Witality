using UnityEngine;
using UnityEngine.UI;

namespace Utils.Unity.Animation
{

public class UIAnimationFadeBehaviour : AbstractUIAnimationBehaviour
{
	
	private Image PanelImage;
	private Color StandardColor;

	private Color TargetColor;

	private Color StartColor;
	// Use this for initialization
	public override  void Init ()
	{
		//Alpha Fade in einbauen zusätzlich
		PanelImage = ObjectToAnimate.GetComponent<Image> ();
		if (PanelImage != null) {
			StandardColor = PanelImage.color;
			if (InOrOutMovement == InOrOut.IN) {
				FadeInAnimation (DontDestroyAfterAnim);
			} else {
				FadeOutAnimation (DontDestroyAfterAnim);
			}
		} else {
			//Theoretisch müsste jedes UI Objekt ein Image haben. Wenn es das nicht hat, dann ist es auch nicht
			//zu sehen und dann macht dieses Script keinen Sinn und löscht sich selbst.
			Destroy (this);
		}
		if (RandomMoveSpeed) {
			Speed = Random.Range (5f, 15f);
		}
		//Speed muss hier deutlich kleiner sein??
		Speed *= 0.30f;
//		DestroyAfterAnim = false;
	}

	public override void FadeOutContent ()
	{
		base.FadeOutContent ();
		TargetColor = StandardColor;
		TargetColor.a = 0f;
		StartColor = StandardColor;
	}

	public override void FadeInContent ()
	{
		base.FadeInContent ();
		TargetColor = StandardColor;
		StartColor = StandardColor;
		StartColor.a = 0f;
	}

	
	// Update is called once per frame
	void Update ()
	{
		if (AnimationRunning) {
			if (Mathf.Abs (StartColor.a - TargetColor.a) > 0.01f) {
				StartColor = Color.Lerp (StartColor, TargetColor, Time.deltaTime * Speed);
				PanelImage.color = StartColor;
			} else {
				AnimationRunning = false;
				StartColor = StandardColor;
				if (InOrOutMovement == InOrOut.OUT) {
					StartColor.a = 0f;
				}
				PanelImage.color = StartColor;
				DestroyAnimation ();
			}
	
		}
	}
}
}