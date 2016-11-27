using UnityEngine;

[System.Serializable]
public class ItemStack {
	[SerializeField] private ItemType itemType;
	[SerializeField] private int count;


	public ItemType ItemType { get { return itemType; } }
	public int Count { get { return count; } }


	public ItemStack (ItemType type, int ct) {
		itemType = type;
		count = ct;
	}


	public void IncrementCount () {
		count++;
	}

	public void IncrementCount (int inc) {
		count += inc;
	}
}
