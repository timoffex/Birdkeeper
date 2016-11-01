using UnityEngine;
using System.Collections;

public class Furniture : MonoBehaviour {

	public int gridX;
	public int gridY;
	private bool[,] objGrid;

	// Offset of far-away corner in objGrid.
	public Vector3 gridCornerOffset;


	public bool renderGrid = false;



	private RoomRenderer room;
	private Vector3 gridXVec;
	private Vector3 gridYVec;


	void Start () {
		room = GameObject.Find ("Room").GetComponent<RoomRenderer> ();
		float gridPerTile = room.GetComponent<Shop> ().numGridTilesPerFloorTile;
		gridXVec = room.generalTile.GetXVector () / gridPerTile;
		gridYVec = room.generalTile.GetYVector () / gridPerTile;

		objGrid = new bool [gridX, gridY];
		for (int x = 0; x < gridX; x++)
			for (int y = 0; y < gridY; y++)
				objGrid [x, y] = true;
	}


	/// <summary>
	/// Returns objGrid[x,y] considering furniture's rotation.
	/// </summary>
	/// <returns><c>true</c>, if grid was gotten, <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public bool GetGrid (int x, int y) {
		return objGrid [x, y];
	}


	public void OnRenderObject () {
		if (renderGrid) {
			Matrix4x4 localToWorld = 
				Matrix4x4.TRS (gridCornerOffset, Quaternion.identity, Vector3.one)
					* transform.localToWorldMatrix;

			GridRenderer.RenderGrid (objGrid,
				localToWorld,
				gridXVec,
				gridYVec, Color.white);
		}
	}
}
