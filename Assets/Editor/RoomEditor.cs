using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RoomRenderer))]
public class RoomEditor : Editor {



	[DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
	static void RenderCustomGizmo (RoomRenderer room, GizmoType gizmoType) {
		Shop shop = room.GetComponent<Shop> ();

		var pos = room.transform.position;



		int numGridPerTile = MetaInformation.Instance ().numGridSquaresPerTile;

		Handles.color = Color.red;
		var xvec = (Vector3)MetaInformation.Instance ().tileXVector / numGridPerTile;
		var yvec = (Vector3)MetaInformation.Instance ().tileYVector / numGridPerTile;

		// Draw Y lines
		for (int x = 0; x <= shop.numTilesX * numGridPerTile; x++)
			Handles.DrawLine (pos + x * xvec, pos + x * xvec + shop.numTilesY * yvec * numGridPerTile);

		// Draw X lines
		for (int y = 0; y <= shop.numTilesY * numGridPerTile; y++)
			Handles.DrawLine (pos + y * yvec, pos + y * yvec + shop.numTilesX * xvec * numGridPerTile);



		Handles.color = Color.green;
		xvec = MetaInformation.Instance ().tileXVector;
		yvec = MetaInformation.Instance ().tileYVector;

		// Draw Y lines
		for (int x = 0; x <= shop.numTilesX; x++)
			Handles.DrawLine (pos + x * xvec, pos + x * xvec + shop.numTilesY * yvec);

		// Draw X lines
		for (int y = 0; y <= shop.numTilesY; y++)
			Handles.DrawLine (pos + y * yvec, pos + y * yvec + shop.numTilesX * xvec);
	}
}
