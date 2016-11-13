using UnityEngine;
using System.Collections;

public class DialogSystem : MonoBehaviour {

	private static DialogSystem instance;
	public static DialogSystem Instance () {
		return instance;
	}

	private static Transform GetCanvas () {
		return GameObject.Find ("Canvas").GetComponent<Transform> (); // TODO
	}



	public GameObject dialogBoxPrefab;


	void Start () {
		instance = this;

		StartCoroutine (LateStart ());
	}

	IEnumerator LateStart () {
		print ("Starting!");
		yield return DisplayMessageAndChoices ("Pick some choice, then check the console.",
			new DialogBox.Choice ("choice1", delegate () { print ("choice1 selected"); }),
			new DialogBox.Choice ("choice2", delegate () { print ("choice2 selected"); }),
			new DialogBox.Choice ("choice3", delegate () { print ("choice3 selected"); }));
		print ("Finished!");
	}


	/// <summary>
	/// A coroutine that displays the given message and stops when the
	/// message is done being displayed and the player presses Continue.
	/// </summary>
	/// <param name="text">The message to display.</param>
	public IEnumerator DisplayMessage (string text) {
		var clone = CloneAndPositionDialogBox ();

		var dbox = clone.GetComponent<DialogBox> ();
		dbox.text = text;

		bool dialogFinished = false;
		dbox.finishDelegate = delegate () {
			dialogFinished = true;
		};

		yield return new WaitUntil (() => dialogFinished);

		Destroy (clone);
	}


	/// <summary>
	/// A coroutine that displays a message with some choices and stops when
	/// a choice is picked.
	/// </summary>
	/// <param name="text">The message to display.</param>
	/// <param name="choices">The choices to give to the user. If empty, the call
	/// is equivalent to DisplayMessage (text).</param>
	public IEnumerator DisplayMessageAndChoices (string text, params DialogBox.Choice[] choices) {
		if (choices.Length == 0)
			yield return DisplayMessage (text);
		else {
			var clone = CloneAndPositionDialogBox ();

			var dbox = clone.GetComponent<DialogBox> ();
			dbox.text = text;

			bool choiceSelected = false;


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

			dbox.choices = wrappedChoices;

			yield return new WaitUntil (() => choiceSelected);

			Destroy (clone);
		}
	}


	private GameObject CloneAndPositionDialogBox () {
		var clone = GameObject.Instantiate (dialogBoxPrefab, GetCanvas ()) as GameObject;

		var rect = clone.GetComponent<RectTransform> ();
		rect.anchorMax = Vector2.one;
		rect.anchorMin = Vector2.one;
		rect.anchoredPosition = new Vector2 (-24, -24);

		return clone;
	}

	private DialogBox.ChoiceSelect CombineDelegates (DialogBox.ChoiceSelect c0, DialogBox.ChoiceSelect c1) {
		return delegate () {
			c0 ();
			c1 ();
		};
	}
}
