
using UnityEngine;
using UnityEngine.EventSystems;


public class EditorDraggedObject : MonoBehaviour {

	public IEditorDraggable draggedObject;

	private Shop shop;

	void Awake () {
		shop = GameObject.Find ("Room").GetComponent<Shop> ();
	}

	void Update () {
		FollowMouse ();

		if (Input.GetMouseButtonUp (0))
			PlaceDown ();
	}

	private void FollowMouse () {
		var z = transform.position.z;
		var p = draggedObject.GetPositionFromMouse ();

		// snap!
		p = shop.shopToWorldCoordinates (
				shop.worldToShopCoordinates (p));

		transform.position = new Vector3(p.x, p.y, z);
	}

	public void PlaceDown() {
		// Clone and place
		draggedObject.PlaceCloneAtMousePosition (); // TODO: this may return false

		// Delete self
		DestroyImmediate (gameObject);
	}
}
