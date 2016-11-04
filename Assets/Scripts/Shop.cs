﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RoomRenderer))]
public class Shop : MonoBehaviour {

	public int numTilesX, numTilesY;

	// Number of grid tiles for every floor tile.
	public int numGridTilesPerFloorTile;

	// True in any occupied grid tile, False elsewhere.
	public bool[,] grid;


	private RoomRenderer room;


	// Use this for initialization
	void Start () {
		room = GetComponent<RoomRenderer> ();
		grid = new bool[numTilesX * numGridTilesPerFloorTile, numTilesY * numGridTilesPerFloorTile];
	}

	public bool GetGrid (int x, int y) {
		if (x < 0 || y < 0 || x >= numTilesX * numGridTilesPerFloorTile || y >= numTilesY * numGridTilesPerFloorTile)
			return true;
		return grid [x, y];
	}

	public void SetGrid (int x, int y, bool val) {
		grid [x, y] = val;
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
		for (int x = 0; x < furniture.gridX; x++)
			for (int y = 0; y < furniture.gridY; y++)
				if (furniture.GetGrid (x, y))
					SetGrid (xpos + x, ypos + y, true);
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
		return position.x >= 0 && position.x < grid.GetLength (0) && position.y >= 0 && position.y < grid.GetLength (1);
	}
}
