using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShopMoverGrid))]
public class ShopMoverGrid_Editor : Editor {


	public void OnSceneGUI () {
		ShopMoverGrid f = (target as ShopMoverGrid);

		Handles.color = Color.white;
		DoGridOffsetHandle (f);
		DoGridHandles (f);
	}

	private void DoGridOffsetHandle (ShopMoverGrid f) {

		GridEditor.DrawVec3Handle (f.transform.position + (Vector3)f.gridOffset, delegate (Vector3 offs) {
			Undo.RecordObject (f, "ShopMoverGrid Change Grid Corner Offset");
			f.gridOffset = offs - f.transform.position;
		});

	}

	private void DoGridHandles (ShopMoverGrid f) {
		RoomRenderer roomRenderer = GameObject.Find ("Room").GetComponent<RoomRenderer> ();
		Shop shop = roomRenderer.GetComponent<Shop> ();
		Vector3 xvec = roomRenderer.generalTile.GetXVector () / shop.numGridTilesPerFloorTile;
		Vector3 yvec = roomRenderer.generalTile.GetYVector () / shop.numGridTilesPerFloorTile;

		var pos = f.transform.position + (Vector3) f.gridOffset;

		var xExtent = f.gridWidth * xvec;
		var yExtent = f.gridHeight * yvec;



		// Draw lines to handles
		Handles.DrawLine (pos, pos + xExtent);
		Handles.DrawLine (pos, pos + yExtent);


		GridEditor.DrawVec3Handle (pos + xExtent + yExtent, delegate (Vector3 newPos) {
			Undo.RecordObject (f, "Furniture Change Grid X");


			IntPair offset = shop.worldToShopCoordinates (newPos - pos + shop.transform.position);
			f.gridWidth = offset.x - 1;
			f.gridHeight = offset.y - 1;
		});



		// Draw grid
		for (int y = 0; y <= f.gridHeight; y++)
			Handles.DrawLine (pos + y * yvec, pos + y * yvec + f.gridWidth * xvec);
		for (int x = 0; x <= f.gridWidth; x++)
			Handles.DrawLine (pos + x * xvec, pos + x * xvec + f.gridHeight * yvec);
	}
}
