using UnityEditor;
using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(MetaInformation))]
public class MetaInformationEditor : Editor {


	private Dictionary<uint, bool> showRecipeDict = new Dictionary<uint, bool>();


	public override void OnInspectorGUI () {
		MetaInformation info = target as MetaInformation;
		info.SetCurrent ();


		if (GUILayout.Button ("Save to default file"))
			info.Save ();

		if (GUILayout.Button ("Load from default file")) {
			info.Load ();
			EditorUtility.SetDirty (target);
		}


		EditorGUI.BeginChangeCheck ();
		var newTileX = EditorGUILayout.Vector2Field ("Tile X Vector", info.tileXVector);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (info, "MetaInformation Change Tile X Vector");
			EditorUtility.SetDirty (target);
			info.tileXVector = newTileX;
		}


		EditorGUI.BeginChangeCheck ();
		var newTileY = EditorGUILayout.Vector2Field ("Tile Y Vector", info.tileYVector);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (info, "MetaInformation Change Tile Y Vector");
			EditorUtility.SetDirty (target);
			info.tileYVector = newTileY;
		}

		EditorGUI.BeginChangeCheck ();
		var newNumGridSquaresPerTile = EditorGUILayout.IntField ("# Grid Squares / Tile", info.numGridSquaresPerTile);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (info, "MetaInformation Change Num Grid Squares Per Tile");
			EditorUtility.SetDirty (target);
			info.numGridSquaresPerTile = newNumGridSquaresPerTile;
		}


		GameObjectFieldFor (info.playerPrefab, "Player Prefab", (newPlayer) => {
			Undo.RecordObject (info, "MetaInformation Change Player Prefab");
			EditorUtility.SetDirty (target);
			info.playerPrefab = newPlayer;
		});

		GameObjectFieldFor (info.roomPrefab, "Room Prefab", (newRoom) => {
			Undo.RecordObject (info, "MetaInformation Change Room Prefab");
			EditorUtility.SetDirty (target);
			info.roomPrefab = newRoom;
		});

		GameObjectFieldFor (info.eventSystemPrefab, "Event System Prefab", (newES) => {
			Undo.RecordObject (info, "MetaInformation Change Event System Prefab");
			EditorUtility.SetDirty (target);
			info.eventSystemPrefab = newES;
		});

		GameObjectFieldFor (info.shopEditorCanvasPrefab, "Shop Editor Canvas Prefab", (newES) => {
			Undo.RecordObject (info, "MetaInformation Change Shop Editor Canvas Prefab");
			EditorUtility.SetDirty (target);
			info.shopEditorCanvasPrefab = newES;
		});

		GameObjectFieldFor (info.shopPhaseDayCanvasPrefab, "Shop Phase Day Canvas Prefab", (newES) => {
			Undo.RecordObject (info, "MetaInformation Change Shop Phase Day Canvas Prefab");
			EditorUtility.SetDirty (target);
			info.shopPhaseDayCanvasPrefab = newES;
		});

		GameObjectFieldFor (info.shopPhaseEditCanvasPrefab, "Shop Phase Edit Canvas Prefab", (newES) => {
			Undo.RecordObject (info, "MetaInformation Change Shop Phase Edit Canvas Prefab");
			EditorUtility.SetDirty (target);
			info.shopPhaseEditCanvasPrefab = newES;
		});





		GUILayout.Space (5);
		GUILayout.Label ("Known Furniture");

		var oldFurnitureMappings = info.GetFurnitureMappings ().ToList ();
		foreach (var kv in oldFurnitureMappings) {
			EditorGUI.BeginChangeCheck ();
			var newValue = EditorGUILayout.ObjectField (kv.Key.ToString (), kv.Value, typeof(GameObject), false) as GameObject;
			if (EditorGUI.EndChangeCheck () && newValue.GetComponent<Furniture> () != null) {
				Undo.RecordObject (info, "MetaInformation Change Furniture ID Mapping");
				EditorUtility.SetDirty (info);
				info.AddMappingForFurniture (kv.Key, newValue);
			}
		}

		EditorGUI.BeginChangeCheck ();
		var newFurniturePrefab = EditorGUILayout.ObjectField ("Add New Furniture", null, typeof(GameObject), false) as GameObject;
		if (EditorGUI.EndChangeCheck () && newFurniturePrefab.GetComponent<Furniture> () != null) {
			Undo.RecordObject (info, "MetaInformation Add Furniture ID Mapping");
			EditorUtility.SetDirty (info);
			info.AddMappingForFurniture (info.GetUnusedCustomerID (), newFurniturePrefab);
		}



		GUILayout.Space (5);
		GUILayout.Label ("Known Customers");

		var oldCustomerMappings = info.GetCustomerIDMappings ().ToList ();
		foreach (var kv in oldCustomerMappings) {
			EditorGUI.BeginChangeCheck ();
			var newValue = EditorGUILayout.ObjectField (kv.Key.ToString (), kv.Value, typeof(GameObject), false);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (info, "MetaInformation Change Customer ID Mapping");
				EditorUtility.SetDirty (info);
				info.AddMappingForCustomerPrefab (kv.Key, newValue as GameObject);
			}
		}

		EditorGUI.BeginChangeCheck ();
		var newCustomerPrefab = EditorGUILayout.ObjectField ("Add New Customer", null, typeof(GameObject), false);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (info, "MetaInformation Add Customer ID Mapping");
			EditorUtility.SetDirty (info);
			info.AddMappingForCustomerPrefab (info.GetUnusedCustomerID (), newCustomerPrefab as GameObject);
		}



		GUILayout.Space (5);
		GUILayout.Label ("Known Item Types");


		// Necessary because DisplayItem may modify item mappings
		var allItemMappings = info.GetItemTypeMappings ().ToList ();
		foreach (var kv in allItemMappings) {
			DisplayItem (info, kv.Key, kv.Value);
			GUILayout.Space (5);
		}

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
		bool showRecipe;
		if (!showRecipeDict.TryGetValue (id, out showRecipe))
			showRecipe = false;

		ItemType newItem;
		bool newShowRecipe;
		if (ItemDisplayEditorUtility.DisplayEditableItemType (target, type, showRecipe, out newItem, out newShowRecipe)) {
			if (newItem == null) {
				Undo.RecordObject (target, "MetaInformation Deleted Item");
				EditorUtility.SetDirty (target);
				target.AddMappingForItemType (id, null);
			} else {
				Undo.RecordObject (target, "MetaInformation Changed Item Type");
				EditorUtility.SetDirty (target);
				target.AddMappingForItemType (id, newItem);
			}
		}

		showRecipeDict [id] = newShowRecipe;
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


		MyEditorUtils.CheckForDragDrop<GameObject> (dropArea, true, process);
	}
}
