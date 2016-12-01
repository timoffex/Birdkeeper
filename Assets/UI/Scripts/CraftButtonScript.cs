using UnityEngine;
using System.Collections;

public class CraftButtonScript : MonoBehaviour {

	public RecipeDisplayer recipeDisplayer;

	
	public void Craft () {
		if (recipeDisplayer.CraftRecipe ())
			StartCoroutine (DialogSystem.Instance ().DisplayMessage ("Crafted successfully!"));
		else
			StartCoroutine (DialogSystem.Instance ().DisplayMessage ("Could not craft the item :c"));
	}
}
