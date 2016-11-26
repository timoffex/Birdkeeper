using UnityEditor;
using UnityEngine;

using System;
using System.Collections.Generic;

[CustomEditor(typeof(MetaInformation))]
public class MetaInformationEditor : Editor {


	public override void OnInspectorGUI () {
		MetaInformation info = target as MetaInformation;



		GameObjectFieldFor (info.playerPrefab, "Player Prefab", (newPlayer) => {
			Undo.RecordObject (info, "MetaInformation Change Player Prefab");
			info.playerPrefab = newPlayer;
		});

		GameObjectFieldFor (info.roomPrefab, "Room Prefab", (newRoom) => {
			Undo.RecordObject (info, "MetaInformation Change Room Prefab");
			info.roomPrefab = newRoom;
		});

		GameObjectFieldFor (info.shopEditorCanvasPrefab, "Shop Editor Canvas Prefab", (newES) => {
			Undo.RecordObject (info, "MetaInformation Change Shop Editor Canvas Prefab");
			info.shopEditorCanvasPrefab = newES;
		});

		GameObjectFieldFor (info.eventSystemPrefab, "Event System Prefab", (newES) => {
			Undo.RecordObject (info, "MetaInformation Change Event System Prefab");
			info.eventSystemPrefab = newES;
		});



		GUILayout.Label ("Furniture ID Mappings");

		foreach (var kv in info.GetFurnitureMappings ()) {
			GUILayout.Label (string.Format ("{0}: {1}", kv.Key, kv.Value));
		}


		DropArea ((obj) => {
			if (obj.GetComponent<Furniture> () != null) {
				if (!info.ContainsMappingForFurnitureNamed (obj.name)) {
					Undo.RecordObject (info, "MetaInformation Add Furniture Mapping");
					info.AddMappingForFurniture (info.GetUnusedFurnitureID (), obj);
				} else {
					Debug.Log (string.Format ("Already have a mapping for {0}", obj.name));
				}
			}
		});
	}

	private void GameObjectFieldFor (GameObject go, string label, Action<GameObject> setter) {
		EditorGUI.BeginChangeCheck ();
		var newGO = EditorGUILayout.ObjectField (label, go, typeof(GameObject), false) as GameObject;
		if (EditorGUI.EndChangeCheck ())
			setter (newGO);
	}

	private void DropArea (Action<GameObject> process) {
		Event evt = Event.current;
		Rect dropArea = GUILayoutUtility.GetRect (0, 50, GUILayout.ExpandWidth (true));
		GUI.Box (dropArea, "Drag & Drop Furniture Prefabs");

		switch (evt.type) {
		case EventType.DragUpdated:
		case EventType.DragPerform:
			if (dropArea.Contains (evt.mousePosition)) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (evt.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag ();

					foreach (UnityEngine.Object draggedObj in DragAndDrop.objectReferences) {
						if (draggedObj is GameObject)
							process (draggedObj as GameObject);
					}
				}
			}
			break;
		}
	}
}
