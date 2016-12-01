using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ItemRecipe {
	/// <summary>
	/// Every stack should be for a unique item type.
	/// </summary>
	[SerializeField] private ItemStack[] requiredItems;


	protected ItemRecipe () {
		requiredItems = new ItemStack [0];
	}


	public ItemRecipe (ItemStack[] items) {
		var list = new List<ItemStack> ();
		foreach (ItemStack stack in items)
			if (stack != null)
				list.Add (stack);
		requiredItems = list.ToArray ();
	}


	public bool CanBeCraftedGiven (Inventory inv) {
		return requiredItems.All ((stack) => inv.HasHowManyOf (stack.ItemType) >= stack.Count);
	}

	/// <summary>
	/// Subtracts the required ingredients from the inventory.
	/// Precondition: CanBeCraftedGiven (inv)
	/// </summary>
	/// <param name="inv">Inv.</param>
	public void UseIngredientsFrom (Inventory inv) {
		foreach (ItemStack ingredient in requiredItems)
			inv.SubtractStack (ingredient);
	}


	public IEnumerable<ItemStack> GetRequiredItems () {
		return requiredItems;
	}


	public static ItemRecipe NoRecipe () {
		return new ItemRecipe ();
	}
}
