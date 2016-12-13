using UnityEngine;
using System;
using System.Collections.Generic;

using ObserverPattern;

[System.Serializable]
public class Inventory : IObservable<Inventory> {

	private class ItemStackList : List<ItemStack> { }

	[SerializeField] private ItemStackList itemStacks;


	[NonSerialized] private List<IObserver<Inventory>> observers;


	public Inventory () {
		observers = new List<IObserver<Inventory>> ();
		itemStacks = new ItemStackList ();
	}


	public void AddItem (ItemType itemType) {
		int idx = itemStacks.FindIndex ((st) => st.ItemType.Equals (itemType));

		if (idx >= 0)
			itemStacks [idx] = itemStacks [idx].IncrementCount ();
		else
			itemStacks.Add (new ItemStack (itemType, 1));

		foreach (var obs in observers)
			obs.OnNext (this);
	}



	public void SubtractStack (ItemStack stack) {
		int idx = itemStacks.FindIndex ((st) => st.ItemType.Equals (stack.ItemType));

		itemStacks [idx] = itemStacks [idx].IncrementCount (-stack.Count);

		if (itemStacks [idx].Count <= 0)
			itemStacks.RemoveAt (idx);

		foreach (var obs in observers)
			obs.OnNext (this);
	}

	public void AddStack (ItemStack stack) {
		int idx = itemStacks.FindIndex ((st) => st.SameTypeAs (stack));

		if (idx >= 0)
			itemStacks [idx] = itemStacks [idx].IncrementCount (stack.Count);
		else
			itemStacks.Add (stack);

		foreach (var obs in observers)
			obs.OnNext (this);
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



	public IDisposable Subscribe (IObserver<Inventory> observer) {
		observers.Add (observer);

		return new Unsubscriber<Inventory> (observers, observer);
	}


	private class Unsubscriber<T> : IDisposable {
		private List<IObserver<T>> observers;
		private IObserver<T> observer;

		public Unsubscriber (List<IObserver<T>> observers, IObserver<T> observer) {
			this.observers = observers;
			this.observer = observer;
		}

		public void Dispose () {
			observers.Remove (observer);
		}
	}
}