using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RecipeDisplayer : MonoBehaviour {

	public Image itemIcon;
	public Text itemName;
	public RectTransform ingredientsArea;

	public ItemStackDisplayer itemStackDisplayerPrefab;


	private ItemType myItem;
	private ItemRecipe myRecipe;


	public void DisplayRecipe (ItemType item, ItemRecipe recipe) {
		if (itemIcon != null) itemIcon.sprite = item.Icon;
		if (itemName != null) itemName.text = item.Name;

		myItem = item;
		myRecipe = recipe;


		// Clean ingredients area
		foreach (Transform child in ingredientsArea)
			Destroy (child.gameObject);


		foreach (ItemStack stack in recipe.GetRequiredItems ()) {
			var stackDisplay = GameObject.Instantiate (itemStackDisplayerPrefab, ingredientsArea) as ItemStackDisplayer;

			stackDisplay.DisplayStack (stack);
		}
	}


	public bool CraftRecipe () {
		if (myItem != null && myRecipe != null) {

			if (!myRecipe.CanBeCraftedGiven (Game.current.inventory))
				return false;
			else {
				myRecipe.UseIngredientsFrom (Game.current.inventory);
				Game.current.inventory.AddItem (myItem);
				return true;
			}

		} else {
			Debug.LogError ("Trying to craft null item or recipe.");
			return false;
		}
	}
}
