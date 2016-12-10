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



		if (Game.current.Phase == GamePhase.DayPhase) {
			var evtSys = ShopEventSystem.Instance ();

			if (evtSys == null)
				Debug.Log ("ShopEventSystem not found.");
			else {
				ShopEventSystem.Instance ().RegisterClickListener (myCollider, delegate {
					var craftingPanel = CraftingPanelScript.TryFindInstance ();


					if (craftingPanel != null) {
						craftingPanel.craftableItems = new List<ItemType> ();
						foreach (uint craftableID in craftableItemIDs) {
							ItemType myItem = info.GetItemTypeByID (craftableID);

							if (myItem != null)
								craftingPanel.craftableItems.Add (myItem);
							else
								Debug.LogErrorFormat ("Bad Item ID: {0}", craftableID);
						}

						craftingPanel.transform.SetAsLastSibling ();
						craftingPanel.gameObject.SetActive (true);
					} else
						Debug.LogError ("Could not find the CraftingPanelScript in scene; not displaying crafting screen.");
				});
			}
		}
	}
}
