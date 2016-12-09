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

[RequireComponent (typeof (RectTransform))]
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
	[SerializeField] private string text;
	[SerializeField] private float secondsPerLetter = 0.1f;


	/// <summary>
	/// Optional choices to display to the user.
	/// </summary>
	private Choice[] choices;

	/// <summary>
	/// Function to call when all text has been displayed and user clicks continuing button.
	/// </summary>
	private DialogFinish finishDelegate;


	/// <summary>
	/// The prefab used to display choices.
	/// </summary>
	[SerializeField] private GameObject choicePrefab;


	[SerializeField] private GameObject continueButton;
	[SerializeField] private Text textObject;


	private float lastLetterTime;
	private int indexStart = 0;
	private int indexEnd = 0;
	private bool finished = false;



	#region Functions for initializing Dialog Boxes.
	public void DisplayMessageAndContinueButton (string mssg, DialogFinish finishDelegate = null) {
		text = mssg;
		this.finishDelegate = finishDelegate;
	}

	public void DisplayMessageAndChoices (string mssg, Choice[] choices, DialogFinish finishDelegate = null) {
		text = mssg;
		this.choices = choices;
		this.finishDelegate = finishDelegate;
	}
	#endregion




	public void FinishDialog () {
		if (finishDelegate != null)
			finishDelegate ();
	}




	// Use this for initialization
	void Start () {
		textObject.text = "";
		continueButton.gameObject.SetActive (false);

		// position self
		var rect = GetComponent<RectTransform> ();
		rect.anchorMin = Vector2.one;
		rect.anchorMax = Vector2.one;
		rect.pivot = Vector2.one;
		rect.anchoredPosition = new Vector2 (-24, -24);
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
		}
	}

	private void DisplayContinueButton () {
		continueButton.gameObject.SetActive (true);
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
