using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// How to use: add to a GameObject that should act like a notification. Make
/// sure myAnimator / animationStartTrigger / animationStopTrigger / myText
/// aren't null. The animator should call the FinishDestroy() method at the
/// end of its stopping animation.
/// </summary>
public class NotificationObject : MonoBehaviour {

	public Animator myAnimator;

	public string animatorStopTrigger;

	public Text myText;


	private float startTime;
	private float deltaTime = 1;


	public void ShowTextForTime (string txt, float timeSeconds) {
		deltaTime = timeSeconds;
		startTime = Time.time;
		myText.text = txt;
	}


	void Update () {
		if (Time.time - startTime >= deltaTime)
			DestroySelf ();
	}


	private void DestroySelf () {
		myAnimator.SetTrigger (animatorStopTrigger);
	}

	public void FinishDestroy () {
		Destroy (gameObject);
	}
}
