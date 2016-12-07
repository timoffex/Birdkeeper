using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

[CustomEditor(typeof(Furniture))]
public class FurnitureEditor : Editor {
	
	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();


		Furniture furniture = target as Furniture;

		EditorGUILayout.LabelField ("Attracted Customers");


		var attractedCustomerList = furniture.GetAttractedCustomers ().ToList ();
		foreach (var kv in attractedCustomerList) {
			var customerPrefab = MetaInformation.Instance ().GetCustomerPrefabByID (kv.Key);
			var name = customerPrefab.name;
			var weight = kv.Value;


			EditorGUILayout.BeginHorizontal ();

			EditorGUI.BeginChangeCheck ();
			var newID = CustomerDisplayEditorUtility.CustomerDropdown (kv.Key);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (furniture, "Furniture Change Customer");

				furniture.RemoveAttractedCustomer (kv.Key);
				furniture.AddAttractedCustomer (newID, weight);
			}

			EditorGUI.BeginChangeCheck ();
			var newWeight = EditorGUILayout.FloatField (weight);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (furniture, "Furniture Change Customer Weight");
				EditorUtility.SetDirty (furniture);

				furniture.SetAttractedCustomerWeight (newID, newWeight);
			}

			if (GUILayout.Button ("-", GUILayout.ExpandWidth (false))) {
				Undo.RecordObject (furniture, "Furniture Remove Customer");
				EditorUtility.SetDirty (furniture);

				furniture.RemoveAttractedCustomer (newID);
			}

			EditorGUILayout.EndHorizontal ();
		}


		if (GUILayout.Button ("+", GUILayout.ExpandWidth (false))) {
			Undo.RecordObject (furniture, "Furniture Add Customer");
			EditorUtility.SetDirty (furniture);


			uint anyID = MetaInformation.Instance ().GetCustomerIDMappings ().FirstOrDefault ().Key;

			if (anyID == default(uint))
				EditorWindow.focusedWindow.ShowNotification (new GUIContent ("No registered customers!"));
			else
				furniture.AddAttractedCustomer (anyID, 1);
		}
	}


	public void OnSceneGUI () {
		Furniture f = (target as Furniture);

		Handles.color = Color.white;
		DoGridOffsetHandle (f);
		DoGridHandles (f);
	}

	private void DoGridOffsetHandle (Furniture f) {
		EditorGUI.BeginChangeCheck ();

		var pos = f.transform.position;
		var offs = Handles.FreeMoveHandle (pos + f.gridCornerOffset,
			           Quaternion.identity, 
			           0.1f * HandleUtility.GetHandleSize (pos + f.gridCornerOffset), Vector3.zero, Handles.DotCap);

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (f, "Furniture Change Grid Corner Offset");
			EditorUtility.SetDirty (f);
			f.gridCornerOffset = offs - pos;
		}
	}

	private void DoGridHandles (Furniture f) {
		Vector3 xvec = MetaInformation.Instance ().tileXVector / MetaInformation.Instance ().numGridSquaresPerTile;
		Vector3 yvec = MetaInformation.Instance ().tileYVector / MetaInformation.Instance ().numGridSquaresPerTile;

		var pos = f.transform.position + f.gridCornerOffset;

		var xExtent = f.gridX * xvec;
		var yExtent = f.gridY * yvec;



		// Draw lines to handles
		Handles.DrawLine (pos, pos + xExtent);
		Handles.DrawLine (pos, pos + yExtent);


		EditorGUI.BeginChangeCheck ();
		Vector3 newX = Handles.FreeMoveHandle (pos + xExtent, Quaternion.LookRotation (xvec),
			               0.1f * HandleUtility.GetHandleSize (pos + xExtent), Vector3.zero, Handles.DotCap);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (f, "Furniture Change Grid X");
			EditorUtility.SetDirty (f);

			f.MyGrid.gridSizeX = (int) Mathf.Round((newX - pos).magnitude / xvec.magnitude);
		}


		EditorGUI.BeginChangeCheck ();
		Vector3 newY = Handles.FreeMoveHandle (pos + yExtent, Quaternion.LookRotation (yvec),
			               0.1f * HandleUtility.GetHandleSize (pos + yExtent), Vector3.zero, Handles.DotCap);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (f, "Furniture Change Grid Y");
			EditorUtility.SetDirty (f);

			f.MyGrid.gridSizeY = (int) Mathf.Round((newY - pos).magnitude / yvec.magnitude);
		}




		// Draw grid
		for (int y = 0; y <= f.gridY; y++)
			Handles.DrawLine (pos + y * yvec, pos + y * yvec + f.gridX * xvec);
		for (int x = 0; x <= f.gridX; x++)
			Handles.DrawLine (pos + x * xvec, pos + x * xvec + f.gridY * yvec);
	}
}
