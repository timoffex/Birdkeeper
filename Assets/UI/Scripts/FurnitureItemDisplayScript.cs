using UnityEngine;
using System.Collections;

public class FurnitureItemDisplayScript : MonoBehaviour {

	public FurnitureItemDisplayer furnitureDisplayPrefab;

	void Start () {
		Game g = Game.current;

		if (g != null) {

			foreach (Transform child in transform)
				Destroy (child.gameObject);

			foreach (var furnitureItemStack in g.furnitureInventory) {
				var displayer = GameObject.Instantiate (furnitureDisplayPrefab, transform) as FurnitureItemDisplayer;
				displayer.DisplayFurnitureItem (furnitureItemStack.fid, furnitureItemStack.count);
			}

		} else
			Debug.Log ("Game is null!");
	}
}
