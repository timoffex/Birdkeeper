using UnityEngine;
using System.Collections;

public class FurnitureItemDisplayScript : MonoBehaviour {

	public FurnitureItemDisplayer furnitureDisplayPrefab;

	void Start () {
		Game g = Game.current;

		if (g != null) {

			foreach (Transform child in transform)
				Destroy (child.gameObject);

			foreach (var furnitureItemStack in g.furnitureInventory.GetFurnitureStacks ()) {
				var displayer = GameObject.Instantiate (furnitureDisplayPrefab, transform) as FurnitureItemDisplayer;
				displayer.DisplayFurnitureItem (furnitureItemStack);
			}

		} else
			Debug.Log ("Game is null!");
	}
}
