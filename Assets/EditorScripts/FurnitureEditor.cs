using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Furniture))]
public class FurnitureEditor : Editor {

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

			f.gridX = (int) Mathf.Round((newX - pos).magnitude / xvec.magnitude);
		}


		EditorGUI.BeginChangeCheck ();
		Vector3 newY = Handles.FreeMoveHandle (pos + yExtent, Quaternion.LookRotation (yvec),
			               0.1f * HandleUtility.GetHandleSize (pos + yExtent), Vector3.zero, Handles.DotCap);
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (f, "Furniture Change Grid Y");

			f.gridY = (int) Mathf.Round((newY - pos).magnitude / yvec.magnitude);
		}




		// Draw grid
		for (int y = 0; y <= f.gridY; y++)
			Handles.DrawLine (pos + y * yvec, pos + y * yvec + f.gridX * xvec);
		for (int x = 0; x <= f.gridX; x++)
			Handles.DrawLine (pos + x * xvec, pos + x * xvec + f.gridY * yvec);
	}
}
