using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour {

	public int numTilesX, numTilesY;

	// Number of grid tiles for every floor tile.
	public int numGridTilesPerFloorTile;

	// True in any occupied grid tile, False elsewhere.
	private bool[,] grid;




	// Use this for initialization
	void Start () {
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
}
