using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RoomRenderer))]
public class Shop : MonoBehaviour, IGrid2DOccupant {



	public static Shop Instance () {
		return Game.current.shop;
	}
		
	public int numTilesX { get { return Game.current.shopSizeX; } }
	public int numTilesY { get { return Game.current.shopSizeY; } }


	public int numGridX {
		get { return numTilesX * MetaInformation.Instance ().numGridSquaresPerTile; }
	}

	public int numGridY {
		get { return numTilesY * MetaInformation.Instance ().numGridSquaresPerTile; }
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
		Game.current.grid.RegisterOccupant (this);
	}

	/// <summary>
	/// Returns true if occupied at the given position, false if unoccupied.
	/// </summary>
	public bool GetGrid (int x, int y) {
		Game g = Game.current;

		return g != null && g.grid.IsVertexOccupied (new IntPair (x, y));
	}


	/// <summary>
	/// precondition: CanPlaceFurniture (xpos, ypos, furniture) == true
	/// </summary>
	/// <param name="xpos">Xpos.</param>
	/// <param name="ypos">Ypos.</param>
	/// <param name="furniture">Furniture.</param>
	public void PlaceFurniture (int xpos, int ypos, Furniture furniture) {
		Game g = Game.current;

		if (g != null) {
			g.AddFurnitureToShop (furniture);
			g.grid.RegisterOccupant (furniture.MyGrid);
		}
	}


	public int GetFurnitureAmount () {
		Game g = Game.current;

		if (g != null)
			return g.furnitureInShop.Count;
		else
			return 0;
	}

	public Furniture GetFurnitureAtIndex (int idx) {
		Game g = Game.current;

		if (g != null && idx < g.furnitureInShop.Count) {
			return g.furnitureInShop [idx].furnitureRef;
		} else
			return null;
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


		var xvec = MetaInformation.Instance ().tileXVector / MetaInformation.Instance ().numGridSquaresPerTile;
		var yvec = MetaInformation.Instance ().tileYVector / MetaInformation.Instance ().numGridSquaresPerTile;

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
		var xvec = MetaInformation.Instance ().tileXVector / MetaInformation.Instance ().numGridSquaresPerTile;
		var yvec = MetaInformation.Instance ().tileYVector / MetaInformation.Instance ().numGridSquaresPerTile;

		var dif = shopCoords.x * xvec + shopCoords.y * yvec;
		var newPos = (Vector2)transform.position + dif;

		return newPos;
	}


	public bool IsPositionInGrid (IntPair position) {
		return position.x >= 0 && position.x < numGridX
			&& position.y >= 0 && position.y < numGridY;
	}

	public bool IsEntryWay (IntPair v1, IntPair v2) {
		if (IsOnInnerEdge (v1) && v1.x == numGridX - 1 && v1.y >= 0 && v1.y < 3)
			return IsOnOuterEdge (v2);
		else if (IsOnInnerEdge (v2) && v2.x == numGridX - 1 && v2.y >= 0 && v2.y < 3)
			return IsOnOuterEdge (v1);
		else
			return false;
	}
	private bool IsOnInnerEdge (IntPair v) {
		return (v.x == 0 && (v.y >= 0 || v.y < numGridY))
		|| (v.x == numGridX - 1 && (v.y >= 0 || v.y < numGridY))
		|| (v.y == 0 && (v.x >= 0 || v.x < numGridX))
		|| (v.y == numGridY - 1 && (v.x >= 0 || v.x < numGridX));
	}
	private bool IsOnOuterEdge (IntPair v) {
		return (v.x == -1 && (v.y >= 0 || v.y < numGridY))
			|| (v.x == numGridX && (v.y >= 0 || v.y < numGridY))
			|| (v.y == -1 && (v.x >= 0 || v.x < numGridX))
			|| (v.y == numGridY && (v.x >= 0 || v.x < numGridX));
	}

	public IntPair[] FindPath (IntPair start, IntPair end) {
		return Pathfinding.FindPath ((x,y) => GetGrid (x,y), numGridX, numGridY, start, end);
	}



	public bool OccupiesSquare (IntPair pos) {
		return false; // shop doesn't occupy squares
	}

	public bool OccupiesEdgeBetween (IntPair v1, IntPair v2) {
		return IsPositionInGrid (v1) != IsPositionInGrid (v2);
//		if (IsPositionInGrid (v1) != IsPositionInGrid (v2))
//			return !IsEntryWay (v1, v2);
//		else
//			return false;
	}

	public bool CanOccupyPosition (Grid2D grid, IntPair pos) {
		return pos.x == 0 && pos.y == 0; // shop can only be placed at (0,0)
	}
}
