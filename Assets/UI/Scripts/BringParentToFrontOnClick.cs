using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class BringParentToFrontOnClick : MonoBehaviour, IPointerDownHandler {

	private RectTransform panelRect;
	private Vector2 localOffset;

	// Use this for initialization
	void Awake () {
		panelRect = transform.parent as RectTransform;
	}

	public void OnPointerDown (PointerEventData data) {
		panelRect.SetAsLastSibling ();
	}
}
