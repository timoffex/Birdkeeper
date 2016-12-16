using UnityEngine;

[System.Serializable]
public class ItemStack {
	[SerializeField] private ItemType itemType;
	[SerializeField] private int count;


	public ItemType ItemType { get { return itemType; } }
	public uint ItemTypeID { get { return itemType.ItemTypeID; } }
	public int Count { get { return count; } }


	public ItemStack (ItemType type, int ct) {
		itemType = type;
		count = ct;
	}

	public ItemStack (uint typeID, int ct) {
		itemType = MetaInformation.Instance ().GetItemTypeByID (typeID);
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
