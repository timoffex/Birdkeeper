using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BasicEditor))]
public class BasicEditor_Editor : Editor {
	public override void OnInspectorGUI () {
		var editor = target as BasicEditor;
		var items = editor.items ?? new GameObject [0];
		editor.items = items;


		var length = items.GetLength (0);


		EditorGUILayout.LabelField ("Drag Furniture here.");

		for (int i = 0; i < length; i++) {
			EditorGUI.BeginChangeCheck ();

			EditorGUILayout.BeginHorizontal ();
			var el = EditorGUILayout.ObjectField (items [i] as Object, typeof(GameObject), true) as GameObject;
			var rem = GUILayout.Button ("-");
			EditorGUILayout.EndHorizontal ();

			if (rem || el == null) {
				Undo.RecordObject (editor, "Basic Editor Remove Item");
				items [i] = null;
			} else {
				var draggable = el.GetComponent<IEditorDraggable> ();

				if (EditorGUI.EndChangeCheck () && draggable != null) {
					Undo.RecordObject (editor, "Basic Editor Change Item");
					items [i] = el;
				}
			}
		}


		EditorGUI.BeginChangeCheck ();

		var newElement = EditorGUILayout.ObjectField (null, typeof(GameObject), true) as GameObject;
		if (newElement != null) {
			var newDraggable = newElement.GetComponent<IEditorDraggable> ();

			if (EditorGUI.EndChangeCheck () && newDraggable != null) {
				Undo.RecordObject (editor, "Basic Editor Add Item");
				var newItems = new GameObject[length + 1];
				items.CopyTo (newItems, 0);
				newItems [length] = newElement;

				editor.items = newItems;
			}
		}
	}
}
