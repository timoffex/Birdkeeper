using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CraftingPanelScript : MonoBehaviour {

	public List<ItemType> craftableItems;


	public ItemStackDisplayer itemStackDisplayPrefab;
	public DraggedItem draggedItemPrefab;

	public RectTransform inventoryZone;



	public AudioClip craftingSuccessfulSound;
	public AudioClip craftingFailedSound;


	void OnEnable () {
		DisplayCraftingScreen ();
	}
		
	public void DisplayCraftingScreen () {
		RemakeInventory ();
	}




	public void CombineItems () {
		CraftingSlot[] slots = GetComponentsInChildren<CraftingSlot> ();

		List<ItemType> items = new List<ItemType> ();

		foreach (var slot in slots) {
			var item = slot.GetItem ();
			if (item != null) {
				items.Add (item);
				slot.Clear ();
			}
		}

		if (items.Any ((item) => !Game.current.inventory.HasItem (item))) {
			NotificationSystem.ShowNotificationIfPossible ("You do not have enough of at least one of the items.");
		} else {
			
			ItemType bestCandidate = null;
			int bestMatch = 0;


			// try to find a matching crafting recipe by looping through all known items and their recipes
			foreach (var kv in MetaInformation.Instance ().GetItemTypeMappings ()) {
				if (kv.Value.Recipe.CanBeCraftedGiven (items)) {

					int matchStrength = 0;
					bool[] matched = new bool[items.Count];

					foreach (var stack in kv.Value.Recipe.GetRequiredItems ()) {
						int numMatches = 0;
						for (int i = 0; i < items.Count && numMatches < stack.Count; i++) {
							if (!matched [i] && stack.ItemTypeID == items [i].ItemTypeID) {
								matched [i] = true;
								numMatches++;
							}
						}
					}

					matchStrength = matched.Select ((b) => b ? 1 : 0).Sum ();

					if (matchStrength > bestMatch) {
						bestCandidate = kv.Value;
						bestMatch = matchStrength;
					}
				}
			}

			if (bestCandidate == null) {
				NotificationSystem.ShowNotificationIfPossible ("Didn't match any recipes.");

				PlaySound (craftingFailedSound);
			} else {

				bestCandidate.Recipe.UseIngredientsFrom (Game.current.inventory);
				Game.current.inventory.AddItem (bestCandidate);

				NotificationSystem.ShowNotificationIfPossible (string.Format ("You made {0}", bestCandidate.Name));

				PlaySound (craftingSuccessfulSound);
			}
			
			RemakeInventory ();
		}
	}


	private void PlaySound (AudioClip sound) {
		AudioSource src = FindObjectOfType<AudioSource> ();

		if (src != null)
			src.PlayOneShot (sound);
	}



	private void RemakeInventory () {
		foreach (Transform child in inventoryZone)
			Destroy (child.gameObject);

		Game g = Game.current;

		if (g != null) {
			foreach (ItemStack stack in g.inventory.GetItemStacks ())
				AddInventoryStack (stack);
		} else
			Debug.Log ("Game.current is null! Not displaying inventory in crafting panel.");
	}

	private void AddInventoryStack (ItemStack stack) {
		ItemStackDisplayer stackDisplayer = GameObject.Instantiate (itemStackDisplayPrefab, inventoryZone) as ItemStackDisplayer;
		stackDisplayer.DisplayStack (stack);


		var onStartDrag = stackDisplayer.gameObject.AddComponent<CreateDraggedItemOnPointerDown> ();

		int count = stack.Count;

		onStartDrag.MakeFor (stack.ItemType, transform as RectTransform, draggedItemPrefab,
			() => {
				if (count > 0) {
					count--;
					stackDisplayer.DisplayStack (new ItemStack (stack.ItemTypeID, count));
					return true;
				} else
					return false;
			}, () => {
				count++;
				stackDisplayer.DisplayStack (new ItemStack (stack.ItemTypeID, count));
			});
	}




	public static CraftingPanelScript TryFindInstance () {
		var myCanvas = GameObject.FindObjectOfType<Canvas> ().gameObject;

		if (myCanvas != null)
			return FindComponentInChildrenOf<CraftingPanelScript> (myCanvas);

		return null;
	}

	private static T FindComponentInChildrenOf<T> (GameObject obj) where T : Component {
		var myComponent = obj.GetComponent<T> ();

		if (myComponent != null)
			return myComponent;
		else {
			foreach (Transform child in obj.transform) {
				myComponent = FindComponentInChildrenOf<T> (child.gameObject);

				if (myComponent != null)
					return myComponent;
			}
		}

		return null;
	}
}
