using UnityEngine;
using System.Collections;


[RequireComponent (typeof (SpriteRenderer)),
	RequireComponent (typeof (Collider2D))]
public class ChangeTintOnHover : MonoBehaviour {

	private Color originalTint;
	public Color hoverTint;

	private SpriteRenderer sr;
	private Collider2D col;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer> ();
		col = GetComponent<Collider2D> ();

		originalTint = sr.color;
	}
	
	// Update is called once per frame
	void Update () {
		if (col.OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition)))
			sr.color = hoverTint;
		else
			sr.color = originalTint;
	}
}
