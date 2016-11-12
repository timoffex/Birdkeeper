using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RoomRenderer))]
public class Shop : MonoBehaviour {


	public int numTilesX, numTilesY;

	// Number of grid tiles for every floor tile.
	public int numGridTilesPerFloorTile;

	public int numGridX {
		get { return numTilesX * numGridTilesPerFloorTile; }
	}

	public int numGridY {
		get { return numTilesY * numGridTilesPerFloorTile; }
	}


	// null in unoccupied tiles, Furniture reference in occupied tiles
	private Furniture[,] obstructionGrid;


	private List<Furniture> containedFurniture;

	private RoomRenderer __roomStored;
	private RoomRenderer room {
		get {
			if (__roomStored == null)
				__roomStored = GetComponent<RoomRenderer> ();
			return __roomStored;
		}
	}


	// Use this for initialization
	void Start () {
		containedFurniture = new List<Furniture> ();
		obstructionGrid = new Furniture[numGridX, numGridY];
	}

	public bool GetGrid (int x, int y) {
		if (!IsPositionInGrid (new IntPair (x, y)))
			return true;

		return obstructionGrid [x, y] != null;
	}

	public void SetGrid (int x, int y, Furniture val) {
		obstructionGrid [x, y] = val;
	}


	public bool CanPlaceFurniture (int xpos, int ypos, Furniture furniture) {
		for (int x = 0; x < furniture.gridX; x++)
			for (int y = 0; y < furniture.gridY; y++)
				if (GetGrid (xpos + x, ypos + y) && furniture.GetGrid (x, y))
					return false;

		return true;
	}

	/// <summary>
	/// precondition: CanPlaceFurniture (xpos, ypos, furniture) == true
	/// </summary>
	/// <param name="xpos">Xpos.</param>
	/// <param name="ypos">Ypos.</param>
	/// <param name="furniture">Furniture.</param>
	public void PlaceFurniture (int xpos, int ypos, Furniture furniture) {
		containedFurniture.Add (furniture);
		for (int x = 0; x < furniture.gridX; x++)
			for (int y = 0; y < furniture.gridY; y++)
				if (furniture.GetGrid (x, y))
					SetGrid (xpos + x, ypos + y, furniture);
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


	public bool IsPositionInGrid (IntPair position) {
		return position.x >= 0 && position.x < numGridX && position.y >= 0 && position.y < numGridY;
	}

	public IntPair[] FindPath (IntPair start, IntPair end) {
		return Pathfinding.FindPath ((x,y) => GetGrid (x,y), numGridX, numGridY, start, end);
	}
}
