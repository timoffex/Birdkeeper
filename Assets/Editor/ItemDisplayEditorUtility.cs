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
			return true;
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


//	/// <summary>
//	/// Returns true on change. If newItem is null, that means the item was removed.
//	/// If a change was made and newItem isn't null, newItem.ItemTypeID == current.ItemTypeID.
//	/// </summary>
//	public static bool DisplayItemType (MetaInformation target, ItemType current, bool showRecipe,
//		out ItemType newItem, out bool newShowRecipe) {
//
//		// Set to true if any change was made.
//		bool madeChange = false;
//
//
//		bool deleted = false;
//		Sprite newIcon = current.Icon;
//		string newName = current.Name;
//		ItemRecipe newRecipe = current.Recipe;
//
//
//
//		// Reserve layout space for the item header.
//		Rect outlineRect = GUILayoutUtility.GetRect (0, 40, GUILayout.ExpandWidth (true));
//		Rect headerRect = new Rect (outlineRect.x + 2, outlineRect.y + 2, outlineRect.width - 4, 36);
//
//		#region Display Header
//		GUI.Box (outlineRect, "");
//		GUI.BeginGroup (headerRect);
//
//		// All of our rectangles
//		Rect iconRect = new Rect (0, 0, 36, 36);
//		Rect deleteItemRect = new Rect (headerRect.width - 20, (headerRect.height-20)/2, 20, 20);
//		Rect showHideRecipeRect = new Rect (deleteItemRect.x - 40, 0, 40, headerRect.height);
//		Rect nameRect = new Rect (iconRect.width, 0, showHideRecipeRect.x - iconRect.width, headerRect.height / 2);
//		Rect idRect = new Rect (iconRect.width, headerRect.height / 2, showHideRecipeRect.x - iconRect.width, headerRect.height / 2);
//
//
//		// Display editable icon
//		MyEditorUtils.DrawSprite (iconRect, current.Icon);
//		MyEditorUtils.CheckForDragDrop<Sprite> (iconRect, false, (spr) => {
//			madeChange = true;
//			newIcon = spr;
//		});
//
//	
//		// Display editable name
//		madeChange |= MyEditorUtils.TextField (nameRect, current.Name, out newName);
//
//		// Display ID, not editable
//		GUI.Label (idRect, current.ItemTypeID.ToString ());
//
//
//		// The Show/Hide Recipe button
//		if (GUI.Button (showHideRecipeRect, showRecipe ? "Hide" : "Show"))
//			newShowRecipe = !showRecipe;
//		else
//			newShowRecipe = showRecipe;
//
//		// The delete item button
//		deleted = GUI.Button (deleteItemRect, "-");
//		madeChange |= deleted;
//
//		GUI.EndGroup ();
//		#endregion
//
//
//		if (showRecipe) {
//			#region Display Recipe
//
//			GUILayout.BeginHorizontal ();
//			GUILayout.Space (45);
//
//			GUILayout.BeginVertical ();
//
//			ItemStack[] allRequiredItems = current.Recipe.GetRequiredItems ().ToArray ();
//			bool recipeChanged = false;
//
//			for (int i = 0; i < allRequiredItems.Length; i++) {
//				ItemStack stack = allRequiredItems [i];
//				ItemStack newStack;
//				if (ItemDisplayEditorUtility.DisplayEditableItemStack (target, stack, out newStack)) {
//					allRequiredItems [i] = newStack;
//					recipeChanged = true;
//				}
//			}
//
//			if (GUILayout.Button ("Add to Recipe", GUILayout.ExpandWidth (false))) {
//				recipeChanged = true;
//
//				ItemStack[] appendedRecipe = new ItemStack[allRequiredItems.Length + 1];
//				for (int i = 0; i < allRequiredItems.Length; i++)
//					appendedRecipe [i] = allRequiredItems [i];
//
//				ItemType it = target.GetItemTypeMappings ().First ().Value;
//
//				appendedRecipe [appendedRecipe.Length - 1] = new ItemStack (it, 0);
//				allRequiredItems = appendedRecipe;
//			}
//
//
//			if (recipeChanged) {
//				madeChange = true;
//				newRecipe = new ItemRecipe (allRequiredItems);
//			}
//
//			GUILayout.EndVertical ();
//			GUILayout.EndHorizontal ();
//
//			#endregion
//		}
//
//
//
//		if (madeChange) {
//			if (deleted)
//				newItem = null;
//			else {
//				// ID will not change.
//				newItem = new ItemType (newName, current.ItemTypeID, newIcon);
//				newItem.SetRecipe (newRecipe);
//			}
//
//			return true;
//		} else {
//			
//			newItem = current;
//			return false;
//		}
//	}
}

