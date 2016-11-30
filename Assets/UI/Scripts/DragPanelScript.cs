using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DragPanelScript : MonoBehaviour, IPointerDownHandler, IDragHandler {

	private RectTransform panelRect;
	private Vector2 localOffset;

	// Use this for initialization
	void Awake () {
		panelRect = transform.parent.gameObject.GetComponentInParent<RectTransform> ();
	}

	public void OnPointerDown (PointerEventData data) {
		panelRect.SetAsLastSibling ();

		localOffset = (Vector2)panelRect.position - data.position;
	}

	public void OnDrag (PointerEventData data) {
		panelRect.position = data.position + localOffset;
	}
}
