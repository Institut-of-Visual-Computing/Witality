using UnityEngine;

namespace Utils.Unity.Animation
{

/// <summary>
/// Das Script verwendet die LocalPosition als Ausgangspunkt für die Berechnungen
/// es wird entweder von der Local position nach draußen bewegt oder zur Local position hin
/// Das Script kombiniert sich selbst mit dem Fader Script
/// </summary>
public class UIAnimationMoveInAndOutBehaviour : AbstractUIAnimationBehaviour
{
	
	/// <summary>
	/// Bewegungsdistanz errechnet sich aus der Größe des Objekt in dern Bewegungsrichtung mal diesen Faktor. Standard ist 2.
	/// </summary>
	public int MoveDistanceFactor = 2;

	//funktioniert nicht wie erwartet
	//	public bool UseInitialStartPosition = false;

	public Transform TransformToAnimate;

	private Vector3 TargetPosition = Vector3.zero;

	private Vector3 StartPosition = Vector3.zero;

	//	private Vector3 InitialStartPosition;

	// Use this for initialization
	public override void Init ()
	{
		
		if (TransformToAnimate == null) {
			TransformToAnimate = ObjectToAnimate.transform;
		}
//		if (UseInitialStartPosition) {
//			InitialStartPosition = TransformToAnimate.position;
//		}
		if (InOrOutMovement == InOrOut.IN) {
			FadeInAnimation (DontDestroyAfterAnim);
		} else {
			FadeOutAnimation (DontDestroyAfterAnim);
		}
		if (RandomMoveSpeed) {
			Speed = Random.Range (5f, 15f);
		}
//		UIAnimationFadeBehaviour fadeAnimation = gameObject.AddComponent<UIAnimationFadeBehaviour> ();
//		fadeAnimation.InOrOutMovement = InOrOutMovement;
//		DestroyAfterAnim = false;
	}

	public override void FadeOutContent ()
	{
//		if (UseInitialStartPosition) {
//			TransformToAnimate.localPosition = InitialStartPosition;
//		}
		StartPosition = TransformToAnimate.localPosition;
		TargetPosition = GetDistantPosition ();
	}

	public override void FadeInContent ()
	{
//		if (UseInitialStartPosition) {
//			TransformToAnimate.localPosition = InitialStartPosition;
//		}
		TargetPosition = TransformToAnimate.localPosition;
		StartPosition = GetDistantPosition ();
		TransformToAnimate.localPosition = StartPosition;
	}

	/// <summary>
	/// gibt die äußere Position abhängig von der richtungsangabe wieder
	/// diese wird dann als start oder target position verwendet,
	/// je nachdem ob nach innen oder nach außen geflogen wird.
	/// </summary>
	/// <returns>The distant position.</returns>
	private Vector3 GetDistantPosition ()
	{
		Vector3 targetPosition = TransformToAnimate.localPosition;
		RectTransform OBRectTrans = ObjectToAnimate.GetComponent<RectTransform> ();
		Rect OBRect = OBRectTrans.rect;
		//Berechnung der Startposition
		if (MoveDirectionMovement == MoveDirection.DOWN) {
			targetPosition = new Vector3 (TransformToAnimate.localPosition.x, TransformToAnimate.localPosition.y + OBRect.height * MoveDistanceFactor, TransformToAnimate.localPosition.z);
		} else if (MoveDirectionMovement == MoveDirection.UP) {
			targetPosition = new Vector3 (TransformToAnimate.localPosition.x, TransformToAnimate.localPosition.y - OBRect.height * MoveDistanceFactor, TransformToAnimate.localPosition.z);
		} else if (MoveDirectionMovement == MoveDirection.LEFT) {
			targetPosition = new Vector3 (TransformToAnimate.localPosition.x + OBRect.width * MoveDistanceFactor, TransformToAnimate.localPosition.y, TransformToAnimate.localPosition.z);
		} else if (MoveDirectionMovement == MoveDirection.RIGHT) {
			targetPosition = new Vector3 (TransformToAnimate.localPosition.x - OBRect.width * MoveDistanceFactor, TransformToAnimate.localPosition.y, TransformToAnimate.localPosition.z);
		}
		return targetPosition;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (AnimationRunning) {
			if (Vector3.Distance (StartPosition, TargetPosition) > 0.01f) {
				StartPosition = Vector3.Lerp (StartPosition, TargetPosition, Time.deltaTime * Speed);
				TransformToAnimate.localPosition = StartPosition;
			} else {
				AnimationRunning = false;
				TransformToAnimate.localPosition = TargetPosition;
				DestroyAnimation ();
			}
		}
	}
}
}