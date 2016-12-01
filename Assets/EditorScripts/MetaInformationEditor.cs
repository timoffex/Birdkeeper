using UnityEditor;
using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(MetaInformation))]
public class MetaInformationEditor : Editor {


	public override void OnInspectorGUI () {
		MetaInformation info = target as MetaInformation;
		info.SetCurrent ();


		EditorGUI.BeginChangeCheck ();
		var newTileX = EditorGUILayout.Vector2Field ("Tile X Vector", info.tileXVector);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (info, "MetaInformation Change Tile X Vector");
			info.tileXVector = newTileX;
		}


		EditorGUI.BeginChangeCheck ();
		var newTileY = EditorGUILayout.Vector2Field ("Tile Y Vector", info.tileYVector);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (info, "MetaInformation Change Tile Y Vector");
			info.tileYVector = newTileY;
		}

		EditorGUI.BeginChangeCheck ();
		var newNumGridSquaresPerTile = EditorGUILayout.IntField ("# Grid Squares / Tile", info.numGridSquaresPerTile);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (info, "MetaInformation Change Num Grid Squares Per Tile");
			info.numGridSquaresPerTile = newNumGridSquaresPerTile;
		}


		GameObjectFieldFor (info.playerPrefab, "Player Prefab", (newPlayer) => {
			Undo.RecordObject (info, "MetaInformation Change Player Prefab");
			info.playerPrefab = newPlayer;
		});

		GameObjectFieldFor (info.roomPrefab, "Room Prefab", (newRoom) => {
			Undo.RecordObject (info, "MetaInformation Change Room Prefab");
			info.roomPrefab = newRoom;
		});

		GameObjectFieldFor (info.eventSystemPrefab, "Event System Prefab", (newES) => {
			Undo.RecordObject (info, "MetaInformation Change Event System Prefab");
			info.eventSystemPrefab = newES;
		});

		GameObjectFieldFor (info.shopEditorCanvasPrefab, "Shop Editor Canvas Prefab", (newES) => {
			Undo.RecordObject (info, "MetaInformation Change Shop Editor Canvas Prefab");
			info.shopEditorCanvasPrefab = newES;
		});

		GameObjectFieldFor (info.shopPhaseDayCanvasPrefab, "Shop Phase Day Canvas Prefab", (newES) => {
			Undo.RecordObject (info, "MetaInformation Change Shop Phase Day Canvas Prefab");
			info.shopPhaseDayCanvasPrefab = newES;
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


		foreach (var kv in info.GetItemTypeMappings ())
			DisplayItem (info, kv.Key, kv.Value);


		if (GUILayout.Button ("Create Item Type", GUILayout.ExpandWidth (false))) {
			uint id = info.GetUnusedItemTypeID ();
			var item = new ItemType ("New Item", id, null);
			info.AddMappingForItemType (id, item);
		}



		GUILayout.Space (5);
		GUILayout.Label ("Other ID Mappings");

		foreach (var kv in info.GetGeneralIDMappings ())
			GUILayout.Label (string.Format ("Prefab Name: {0}\t| ID: {1}", kv.Value.name, kv.Key));
	}



	private void DisplayItem (MetaInformation target, uint id, ItemType type) {
		MetaInformation info = target as MetaInformation;

		Rect fullItemRect = GUILayoutUtility.GetRect (0, 25, GUILayout.ExpandWidth (true));

		GUI.BeginGroup (fullItemRect);

		Rect iconRect = new Rect (0, 0, 25, 25);
		Rect nameRect = new Rect (25, 0, fullItemRect.width - 125, 25);
		Rect idRect = new Rect (iconRect.width + nameRect.width, 0, 100, 25);

		if (type.Icon != null) {
			Texture2D texture = type.Icon.texture;

			if (type.Icon.rect.width == type.Icon.texture.width)
				GUI.Box (iconRect, texture);
			else {
				float width = type.Icon.texture.width;
				float height = type.Icon.texture.height;

				Rect sourceRect = type.Icon.textureRect;
				Rect normalizedSourceRect = new Rect (sourceRect.x / width, sourceRect.y / height,
					                            sourceRect.width / width, sourceRect.height / height);

				Graphics.DrawTexture (iconRect, texture, normalizedSourceRect,
					(int)type.Icon.border [0], (int)type.Icon.border [1], (int)type.Icon.border [2], (int)type.Icon.border [3]);
			}
		} else
			GUI.Box (iconRect, (Texture2D)null);

		CheckForDragDrop<Sprite> (iconRect, false, (spr) => {
			Undo.RecordObject (info, string.Format ("MetaInformation Change Icon For Item {0}", id));
			type.SetIcon (spr);
		});


		EditorGUI.BeginChangeCheck ();
		var newName = GUI.TextField (nameRect, string.Format ("{0}", type.Name));
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (info, string.Format ("MetaInformation Change Item Name For Item {0}", id));
			type.SetName (newName);
		}

		GUI.Label (idRect, id.ToString ());


		GUI.EndGroup ();

		DisplayItemRecipe (target, type);
	}


	private void DisplayItemRecipe (MetaInformation target, ItemType type) {
		GUILayout.Label (string.Format ("Recipe for {0}", type.Name));

		ItemStack[] allRequiredItems = type.Recipe.GetRequiredItems ().ToArray ();
		bool recipeChanged = false;

		for (int i = 0; i < allRequiredItems.Length; i++) {
			ItemStack stack = allRequiredItems [i];
			ItemStack newStack;
			if (ItemDisplayEditorUtility.DisplayEditableItemStack (target, stack, out newStack)) {
				allRequiredItems [i] = newStack;
				recipeChanged = true;
			}
		}

		if (GUILayout.Button ("Add to Recipe")) {
			recipeChanged = true;

			ItemStack[] appendedRecipe = new ItemStack[allRequiredItems.Length + 1];
			for (int i = 0; i < allRequiredItems.Length; i++)
				appendedRecipe [i] = allRequiredItems [i];

			ItemType it = target.GetItemTypeMappings ().First ().Value;

			appendedRecipe [appendedRecipe.Length - 1] = new ItemStack (it, 0);
			allRequiredItems = appendedRecipe;
		}


		if (recipeChanged) {
			Undo.RecordObject (target, string.Format ("MetaInformation Changed Recipe For Item {0}", type.Name));
			type.SetRecipe (new ItemRecipe (allRequiredItems));
		}
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

		CheckForDragDrop<GameObject> (dropArea, true, process);
	}

	private void CheckForDragDrop<T> (Rect area, bool allowMultiples, Action<T> process) {
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
