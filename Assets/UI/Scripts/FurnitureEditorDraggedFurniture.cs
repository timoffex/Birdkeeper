using UnityEngine;
using UnityEngine.EventSystems;


public class FurnitureEditorDraggedFurniture : MonoBehaviour {

	public Furniture draggedObject;
	public uint furnitureID;

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
		var newFurnitureObj = Furniture.InstantiateFurnitureByID (furnitureID);
		var furniture = newFurnitureObj.GetComponent<Furniture> ();

		var placementCoords = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		if (!furniture.PlaceAtLocation (shop, shop.worldToShopCoordinates (placementCoords)))
			Destroy (newFurnitureObj);

		// Delete self
		Destroy (gameObject);
	}

	private Vector3 GetSnappedMouseCoords () {
		return shop.shopToWorldCoordinates (
			shop.worldToShopCoordinates (Camera.main.ScreenToWorldPoint (Input.mousePosition)));
	}
}
