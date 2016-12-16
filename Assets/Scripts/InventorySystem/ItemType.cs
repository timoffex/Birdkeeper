using UnityEngine;

[System.Serializable]
[CreateAssetMenu (menuName = "Create New Item", fileName = "New Item")]
public class ItemType : ScriptableObject {
	[SerializeField] private string itemName;
	[SerializeField] private uint itemTypeID;
	[SerializeField] private Sprite icon;
	[SerializeField] private ItemRecipe recipe;

	public string Name { get { return itemName; } }
	public uint ItemTypeID { get { return itemTypeID; } }
	public Sprite Icon { get { return icon; } }
	public ItemRecipe Recipe { get { return recipe; } }

	public override bool Equals (object obj) {
		return obj is ItemType && ((ItemType)obj).ItemTypeID == ItemTypeID;
	}

	public override int GetHashCode () {
		return (int)ItemTypeID;
	}



	public void SetIcon (Sprite spr) {
		icon = spr;
	}

	public void SetName (string nam) {
		itemName = nam;
	}

	public void SetRecipe (ItemRecipe recip) {
		recipe = recip;
	}

	public void SetID (uint id) {
		itemTypeID = id;
	}
}
