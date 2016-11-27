using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RoomRenderer))]
public class Shop : MonoBehaviour {



	public static Shop Instance () {
		return Game.current.shop;
	}
		
	public int numTilesX { get { return Game.current.shopSizeX; } }
	public int numTilesY { get { return Game.current.shopSizeY; } }

	// Number of grid tiles for every floor tile.
	public int numGridTilesPerFloorTile;

	public int numGridX {
		get { return numTilesX * numGridTilesPerFloorTile; }
	}

	public int numGridY {
		get { return numTilesY * numGridTilesPerFloorTile; }
	}

	private ShopFurnitureGrid _furnitureGrid;
	private ShopFurnitureGrid furnitureGrid {
		get {
			if (_furnitureGrid == null)
				_furnitureGrid = new ShopFurnitureGrid (numGridX, numGridY);
			return _furnitureGrid;
		}

		set {
			_furnitureGrid = value;
		}
	}


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
		Game.current.shop = this;
	}

	/// <summary>
	/// Returns true if occupied at the given position, false if unoccupied.
	/// </summary>
	public bool GetGrid (int x, int y) {
		return furnitureGrid.GetGrid (x, y);
	}


	public bool CanPlaceFurniture (int xpos, int ypos, Furniture furniture) {
		return furnitureGrid.CanPlaceFurniture (xpos, ypos, furniture);
	}

	/// <summary>
	/// precondition: CanPlaceFurniture (xpos, ypos, furniture) == true
	/// </summary>
	/// <param name="xpos">Xpos.</param>
	/// <param name="ypos">Ypos.</param>
	/// <param name="furniture">Furniture.</param>
	public void PlaceFurniture (int xpos, int ypos, Furniture furniture) {
		furnitureGrid.PlaceFurniture (xpos, ypos, furniture);
	}


	public int GetFurnitureAmount () {
		return furnitureGrid.GetFurnitureAmount ();
	}

	public IEnumerable GetFurniture () {
		return furnitureGrid.GetFurniture ();
	}

	public Furniture GetFurnitureAtIndex (int idx) {
		return furnitureGrid.GetFurnitureAtIndex (idx);
	}

	public IntPair worldToShopCoordinates (Vector2 wrld) {
		// a*XV + b*YV = dif
		// [XV | YV] * [a,b]' = dif
		// [a,b]' = [Xx Yx; Xy Yy]^-1 * dif
		// [a,b]' = [Yy -Yx; -Xy Xx] * dif / (XxYy - YxXy)

		var shopVec = worldToShopCoordinatesFloat (wrld);
		
		return new IntPair (
			(int)Mathf.Floor (shopVec.x),
			(int)Mathf.Floor (shopVec.y));
	}

	public Vector2 worldToShopCoordinatesFloat (Vector2 wrld) {
		Vector2 dif = wrld - (Vector2) transform.position;


		var xvec = room.generalTile.GetXVector () / numGridTilesPerFloorTile;
		var yvec = room.generalTile.GetYVector () / numGridTilesPerFloorTile;

		// a*XV + b*YV = dif
		// [XV | YV] * [a,b]' = dif
		// [a,b]' = [Xx Yx; Xy Yy]^-1 * dif
		// [a,b]' = [Yy -Yx; -Xy Xx] * dif / (XxYy - YxXy)

		var shopVec = new Vector2 (yvec.y * dif.x - yvec.x * dif.y, xvec.x * dif.y - xvec.y * dif.x)
			/ (xvec.x * yvec.y - yvec.x * xvec.y);

		return shopVec;
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
		return furnitureGrid.IsPositionInGrid (position);
	}

	public IntPair[] FindPath (IntPair start, IntPair end) {
		return Pathfinding.FindPath ((x,y) => GetGrid (x,y), numGridX, numGridY, start, end);
	}
}
