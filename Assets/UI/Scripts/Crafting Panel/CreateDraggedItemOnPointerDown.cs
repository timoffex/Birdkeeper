using UnityEngine;


public class CreateDraggedItemOnPointerDown : MonoBehaviour {

	private DraggedItem draggedItemPrefab;
	private RectTransform parent;
	private ItemType item;

	private System.Func<bool> onTakeItem;
	private System.Action onReturnItem;


	void Update () {
		if (Input.GetMouseButtonDown (0) && draggedItemPrefab != null) {


			if (RectTransformUtility.RectangleContainsScreenPoint (transform as RectTransform, Input.mousePosition)) {
				if (onTakeItem ()) {
					var dragged = GameObject.Instantiate (draggedItemPrefab, parent) as DraggedItem;
					dragged.transform.SetAsLastSibling ();
					dragged.SetItem (item, onReturnItem);
				}
			}
		}
	}



	public void MakeFor (ItemType item, RectTransform parentPanel, DraggedItem draggedItemPrefab,
		System.Func<bool> onTakeItem,
		System.Action onReturnItem) {

		parent = parentPanel;
		this.item = item;
		this.draggedItemPrefab = draggedItemPrefab;
		this.onReturnItem = onReturnItem;
		this.onTakeItem = onTakeItem;
	}
}