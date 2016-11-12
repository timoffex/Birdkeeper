using UnityEngine;
using System.Collections;

public class LootDropper : MonoBehaviour {

	public Transform droppedPrefab;

	/// <summary>
	/// Helper function to drop the given droppedPrefab.
	/// </summary>
	public virtual void DropItem () {
		IDroppable dropIface = droppedPrefab.GetComponent<IDroppable> ();


		if (dropIface) {
			// If droppedPrefab has component IDroppable, use its DropAt () function.
			dropIface.DropAt (GetShop (), GetLocation ());
		} else {
			// Otherwise, just use a default dropping animation.

			var clone = GameObject.Instantiate (droppedPrefab, transform.parent) as GameObject;

			clone.transform.position = transform.position;

		}
	}


	private Shop myShop;
	private Shop GetShop () {
		if (!myShop)
			myShop = GameObject.Find ("Room").GetComponent<Shop> (); // TODO SHOP

		return myShop;
	}

	private IntPair GetLocation () {
		return GetShop ().worldToShopCoordinates (transform.position);
	}
}
