using UnityEngine;
using System.Collections;

public class CraftButtonScript : MonoBehaviour {

	public RecipeDisplayer recipeDisplayer;

	
	public void Craft () {
		if (recipeDisplayer.CraftRecipe ())
			NotificationSystem.ShowNotificationIfPossible ("Crafted successfully!");
		else
			NotificationSystem.ShowNotificationIfPossible ("Could not craft the item :c");
	}
}
