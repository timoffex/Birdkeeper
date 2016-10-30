using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RoomRenderer))]
public class Shop : MonoBehaviour {

	public int numTilesX, numTilesY;

	// Number of grid tiles for every floor tile.
	public int numGridTilesPerFloorTile;

	// True in any occupied grid tile, False elsewhere.
	private bool[,] grid;


	private RoomRenderer room;


	// Use this for initialization
	void Start () {
		room = GetComponent<RoomRenderer> ();
		grid = new bool[numTilesX, numTilesY];
	}

	public bool GetGrid (int x, int y) {
		return grid [x, y];
	}


	public bool CanPlaceFurniture (int xpos, int ypos, Furniture furniture) {
		for (int x = 0; x < furniture.gridX; x++)
			for (int y = 0; y < furniture.gridY; y++)
				if (GetGrid (xpos + x, ypos + y) && furniture.GetGrid (x, y))
					return false;

		return true;
	}

	public IntPair worldToShopCoordinates (Vector2 wrld) {
		Vector2 dif = wrld - (Vector2) transform.position;


		var xvec = room.generalTile.GetXVector () / numGridTilesPerFloorTile;
		var yvec = room.generalTile.GetYVector () / numGridTilesPerFloorTile;

		// a*XV + b*YV = dif
		// [XV | YV] * [a,b]' = dif
		// [a,b]' = [Xx Yx; Xy Yy]^-1 * dif
		// [a,b]' = [Yy -Yx; -Xy Xx] * dif / (XxYy - YxXy)

		var shopVec = new Vector2 (yvec.y * dif.x - yvec.x * dif.y, xvec.x * dif.y - xvec.y * dif.x)
		              / (xvec.x * yvec.y - yvec.x * xvec.y);
		
		return new IntPair (
			(int)Mathf.Floor (shopVec.x),
			(int)Mathf.Floor (shopVec.y));
	}

	/// <summary>
	/// Gives the coordinates of the center of the tile at (shopCoords.x, shopCoords.y)
	/// </summary>
	/// <returns>The world coordinates of the center of the tile.</returns>
	/// <param name="shopCoords">Shop coordinates.</param>
	public Vector2 shopToWorldCoordinates (IntPair shopCoords) {
		var xvec = room.generalTile.GetXVector () / numGridTilesPerFloorTile;
		var yvec = room.generalTile.GetYVector () / numGridTilesPerFloorTile;

		var dif3D = shopCoords.x * xvec + shopCoords.y * yvec;
		var newPos = transform.position + dif3D;

		return (Vector2) newPos;
	}
}
