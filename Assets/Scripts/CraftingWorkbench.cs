using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Collider2D))]
public class CraftingWorkbench : MonoBehaviour {

	public uint[] craftableItemIDs;

	// Use this for initialization
	void Start () {
		var myCollider = GetComponent<Collider2D> ();
		var info = MetaInformation.Instance ();

		var evtSys = ShopEventSystem.Instance ();

		if (evtSys == null)
			Debug.LogError ("ShopEventSystem not found!");
		else {
			ShopEventSystem.Instance ().RegisterClickListener (myCollider, delegate {
				
				var craftingPanel = CraftingPanelScript.TryFindInstance ();

				craftingPanel.craftableItems = new List<ItemType> ();
				foreach (uint craftableID in craftableItemIDs)
					craftingPanel.craftableItems.Add (info.GetItemTypeByID (craftableID));

				craftingPanel.transform.SetAsLastSibling ();
				craftingPanel.gameObject.SetActive (true);

			});
		}
	}
}
