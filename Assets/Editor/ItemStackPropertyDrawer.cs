using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;
using System.Linq;

[CustomPropertyDrawer (typeof (ItemStack))]
public class ItemStackPropertyDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty (position, label, property);

		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);



		Rect dropDownRect = new Rect (position.x, position.y, position.width-50, position.height);
		Rect countRect = new Rect (dropDownRect.xMax, position.y, 50, position.height);


		MetaInformation info = MetaInformation.Instance ();


		uint itemID = (uint)property.FindPropertyRelative ("itemTypeID").intValue;





		List<ItemType> allItems = info.GetItemTypeMappings ().Select ((kv) => kv.Value).Reverse ().ToList ();
		allItems.Add (null);
		allItems = ((IEnumerable<ItemType>)allItems).Reverse ().ToList ();

		string[] allItemNames = allItems.Select ((it) => it == null ? "None" : it.Name).ToArray ();

		int itemIndex;
		if (itemID == 0)
			itemIndex = 0;
		else
			itemIndex = allItems.FindIndex ((it) => it != null && it.ItemTypeID == itemID);



		EditorGUI.BeginChangeCheck ();
		int newItemIndex = EditorGUI.Popup (dropDownRect, itemIndex, allItemNames);
		if (EditorGUI.EndChangeCheck ()) {
			property.FindPropertyRelative ("itemTypeID").intValue = newItemIndex == 0 ? 0 : (int)allItems [newItemIndex].ItemTypeID;
			property.serializedObject.ApplyModifiedProperties ();
		}




		var oldLabelWidth = EditorGUIUtility.labelWidth;
		EditorGUI.BeginChangeCheck ();
		EditorGUIUtility.labelWidth = 0;
		int newCount = EditorGUI.IntField (countRect, property.FindPropertyRelative ("count").intValue);
		EditorGUIUtility.labelWidth = oldLabelWidth;
		if (EditorGUI.EndChangeCheck ()) {
			property.FindPropertyRelative ("count").intValue = newCount;
			property.serializedObject.ApplyModifiedProperties ();
		}






		EditorGUI.EndProperty ();
	}
}

