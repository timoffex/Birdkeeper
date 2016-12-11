using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;


public class FurnitureItemDisplayer : MonoBehaviour, IPointerDownHandler {

	public Image furnitureIcon;
	public Text furnitureName;
	public Text furnitureCount;

	public AudioClip placedSuccessfullySound;

	private Furniture_hovering hoverPrefab;
	private FurnitureStack representedStack;
	private bool okayToDrag = false;

	public void DisplayFurnitureItem (FurnitureStack stack) {
		MetaInformation info = MetaInformation.Instance ();

		if (info != null) {
			GameObject furnitureObj = info.GetFurniturePrefabByID (stack.FurnitureID);

			if (furnitureObj != null) {

				Furniture furniture = furnitureObj.GetComponent<Furniture> ();

				if (furnitureIcon != null) furnitureIcon.sprite = furniture.GetIcon ();
				if (furnitureName != null) furnitureName.text = furnitureObj.name;
				if (furnitureCount != null) furnitureCount.text = stack.Count.ToString ();

				representedStack = stack;
				okayToDrag = true;
				hoverPrefab = furniture.GetHoveringPrefab ();

			} else
				Debug.LogErrorFormat ("Furniture doesn't exist! Something's wrong.");


		} else
			Debug.Log ("MetaInformation is null.");
	}

	public void OnPointerDown (PointerEventData data) {
		if (okayToDrag)
			StartCoroutine (StartPlacing (representedStack, GameObject.Instantiate (hoverPrefab)));
	}



	private IEnumerator StartPlacing (FurnitureStack stack, Furniture_hovering draggedObject) {

		if (stack.Count > 0) {
			if (furnitureCount != null)
				furnitureCount.text = (stack.Count - 1).ToString ();

			while (true) {
				if (Input.GetMouseButton (0)) {
					draggedObject.PositionOverMouse ();
					yield return new WaitForFixedUpdate ();
				} else {
					if (PlaceHoveringFurniture (stack.FurnitureID, draggedObject)) {
						// Placed successfully. Subtract item from Game.current.furnitureInventory
						Game.current.furnitureInventory.SubtractOne (stack.FurnitureID);
						PlaySound (placedSuccessfullySound);
					} else {
						// Didn't place successfully. Restore the count.
						furnitureCount.text = stack.Count.ToString ();
					}

					Destroy (draggedObject.gameObject);
					break;
				}
			}

		} else {
			Debug.Log ("You don't have enough of this furniture item.");
			Destroy (draggedObject.gameObject);
		}

	}


	private bool PlaceHoveringFurniture (uint fid, Furniture_hovering draggedObject) {
		Shop shop = Game.current.shop;

		if (shop != null) {
			var shopCoords = draggedObject.GetShopPosition ();

			var furnitureObj = Furniture.InstantiateFurnitureByID (fid);
			var furniture = furnitureObj.GetComponent<Furniture> ();


			if (!furniture.PlaceAtLocation (shop, shopCoords)) {
				Destroy (furnitureObj);
				return false;
			} else
				return true;
		} else {
			Debug.LogError ("No shop object found!");
			return false;
		}
	}

	private void PlaySound (AudioClip ac) {
		AudioSource src = FindObjectOfType<AudioSource> ();
		if (src != null)
			src.PlayOneShot (ac);
	}
}
