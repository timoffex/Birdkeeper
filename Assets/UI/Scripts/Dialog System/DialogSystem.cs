using UnityEngine;
using System.Collections;

public class DialogSystem : MonoBehaviour {

	/// <summary>
	/// Call this to get the DialogSystem object in the game. If it doesn't
	/// exist, this method will return null meaning that you can't make a dialog.
	/// </summary>
	public static DialogSystem Instance () {
		return FindObjectOfType<DialogSystem> ();
	}


	/// <summary>
	/// This will be cloned for every new dialog box.
	/// </summary>
	[SerializeField] private DialogBox dialogBoxPrefab;

	/// <summary>
	/// Where to put dialog boxes.
	/// </summary>
	[SerializeField] private RectTransform targetContainer;



	/// <summary>
	/// A coroutine that displays the given message and stops when the
	/// message is done being displayed and the player presses Continue.
	/// </summary>
	/// <param name="text">The message to display.</param>
	public IEnumerator DisplayMessage (string text) {
		var dbox = MakeNewDialogBox ();

		bool dialogFinished = false;
		dbox.DisplayMessageAndContinueButton (text, () => {
			dialogFinished = true;
		});

		yield return new WaitUntil (() => dialogFinished);

		Destroy (dbox.gameObject);
	}


	/// <summary>
	/// A coroutine that displays a message with some choices and stops when
	/// a choice is picked.
	/// </summary>
	/// <param name="text">The message to display.</param>
	/// <param name="choices">The choices to give to the user. If empty, the call
	/// is equivalent to DisplayMessage (text).</param>
	public IEnumerator DisplayMessageAndChoices (string text, params DialogBox.Choice[] choices) {
		if (choices.Length == 0) // If no choices provided, just use the default display with the continue button.
			yield return DisplayMessage (text);
		else {
			var dbox = MakeNewDialogBox ();


			// This will be false until a choice is selected.
			bool choiceSelected = false;


			// Makes the provided choices set the "choiceSelected" variable to true.
			var wrappedChoices = new DialogBox.Choice[choices.Length];
			for (int i = 0; i < choices.Length; i++) {
				var num = i;
				var choice = choices [num];

				string t = choice.text;
				DialogBox.ChoiceSelect del = CombineDelegates (() => {
					choiceSelected = true;
				}, choice.choiceDelegate);

				wrappedChoices [i] = new DialogBox.Choice (t, del);
			}


			dbox.DisplayMessageAndChoices (text, wrappedChoices);


			yield return new WaitUntil (() => choiceSelected);

			Destroy (dbox.gameObject);
		}
	}


	private DialogBox MakeNewDialogBox () {
		var clone = GameObject.Instantiate (dialogBoxPrefab, targetContainer) as DialogBox;

		return clone;
	}

	private DialogBox.ChoiceSelect CombineDelegates (DialogBox.ChoiceSelect c0, DialogBox.ChoiceSelect c1) {
		return delegate () {
			c0 ();
			c1 ();
		};
	}
}
