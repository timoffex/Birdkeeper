using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ItemRecipe {
	/// <summary>
	/// Every stack should be for a unique item type.
	/// </summary>
	[SerializeField] private ItemStackList requiredItems;


	protected ItemRecipe () {
		requiredItems = new ItemStackList ();
	}


	public ItemRecipe (ItemStack[] items) {
		requiredItems = new ItemStackList ();
		foreach (ItemStack stack in items)
			if (stack != null)
				requiredItems.Add (stack);
	}


	public bool CanBeCraftedGiven (Inventory inv) {
		return requiredItems.All ((stack) => inv.HasHowManyOf (stack.ItemType) >= stack.Count);
	}

	public IEnumerable<ItemStack> GetRequiredItems () {
		return requiredItems;
	}


	[Serializable] public class ItemStackList : List<ItemStack> { }


	public static ItemRecipe NoRecipe () {
		return new ItemRecipe ();
	}
}
