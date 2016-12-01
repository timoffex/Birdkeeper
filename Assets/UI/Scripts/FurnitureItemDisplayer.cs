using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;


public class FurnitureItemDisplayer : MonoBehaviour, IPointerDownHandler {

	public Image furnitureIcon;
	public Text furnitureName;
	public Text furnitureCount;

	private System.Action dragOutDelegate;

	public void DisplayFurnitureItem (uint fid, uint count) {
		MetaInformation info = MetaInformation.Instance ();

		if (info != null) {
			GameObject furnitureObj = info.GetFurniturePrefabByID (fid);

			if (furnitureObj != null) {

				Furniture furniture = furnitureObj.GetComponent<Furniture> ();

				if (furnitureIcon != null) furnitureIcon.sprite = furniture.GetIcon ();
				if (furnitureName != null) furnitureName.text = furnitureObj.name;
				if (furnitureCount != null) furnitureCount.text = count.ToString ();

				dragOutDelegate = delegate {
					Debug.Log ("Creating furniture.");

				};

			} else
				Debug.LogErrorFormat ("Furniture doesn't exist! Something's wrong.");


		} else
			Debug.Log ("MetaInformation is null.");
	}

	public void OnPointerDown (PointerEventData data) {
		if (dragOutDelegate != null)
			dragOutDelegate ();
	}



	private IEnumerator StartPlacing () {
		yield return null;
	}
}
