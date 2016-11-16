
using UnityEngine;
using UnityEngine.EventSystems;


public class EditorDraggedObject : MonoBehaviour {

	public IEditorDraggable draggedObject;

	private Shop shop { get { return Shop.Instance (); } }

	void Update () {
		FollowMouse ();

		if (Input.GetMouseButtonUp (0))
			PlaceDown ();
	}

	private void FollowMouse () {

		var mouseSnapped = GetSnappedMouseCoords ();


		var z = transform.position.z;
		var p = draggedObject.GetPivotPosition (mouseSnapped);

		transform.position = new Vector3(p.x, p.y, z);
	}

	public void PlaceDown() {
		// Clone and place
		draggedObject.PlaceCloneAtPosition (Camera.main.ScreenToWorldPoint (Input.mousePosition)); // TODO: this may return false

		// Delete self
		DestroyImmediate (gameObject);
	}

	private Vector3 GetSnappedMouseCoords () {
		return shop.shopToWorldCoordinates (
			shop.worldToShopCoordinates (Camera.main.ScreenToWorldPoint (Input.mousePosition)));
	}
}
