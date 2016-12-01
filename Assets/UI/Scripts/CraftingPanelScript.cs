using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CraftingPanelScript : MonoBehaviour {

	public List<ItemType> craftableItems;


	public ItemDisplayer itemTypeDisplayPrefab;
	public RecipeDisplayer recipeDisplayPrefab;


	public RectTransform optionsZone;
	public RectTransform recipeZone;


	void OnEnable () {
		DisplayCraftingOptions ();
	}


	private void Clean () {
		optionsZone.gameObject.SetActive (false);
		recipeZone.gameObject.SetActive (false);

		foreach (Transform child in optionsZone) Destroy (child.gameObject);
		foreach (Transform child in recipeZone) {
			Debug.LogFormat ("Destroying {0}", child.gameObject.name);
			Destroy (child.gameObject);
		}
	}
		
	public void DisplayCraftingOptions () {
		Clean ();
		optionsZone.gameObject.SetActive (true);

		foreach (ItemType item in craftableItems) {
			var itemDisplay = GameObject.Instantiate (itemTypeDisplayPrefab, optionsZone) as ItemDisplayer;
			itemDisplay.DisplayItem (item);

			var action = itemDisplay.gameObject.AddComponent<DoSomethingOnClick> ();
			action.clickDelegate = GetDisplayRecipeDelegate (item);
		}
	}

	public void DisplayRecipeForItem (ItemType item, ItemRecipe recipe) {
		Clean ();
		recipeZone.gameObject.SetActive (true);

		var recipeDisplay = GameObject.Instantiate (recipeDisplayPrefab, recipeZone) as RecipeDisplayer;
		recipeDisplay.DisplayRecipe (item, recipe);
	}


	private DoSomethingOnClick.ClickDelegate GetDisplayRecipeDelegate (ItemType item) {
		return delegate {
			DisplayRecipeForItem (item, item.Recipe);
		};
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
