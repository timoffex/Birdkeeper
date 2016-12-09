using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Inventory {

	private class ItemStackList : List<ItemStack> { }

	[SerializeField] private ItemStackList itemStacks;

	public Inventory () {
		itemStacks = new ItemStackList ();
	}


	public void AddItem (ItemType itemType) {
		int idx = itemStacks.FindIndex ((st) => st.ItemType.Equals (itemType));

		if (idx >= 0)
			itemStacks [idx] = itemStacks [idx].IncrementCount ();
		else
			itemStacks.Add (new ItemStack (itemType, 1));
	}



	public void SubtractStack (ItemStack stack) {
		int idx = itemStacks.FindIndex ((st) => st.ItemType.Equals (stack.ItemType));

		itemStacks [idx] = itemStacks [idx].IncrementCount (-stack.Count);

		if (itemStacks [idx].Count <= 0)
			itemStacks.RemoveAt (idx);
	}

	public void AddStack (ItemStack stack) {
		int idx = itemStacks.FindIndex ((st) => st.SameTypeAs (stack));

		if (idx >= 0)
			itemStacks [idx] = itemStacks [idx].IncrementCount (stack.Count);
		else
			itemStacks.Add (stack);
	}


	public bool HasItem (ItemType item) {
		return HasHowManyOf (item) > 0;
	}

	public int HasHowManyOf (ItemType item) {
		ItemStack stack = itemStacks.Find ((st) => st.ContainsType (item));

		if (stack != null)
			return stack.Count;
		else
			return 0;
	}


	public bool HasStack (ItemStack stack) {
		return HasHowManyOf (stack.ItemType) >= stack.Count;
	}



	/// <summary>
	/// Each stack is guaranteed to be of a unique type.
	/// </summary>
	/// <returns>The item stacks.</returns>
	public IEnumerable<ItemStack> GetItemStacks () {
		for (int i = 0; i < itemStacks.Count; i++)
			yield return itemStacks[i];
	}
}