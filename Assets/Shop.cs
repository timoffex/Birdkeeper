using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour {

	public int numTilesX, numTilesY;

	// Number of grid tiles for every floor tile.
	public int numGridTilesPerFloorTile;

	// True in any occupied grid tile, False elsewhere.
	public bool[,] grid;




	// Use this for initialization
	void Start () {
		grid = new bool[numTilesX, numTilesY];
	}



	public bool CanPlaceFurniture (int x, int y, Furniture furniture) {
		return false;
	}
}
