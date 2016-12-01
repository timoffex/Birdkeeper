using UnityEditor;
using UnityEngine;

using System;
using System.Linq;


[CustomEditor (typeof (CraftingWorkbench))]
public class CraftingWorkbenchEditor : Editor {
	public override void OnInspectorGUI () {
		CraftingWorkbench workbench = target as CraftingWorkbench;
		MetaInformation info = MetaInformation.Instance ();

		uint[] currentCraftableIDs = workbench.craftableItemIDs;



		bool madeChange = false;


		// Display current items
		for (int i = 0; i < currentCraftableIDs.Length; i++) {
			ItemType newItem;
			ItemType currentItem = info.GetItemTypeByID (currentCraftableIDs [i]);

			if (currentItem == null) {
				currentCraftableIDs [i] = 0;
				madeChange = true;
			} else if (ItemDisplayEditorUtility.DisplayEditableItemSelection (info, currentItem, out newItem)) {
				currentCraftableIDs [i] = newItem.ItemTypeID;
				madeChange = true;
			}
		}


		// Have an add button
		if (GUILayout.Button ("Add Craftable Item")) {
			uint[] newCraftableIDs = new uint[currentCraftableIDs.Length + 1];
			Array.Copy (currentCraftableIDs, newCraftableIDs, currentCraftableIDs.Length);

			var kv = info.GetItemTypeMappings ().FirstOrDefault ();

			if (kv.Key != default(uint)) // TODO probably wrong
				newCraftableIDs [newCraftableIDs.Length - 1] = kv.Key;
			else
				newCraftableIDs [newCraftableIDs.Length - 1] = 0;
			
			currentCraftableIDs = newCraftableIDs;

			madeChange = true;
		}


		// Remove all null entries and save
		if (madeChange) {
			uint[] finalCraftable = currentCraftableIDs.Where ((itemID) => itemID != 0).ToArray ();

			Undo.RecordObject (workbench, "CraftingWorkbench Changed Crafting Options");
			workbench.craftableItemIDs = finalCraftable;
		}

	}
}
