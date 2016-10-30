using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoomRenderer))]
public class RoomEditor : Editor {

	void OnSceneGUI () {
		RoomRenderer room = (target as RoomRenderer);

		var pos = room.transform.position;

		var xvec = room.generalTile.xVector;
		var yvec = room.generalTile.yVector;


		Handles.color = Color.green;

		// Draw Y lines
		for (int x = 0; x <= room.roomSizeX; x++)
			Handles.DrawLine (pos + x * xvec, pos + x * xvec + room.roomSizeY * yvec);

		// Draw X lines
		for (int y = 0; y <= room.roomSizeY; y++)
			Handles.DrawLine (pos + y * yvec, pos + y * yvec + room.roomSizeX * xvec);
	}
}
