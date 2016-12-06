using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class BubbleLayout : MonoBehaviour {


	public bool isVertical = true;

	/// <summary>
	/// True means left-to-right or top-to-bottom, depending on Is Vertical setting.
	/// False means right-to-left or bottom-to-top.
	/// </summary>
	public bool naturalDirection = true;


	public float bubblingSpeed = 1;
	public float bubblingSensitivity = 1;

	/// <summary>
	/// Children are ordered by their index.
	/// </summary>
	private List<RectTransform> childrenList;



	// Use this for initialization
	void Awake () {
		childrenList = new List<RectTransform> ();
		foreach (RectTransform child in transform)
			childrenList.Add (child);
	}
	
	// Update is called once per frame
	void Update () {
		childrenList.Clear ();
		foreach (RectTransform child in transform)
			childrenList.Add (child);

		if (childrenList.Count > 0) {
			ValidateChildren ();
			MoveChildren ();
		}
	}


	/// <summary>
	/// Makes sure no child is null and sets the anchors/pivots of the children.
	/// </summary>
	private void ValidateChildren () {

		childrenList = childrenList.Where ((child) => child != null).ToList ();
		childrenList.ForEach ((child) => {

			float x, y;

			if (isVertical) {
				x = 0.5f;

				child.anchoredPosition = new Vector2 (0, child.anchoredPosition.y);
				if (naturalDirection)  	y = 1;
								 else   y = 0;
			} else {
				y = 0.5f;

				child.anchoredPosition = new Vector2 (child.anchoredPosition.x, 0);
				if (naturalDirection)	x = 0;
								 else   x = 1;
			}


			child.anchorMin = new Vector2 (x, y);
			child.anchorMax = new Vector2 (x, y);
			child.pivot = new Vector2 (x, y);
		});
	}


	private void MoveChildren () {
		float[] targetPositions = CalculateTargetPositions ();


		for (int i = 0; i < childrenList.Count; i++) {
			RectTransform t = childrenList [i];

			float position = GetPosition (t);

			float delta = targetPositions [i] - position;
			float dist = Mathf.Abs (delta);

			// movement speed = bubblingSpeed * (1 + bubblingSensitivity * dist)

			float speed = bubblingSpeed * (1 + bubblingSensitivity * dist);
			float velocity = speed * Mathf.Sign (delta);

			float newPosition = position + velocity;

			// If movement causes us to overshoot, don't overshoot.
			if ((targetPositions [i] - newPosition < 0) != (delta < 0))
				newPosition = targetPositions [i];


			SetPosition (t, newPosition);
		}
	}


	private float[] CalculateTargetPositions () {
		float[] heights = new float[childrenList.Count];

		if (heights.Length == 0)
			return heights;


		heights [0] = 0;

		for (int i = 1; i < childrenList.Count; i++)
			if (isVertical != naturalDirection)
				heights [i] = heights [i - 1] + GetSize (childrenList [i - 1]);
			else
				heights [i] = heights [i - 1] - GetSize (childrenList [i - 1]);

		return heights;
	}


	private void SetPosition (RectTransform t, float p) {
		if (isVertical)
			t.anchoredPosition += new Vector2 (0, p - GetPosition (t));
		else
			t.anchoredPosition += new Vector2 (p - GetPosition (t), 0);
	}

	private float GetPosition (RectTransform t) {
		if (isVertical) {
			return t.anchoredPosition.y;
		} else {
			return t.anchoredPosition.x;
		}
	}

	private float GetSize (RectTransform t) {
		if (isVertical)
			return t.rect.size.y;
		else
			return t.rect.size.x;
	}
}
