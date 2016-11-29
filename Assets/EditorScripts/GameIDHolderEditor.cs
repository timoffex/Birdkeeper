using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor (typeof (GameIDHolder))]
public class GameIDHolderEditor : Editor {
	public override void OnInspectorGUI () {
		GameIDHolder scrpt = target as GameIDHolder;
		var id = scrpt.GameID;

		PropertyModification[] mods = PrefabUtility.GetPropertyModifications (scrpt);

		if (mods == null || (mods.Length > 0 && mods.Any ((p) => p.propertyPath.Equals ("myID"))))
			EditorGUILayout.LabelField (string.Format ("My type ID is {0}", id), EditorStyles.boldLabel);
		else
			EditorGUILayout.LabelField (string.Format ("My type ID is {0}", id));


		if (GUILayout.Button ("Remove Component")) {
			scrpt.RemoveMyIDMapping ();
			DestroyImmediate (scrpt);
		}
	}
}
