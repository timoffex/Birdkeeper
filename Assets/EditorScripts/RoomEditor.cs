using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoomRenderer))]
public class RoomEditor : Editor {



	[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
	static void RenderCustomGizmo (RoomRenderer room, GizmoType gizmoType) {
		Shop shop = room.GetComponent<Shop> ();

		var pos = room.transform.position;





		Handles.color = Color.red;
		var xvec = room.generalTile.GetXVector () / shop.numGridTilesPerFloorTile;
		var yvec = room.generalTile.GetYVector () / shop.numGridTilesPerFloorTile;

		// Draw Y lines
		for (int x = 0; x <= shop.numTilesX * shop.numGridTilesPerFloorTile; x++)
			Handles.DrawLine (pos + x * xvec, pos + x * xvec + shop.numTilesY * yvec * shop.numGridTilesPerFloorTile);

		// Draw X lines
		for (int y = 0; y <= shop.numTilesY * shop.numGridTilesPerFloorTile; y++)
			Handles.DrawLine (pos + y * yvec, pos + y * yvec + shop.numTilesX * xvec * shop.numGridTilesPerFloorTile);



		Handles.color = Color.green;
		xvec = room.generalTile.GetXVector ();
		yvec = room.generalTile.GetYVector ();

		// Draw Y lines
		for (int x = 0; x <= shop.numTilesX; x++)
			Handles.DrawLine (pos + x * xvec, pos + x * xvec + shop.numTilesY * yvec);

		// Draw X lines
		for (int y = 0; y <= shop.numTilesY; y++)
			Handles.DrawLine (pos + y * yvec, pos + y * yvec + shop.numTilesX * xvec);
	}
}
