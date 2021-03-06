﻿using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor {
	public void OnSceneGUI () {
		Tile t = (target as Tile);


		var pos = t.transform.position;
		var xpos = pos + t.GetXVector ();
		var ypos = pos + t.GetYVector ();
		
		

		Handles.color = Color.white;
		Handles.DrawSolidDisc (pos, Vector3.forward, 0.1f * HandleUtility.GetHandleSize (pos));
		Handles.DrawLine (pos, xpos);
		Handles.DrawLine (pos, ypos);


		EditorGUI.BeginChangeCheck ();

		Vector3 xVec = Handles.FreeMoveHandle (xpos, Quaternion.identity,
			0.1f * HandleUtility.GetHandleSize (xpos),
			new Vector3 (0.5f, 0.5f, 0.5f), Handles.DotCap);

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (t, "Tile Change X Vector");
			EditorUtility.SetDirty (target);
			t.SetXVector (xVec - t.transform.position);
		}


		EditorGUI.BeginChangeCheck ();

		Vector3 yVec = Handles.FreeMoveHandle (ypos, Quaternion.identity,
			0.1f * HandleUtility.GetHandleSize (ypos),
			new Vector3 (0.5f, 0.5f, 0.5f), Handles.DotCap);

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (t, "Tile Change Y Vector");
			EditorUtility.SetDirty (target);
			t.SetYVector (yVec - t.transform.position);
		}
	}
}
