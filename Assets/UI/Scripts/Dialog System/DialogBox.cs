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


	public class Choice {
		public Choice (string text, ChoiceSelect del) {
			this.text = text;
			this.choiceDelegate = del;
		}

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


	/// <summary>
	/// The prefab used to display choices.
	/// </summary>
	public GameObject choicePrefab;


	private GameObject continueButton;
	private Text textObject;


	private float lastLetterTime;
	private int indexStart = 0;
	private int indexEnd = 0;
	private bool finished = false;


	// Use this for initialization
	void Start () {
		textObject = gameObject.GetComponentInChildren<Text> ();
		continueButton = gameObject.GetComponentInChildren<DoSomethingOnClick> ().gameObject;

		textObject.text = "";
		continueButton.SetActive (false);
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

					if (finishDelegate != null)
						DisplayContinueButton ();
					if (choices != null && choices.Length > 0)
						DisplayChoices ();
				}
			}

			textObject.text = text.Substring (indexStart, indexEnd - indexStart);
		} else {
		}
	}

	private void DisplayContinueButton () {
		continueButton.SetActive (true);
		var clickScript = continueButton.GetComponent<DoSomethingOnClick> ();
		clickScript.clickDelegate = delegate () {
			finishDelegate ();
		};
	}

	private void DisplayChoices () {

		var height = choicePrefab.GetComponent<RectTransform> ().rect.height;

		for (int i = 0; i < choices.Length; i++) {

			var choice = choices [i];


			var clone = GameObject.Instantiate (choicePrefab, transform) as GameObject;
			var rect = clone.GetComponent<RectTransform> ();
			var choiceScript = clone.GetComponent<DialogChoice> ();


			rect.anchorMax = Vector2.zero;
			rect.anchorMin = Vector2.zero;
			rect.pivot = new Vector2 (0, 1);
			rect.anchoredPosition = new Vector2 (10, 10 - (height + 5) * i);


			choiceScript.SetText (choice.text);


			var doSomethingScript = clone.AddComponent<DoSomethingOnClick> ();
			doSomethingScript.clickDelegate = () => choice.choiceDelegate ();
		}
	}
}
