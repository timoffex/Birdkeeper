using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


[RequireComponent (typeof (RectTransform))]
public class CraftingSlot : MonoBehaviour {
	private ItemType item;
	private Action onRemoveItem;


	public Image itemIcon;



	public void SetItem (ItemType itemType, Action onRemove) {
		if (onRemoveItem != null)
			onRemoveItem ();

		item = itemType;
		onRemoveItem = onRemove;

		itemIcon.gameObject.SetActive (true);
		itemIcon.sprite = itemType.Icon;
	}

	public ItemType GetItem () {
		return item;
	}

	public void Clear () {
		if (onRemoveItem != null)
			onRemoveItem ();

		item = null;
		onRemoveItem = null;
		itemIcon.sprite = null;

		if (itemIcon.gameObject != gameObject)
			itemIcon.gameObject.SetActive (false);
	}
}
