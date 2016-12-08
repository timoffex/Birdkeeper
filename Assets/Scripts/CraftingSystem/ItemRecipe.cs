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
		if (requiredItems.Length == 0)
			return false;
		
		return requiredItems.All ((stack) => inv.HasHowManyOf (stack.ItemType) >= stack.Count);
	}

	public bool CanBeCraftedGiven (List<ItemType> items) {
		if (requiredItems.Length == 0)
			return false;

		int[] remaining = new int[items.Count];
		for (int i = 0; i < remaining.Length; i++)
			remaining [i] = 1;

		foreach (var stack in requiredItems) {
			int ct = stack.Count;

			for (int i = 0; i < items.Count && ct > 0; i++) {
				if (remaining [i] > 0 && stack.ItemTypeID == items [i].ItemTypeID) {
					ct--;
					remaining [i]--;
				}
			}

			if (ct > 0)
				return false;
		}

		return true;
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
