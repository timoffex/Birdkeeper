using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Inventory {

	private class ItemStackList : List<ItemStack> { }

	[SerializeField] private ItemStackList itemStacks = new ItemStackList ();


	public void AddItem (ItemType itemType) {
		ItemStack stack = itemStacks.Find ((st) => st.ItemType.Equals (itemType));

		if (stack != null)
			stack.IncrementCount ();
		else
			itemStacks.Add (new ItemStack (itemType, 1));
	}

	public void AddStack (ItemStack stack) {
		ItemStack existingStack = itemStacks.Find ((st) => st.ItemType.Equals (stack.ItemType));

		if (existingStack != null)
			stack.IncrementCount (stack.Count);
		else
			itemStacks.Add (stack);
	}

	public bool HasItem (ItemType item) {
		return HasHowManyOf (item) > 0;
	}

	public int HasHowManyOf (ItemType item) {
		ItemStack stack = itemStacks.Find ((st) => st.ItemType.Equals (item));

		if (stack != null)
			return stack.Count;
		else
			return 0;
	}


	public IEnumerable<ItemStack> GetItemStacks () {
		return itemStacks;
	}
}