using UnityEngine;

namespace Utils.Unity.Animation
{

	public abstract class AbstractUIAnimationBehaviour : MonoBehaviour
	{
		public bool DontDestroyAfterAnim;
		public GameObject GameObjectToDestroy;
		public GameObject Background;
		public GameObject ObjectToAnimate;
		public bool AnimationRunning = true;

		// Use this for initialization
		void Start ()
		{
			if (ObjectToAnimate == null) {
				ObjectToAnimate = gameObject;
			}
			Init ();
		}

		public virtual void Init ()
		{

		}

		public enum MoveDirection
		{
			LEFT,
			RIGHT,
			UP,
			DOWN
		}

		public enum InOrOut
		{
			IN,
			OUT
		}

		public virtual void SetOppositeMoveDirection ()
		{
			if (MoveDirectionMovement == MoveDirection.DOWN) {
				MoveDirectionMovement = MoveDirection.UP;
			} else if (MoveDirectionMovement == MoveDirection.UP) {
				MoveDirectionMovement = MoveDirection.DOWN;
			} else if (MoveDirectionMovement == MoveDirection.LEFT) {
				MoveDirectionMovement = MoveDirection.RIGHT;
			} else if (MoveDirectionMovement == MoveDirection.RIGHT) {
				MoveDirectionMovement = MoveDirection.LEFT;
			}
		}

		/// <summary>
		/// Wird das Objekt von seinem Zielplatz weg oder hin bewegt?
		/// </summary>
		public InOrOut InOrOutMovement = InOrOut.IN;
		/// <summary>
		/// Kommt das Objekt von Oben, Unten, rechts oder links herein?
		/// </summary>
		public MoveDirection MoveDirectionMovement = MoveDirection.DOWN;

		public bool RandomMoveSpeed = false;

		public float Speed = 10f;

		/// <summary>
		/// Löscht nur, wenn dont Destroy false ist
		/// Löscht bei Out Movement ein spezifiziertes Objekt oder das Objekt an dem Das Script hängt
		/// Bei in Movement wird nur das Script selbst gelöscht
		/// </summary>
		protected void DestroyAnimation ()
		{
			if (!DontDestroyAfterAnim) {
	//			print ("Lösche " + this + " von " + gameObject.name);
				if (InOrOutMovement == InOrOut.IN) {
					Destroy (this);
				} else {
					if (GameObjectToDestroy != null) {
						Destroy (GameObjectToDestroy);
					} else {
						Destroy (gameObject);
					}
				}
			} else {
				if (InOrOutMovement == InOrOut.OUT) {
					if (GameObjectToDestroy != null) {
						GameObjectToDestroy.SetActive (false);
					} else {
						gameObject.SetActive (true);
					}
				}
			}
		}

		public virtual void FadeOutAnimation (bool dontDestroy = true)
		{
			AnimationRunning = true;
			InOrOutMovement = InOrOut.OUT;
			FadeOutBackground ();
			FadeOutContent ();
			DontDestroyAfterAnim = dontDestroy;
		}

		public virtual void FadeOutBackground ()
		{
			if (Background != null) {
				UIAnimationFadeBehaviour fadeAnimation = Background.GetComponent<UIAnimationFadeBehaviour> ();
				if (fadeAnimation == null) {
					fadeAnimation = Background.AddComponent<UIAnimationFadeBehaviour> ();
				}
				fadeAnimation.InOrOutMovement = InOrOut.OUT;
				fadeAnimation.DontDestroyAfterAnim = DontDestroyAfterAnim;
				fadeAnimation.FadeOutAnimation ();
			} else {

			}
		}

		public virtual void FadeOutContent ()
		{
			AnimationRunning = true;
		}

		public virtual void FadeInAnimation (bool dontDestroy = true)
		{
			AnimationRunning = true;
			InOrOutMovement = InOrOut.IN;
			FadeInBackground ();
			FadeInContent ();
			DontDestroyAfterAnim = dontDestroy;
		}

		public virtual void FadeInBackground ()
		{
			if (Background != null) {
				UIAnimationFadeBehaviour fadeAnimation = Background.GetComponent<UIAnimationFadeBehaviour> ();
				if (fadeAnimation == null) {
					fadeAnimation = Background.AddComponent<UIAnimationFadeBehaviour> ();
				}
				fadeAnimation.InOrOutMovement = InOrOut.IN;
				fadeAnimation.DontDestroyAfterAnim = DontDestroyAfterAnim;
				fadeAnimation.FadeInAnimation ();
			}
		}

		public virtual void FadeInContent ()
		{
			AnimationRunning = true;
		}
	}
}
