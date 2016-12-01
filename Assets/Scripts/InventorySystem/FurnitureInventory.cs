using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FurnitureInventory {

	private class FurnitureStackList : List<FurnitureStack> { }

	[SerializeField] private FurnitureStackList furnitureStacks;

	public FurnitureInventory () {
		furnitureStacks = new FurnitureStackList ();
	}


	public void Add (FurnitureStack stack) {
		FurnitureStack existing = FindStackMatching (stack);

		if (existing != null)
			existing.IncrementCount (stack.Count);
		else
			furnitureStacks.Add (stack);
	}

	public void SubtractOne (uint fid) {
		FurnitureStack existing = FindStackMatching (fid);

		if (existing != null) {
			existing.IncrementCount (-1);
			if (existing.Count <= 0)
				RemoveStacksWithID (existing.FurnitureID);
		} else
			Debug.LogError ("Trying to subtract from a stack that doesn't exist!");
	}


	public IEnumerable<FurnitureStack> GetFurnitureStacks () {
		for (int i = 0; i < furnitureStacks.Count; i++)
			yield return furnitureStacks [i];
	}


	private FurnitureStack FindStackMatching (FurnitureStack stack) {
		return FindStackMatching (stack.FurnitureID);
	}

	private FurnitureStack FindStackMatching (uint fid) {
		foreach (FurnitureStack existing in furnitureStacks)
			if (existing.FurnitureID == fid)
				return existing;

		return null;
	}

	private void RemoveStacksWithID (uint fid) {
		for (int i = 0; i < furnitureStacks.Count; i++)
			if (furnitureStacks [i].FurnitureID == fid)
				furnitureStacks.RemoveAt (i);
	}
}