using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System;
using System.Linq;
using System.Collections.Generic;


public class DraggedItem : MonoBehaviour {

	[SerializeField] private Image itemIcon;

	private ItemType draggedItem;
	private Action onItemReturn;

	void Update () {
		// Follow mouse
		transform.position = Input.mousePosition;

		if (!Input.GetMouseButton (0))
			Drop ();
	}



	public void SetItem (ItemType item, Action onItemReturn) {
		draggedItem = item;
		itemIcon.sprite = item.Icon;
		this.onItemReturn = onItemReturn;
	}



	private void Drop () {

		var allSlots = FindObjectsOfType<CraftingSlot> ();

		for (int i = 0; i < allSlots.Length; i++) {
			var slot = allSlots [i];
			var slotRect = slot.GetComponent<RectTransform> ();

			if (RectTransformUtility.RectangleContainsScreenPoint (slotRect, Input.mousePosition)) {
				slot.SetItem (draggedItem, onItemReturn);
				Destroy (gameObject); // destroy early before calling onItemReturn ()
				return;
			}
		}

		onItemReturn ();
		Destroy (gameObject);
	}
}
