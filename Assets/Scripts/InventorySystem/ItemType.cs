using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ItemType : ScriptableObject {
	[SerializeField] private uint itemTypeID;
	[SerializeField] private string itemName;
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

#if UNITY_EDITOR
	[MenuItem ("Assets/Create/Create New Item", priority = 0)]
	public static void CreateItem () {
		ItemType newItem = ScriptableObject.CreateInstance<ItemType> ();

		MetaInformation info = MetaInformation.Instance ();


		newItem.itemName = "New Item";
		newItem.itemTypeID = info.GetUnusedItemTypeID ();
		info.AddMappingForItemType (newItem.ItemTypeID, newItem);


		AssetDatabase.CreateAsset (newItem, "Assets/Items/New Item.asset");


		EditorGUIUtility.PingObject (newItem);
	}
#endif
}
