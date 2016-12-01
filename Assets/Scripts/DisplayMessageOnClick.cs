using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider2D))]
public class DisplayMessageOnClick : MonoBehaviour {
	public string message;

	// Use this for initialization
	void Start () {
		Collider2D col = GetComponent<Collider2D> ();
		if (col == null)
			col = gameObject.AddComponent<BoxCollider2D> ();

		ShopEventSystem.Instance ().RegisterClickListener (col, () => BeginDisplayingMessage ());
	}

	void BeginDisplayingMessage () {
		StartCoroutine (DialogSystem.Instance ().DisplayMessageAndChoices (message,
			new DialogBox.Choice ("Chirp?", () => print ("Uh huh.")),
			new DialogBox.Choice ("I see, I see.", () => print ("No you don't."))));
	}
}
