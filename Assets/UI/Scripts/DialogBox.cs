using UnityEngine;
using System.Collections;

using UnityEngine.UI;


/// <summary>
/// Displays the given text.
/// 
/// If choices are provided, the dialog will call the appropriate choice's delegate
/// when it is selected.
/// 
/// If no choices are provided, then the dialog will call the finishDelegate when
/// all text has been displayed and the user chooses to continue.
/// </summary>
public class DialogBox : MonoBehaviour {

	public delegate void DialogFinish ();
	public delegate void ChoiceSelect ();


	public struct Choice {
		public string text;
		public ChoiceSelect choiceDelegate;
	}


	/// <summary>
	/// Text to display.
	/// </summary>
	public string text;


	public float secondsPerLetter = 0.1f;


	/// <summary>
	/// Optional choices to display to the user.
	/// </summary>
	public Choice[] choices;

	/// <summary>
	/// Function to call when all text has been displayed and user clicks continuing button.
	/// </summary>
	public DialogFinish finishDelegate;



	private GameObject continueButtonPrefab;




	private Text textPrefab;


	private float lastLetterTime;
	private int indexStart = 0;
	private int indexEnd = 0;
	private bool finished = false;


	// Use this for initialization
	void Start () {
		textPrefab = gameObject.GetComponentInChildren<Text> ();
		continueButtonPrefab = gameObject.GetComponentInChildren<DialogContinueButton> ().gameObject;

		textPrefab.text = "";
		continueButtonPrefab.SetActive (false);
	}

	void Update () {
		var time = Time.time;

		if (!finished) {
			if (time - lastLetterTime > secondsPerLetter) {
				indexEnd = indexEnd + 1;
				lastLetterTime = time;

				if (indexEnd > text.Length) {
					indexEnd = text.Length;
					finished = true;

					DisplayContinueButton ();
				}
			}

			textPrefab.text = text.Substring (indexStart, indexEnd - indexStart);
		} else {

		}
	}

	private void DisplayContinueButton () {
		continueButtonPrefab.SetActive (true);
		var continueButtonScript = continueButtonPrefab.GetComponent<DialogContinueButton> ();
		continueButtonScript.continueDelegate = delegate () {
			if (finishDelegate != null)
				finishDelegate ();
		};
	}
}
