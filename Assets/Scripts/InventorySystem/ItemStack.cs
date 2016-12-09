using UnityEngine;

[System.Serializable]
public class ItemStack {
	[SerializeField] private uint itemTypeID;
	[SerializeField] private int count;


	public ItemType ItemType { get { return MetaInformation.Instance ().GetItemTypeByID (itemTypeID); } }
	public uint ItemTypeID { get { return itemTypeID; } }
	public int Count { get { return count; } }


	public ItemStack (ItemType type, int ct) {
		itemTypeID = type.ItemTypeID;
		count = ct;
	}

	public ItemStack (uint typeID, int ct) {
		itemTypeID = typeID;
		count = ct;
	}


	public bool ContainsType (ItemType type) {
		return ItemTypeID == type.ItemTypeID;
	}

	public bool SameTypeAs (ItemStack stack) {
		return ItemTypeID == stack.ItemTypeID;
	}


	public ItemStack IncrementCount () {
		return new ItemStack (ItemTypeID, Count + 1);
	}

	public ItemStack IncrementCount (int inc) {
		return new ItemStack (ItemTypeID, Count + inc);
	}
}
