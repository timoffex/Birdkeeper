using UnityEngine;

[System.Serializable]
public class ItemType {
	[SerializeField] private string name;
	[SerializeField] private uint itemTypeID;
	[SerializeField] private Sprite icon;

	public string Name { get { return name; } }
	public uint ItemTypeID { get { return itemTypeID; } }
	public Sprite Icon { get { return icon; } }



	public ItemType (string name, uint id, Sprite icon) {
		this.name = name;
		this.itemTypeID = id;
		this.icon = icon;
	}


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
		name = nam;
	}
}
