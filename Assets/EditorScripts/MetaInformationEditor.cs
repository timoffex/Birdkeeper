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



		GUILayout.Space (5);
		GUILayout.Label ("Known Furniture");

		foreach (var kv in info.GetFurnitureMappings ()) {
			GUILayout.Label (string.Format ("{0}: {1}", kv.Value, kv.Key));
		}


		GameObjectDropArea ((obj) => {
			if (obj.GetComponent<Furniture> () != null) {
				if (!info.ContainsMappingForFurnitureNamed (obj.name)) {
					Undo.RecordObject (info, "MetaInformation Add Furniture Mapping");
					info.AddMappingForFurniture (info.GetUnusedFurnitureID (), obj);
				} else {
					Debug.Log (string.Format ("Already have a mapping for {0}", obj.name));
				}
			}
		});


		GUILayout.Space (5);
		GUILayout.Label ("Known Item Types");


		foreach (var kv in info.GetItemTypeMappings ()) {

			Rect fullItemRect = GUILayoutUtility.GetRect (0, 25, GUILayout.ExpandWidth (true));

			GUI.BeginGroup (fullItemRect);

			Rect iconRect = new Rect (0, 0, 25, 25);
			Rect nameRect = new Rect (25, 0, fullItemRect.width - 25, 25);


			Texture2D texture = null;
			if (kv.Value.Icon != null)
				texture = TextureFromSprite (kv.Value.Icon);
			
			GUI.Box (iconRect, texture);
			CheckForDrag<Sprite> (iconRect, false, (spr) => {
				Undo.RecordObject (info, string.Format ("MetaInformation Change Icon For Item {0}", kv.Key));
				kv.Value.SetIcon (spr);
			});


			EditorGUI.BeginChangeCheck ();
			var newName = GUI.TextField (nameRect, string.Format ("{0}", kv.Value.Name, kv.Key));
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (info, string.Format ("MetaInformation Change Item Name For Item {0}", kv.Key));
				kv.Value.SetName (newName);
			}


			GUI.EndGroup ();
		}

		if (GUILayout.Button ("Create Item Type", GUILayout.ExpandWidth (false))) {
			uint id = info.GetUnusedItemTypeID ();
			var item = new ItemType ("New Item", id, null);
			info.AddMappingForItemType (id, item);
		}
	}


	private static Texture2D TextureFromSprite (Sprite sp) {
		if (sp.rect.width != sp.texture.width) {
			Texture2D newText = new Texture2D((int)sp.rect.width,(int)sp.rect.height);
			Color[] newColors = sp.texture.GetPixels((int)sp.textureRect.x, 
				(int)sp.textureRect.y, 
				(int)sp.textureRect.width, 
				(int)sp.textureRect.height );
			newText.SetPixels(newColors);
			newText.Apply();
			return newText;
		} else
			return sp.texture;
	}

	private void GameObjectFieldFor (GameObject go, string label, Action<GameObject> setter) {
		EditorGUI.BeginChangeCheck ();
		var newGO = EditorGUILayout.ObjectField (label, go, typeof(GameObject), false) as GameObject;
		if (EditorGUI.EndChangeCheck ())
			setter (newGO);
	}

	private void GameObjectDropArea (Action<GameObject> process) {
		Event evt = Event.current;
		Rect dropArea = GUILayoutUtility.GetRect (0, 50, GUILayout.ExpandWidth (true));
		GUI.Box (dropArea, "Drag & Drop Furniture Prefabs");

		CheckForDrag<GameObject> (dropArea, true, process);
	}

	private void CheckForDrag<T> (Rect area, bool allowMultiples, Action<T> process) {
		Event evt = Event.current;

		switch (evt.type) {
		case EventType.DragUpdated:
		case EventType.DragPerform:
			if (area.Contains (evt.mousePosition)) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (evt.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag ();

					foreach (object draggedObj in DragAndDrop.objectReferences) {
						if (draggedObj is T) {
							process ((T)draggedObj);

							if (!allowMultiples)
								break;
						} else {
							string errorText = string.Format ("Please enter an object of type {0}", typeof(T).Name);
							Debug.Log (errorText);
							EditorWindow.focusedWindow.ShowNotification (new GUIContent (errorText));
						}
					}
				}
			}
			break;
		}
	}
}
