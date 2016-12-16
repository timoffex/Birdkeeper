using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;
using System.Linq;

//[CustomPropertyDrawer (typeof (ItemStack))]
public class ItemStackPropertyDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty (position, label, property);

		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);


		Rect dropDownRect = new Rect (position.x, position.y, position.width-50, position.height);
		Rect countRect = new Rect (dropDownRect.xMax, position.y, 50, position.height);


		SerializedProperty myItemProp = property.FindPropertyRelative ("itemType");
		SerializedProperty myCountProp = property.FindPropertyRelative ("count");


		ItemType myItem = (ItemType)myItemProp.objectReferenceValue;
		int myCount = myCountProp.intValue;

		MetaInformation info = MetaInformation.Instance ();


		var allItems = info.GetItemTypeMappings ().Select ((kv) => kv.Value).ToList ();
		var allItemNames = allItems.Select ((item) => item.Name).ToArray ();

		int myIndex = allItemNames.ToList ().IndexOf (myItem.Name);


		EditorGUI.BeginChangeCheck ();
		int newIndex = EditorGUI.Popup (dropDownRect, myIndex, allItemNames);
		if (EditorGUI.EndChangeCheck ()) {
			
		}


		EditorGUI.EndProperty ();
	}
}

