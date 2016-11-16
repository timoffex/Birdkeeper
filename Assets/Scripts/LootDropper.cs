using UnityEngine;
using System.Collections;

public class LootDropper : MonoBehaviour {

	public Transform droppedPrefab;

	/// <summary>
	/// Helper function to drop the given droppedPrefab.
	/// </summary>
	public virtual void DropItem () {
		IDroppable dropIface = droppedPrefab.GetComponent<IDroppable> ();


		if (dropIface != null) {
			// If droppedPrefab has component IDroppable, use its DropAt () function.
			dropIface.DropAt (Shop.Instance (), GetLocation ());
		} else {
			// Otherwise, just use a default dropping animation.

			var clone = GameObject.Instantiate (droppedPrefab, transform.parent) as GameObject;

			clone.transform.position = transform.position;

		}
	}

	private IntPair GetLocation () {
		return Shop.Instance ().worldToShopCoordinates (transform.position);
	}
}
