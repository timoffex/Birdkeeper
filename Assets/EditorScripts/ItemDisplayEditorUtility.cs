using UnityEditor;
using UnityEngine;

using System;
using System.Linq;

public static class ItemDisplayEditorUtility {

	public static bool DisplayEditableItemStack (MetaInformation target, ItemStack stack, out ItemStack newStack) {
		ItemType itemType = target.GetItemTypeByID (stack.ItemTypeID);
		uint itemID = itemType.ItemTypeID;
		string itemName = itemType.Name;


		ItemType[] allItems = target.GetItemTypeMappings ().Select ((kv) => kv.Value).ToArray ();
		string[] allItemNames = allItems.Select ((item) => item.Name).ToArray ();

		int itemIndex = Array.FindIndex (allItems, (item) => item.ItemTypeID == itemID);

		if (itemIndex == -1) {
			Debug.LogErrorFormat ("Item not registered!");
			newStack = null;
			return false;
		}

		bool madeChange = false;
		newStack = stack;
		EditorGUILayout.BeginHorizontal ();

		EditorGUI.BeginChangeCheck ();
		int newItemIndex = EditorGUILayout.Popup (itemIndex, allItemNames);

		int newCount = EditorGUILayout.IntField (stack.Count);
		if (EditorGUI.EndChangeCheck ()) {
			newStack = new ItemStack (allItems [newItemIndex], newCount);
			madeChange = true;
		}

		if (GUILayout.Button ("-")) {
			newStack = null;
			madeChange = true;
		}

		EditorGUILayout.EndHorizontal ();


		return madeChange;
	}


	public static bool DisplayEditableItemSelection (MetaInformation target, ItemType current, out ItemType newItem) {
		uint itemID = current.ItemTypeID;
		string itemName = current.Name;

		newItem = null;

		ItemType[] allItems = target.GetItemTypeMappings ().Select ((kv) => kv.Value).ToArray ();
		string[] allItemNames = allItems.Select ((item) => item.Name).ToArray ();

		int itemIndex = Array.FindIndex (allItems, (item) => item.ItemTypeID == itemID);

		if (itemIndex == -1) {
			Debug.LogErrorFormat ("Item {0} is not registered!", itemName);
			newItem = null;
			return false;
		}


		bool madeChange = false;

		EditorGUILayout.BeginHorizontal ();


		EditorGUI.BeginChangeCheck ();
		int newItemIndex = EditorGUILayout.Popup (itemIndex, allItemNames);
		if (EditorGUI.EndChangeCheck ()) {
			newItem = allItems [newItemIndex];
			madeChange = true;
		}


		if (GUILayout.Button ("-")) {
			newItem = null;
			madeChange = true;
		}


		EditorGUILayout.EndHorizontal ();


		return madeChange;
	}

}

