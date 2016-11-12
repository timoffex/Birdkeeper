using UnityEditor;
using UnityEngine;

public class GridEditor {

	public delegate void ChangeDelegateVec3 (Vector3 changedPosition);

	public static void DrawVec3Handle (Vector3 position, ChangeDelegateVec3 onChange) {
		EditorGUI.BeginChangeCheck ();

		var offs = Handles.FreeMoveHandle (position,
			Quaternion.identity, 
			0.1f * HandleUtility.GetHandleSize (position), Vector3.zero, Handles.DotCap);

		if (EditorGUI.EndChangeCheck ())
			onChange (offs);
	}
}
