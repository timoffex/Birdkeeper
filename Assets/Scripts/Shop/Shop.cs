using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RoomRenderer))]
public class Shop : MonoBehaviour, IGrid2DShape {



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
		Game.current.grid.AddShape (this, new IntPair (0, 0));
	}

	/// <summary>
	/// Returns true if occupied at the given position, false if unoccupied.
	/// </summary>
	public bool GetGrid (int x, int y) {
		Game g = Game.current;

		return g != null && g.grid.IsSquareOccupied (new IntPair (x, y));
	}


	/// <summary>
	/// precondition: CanPlaceFurniture (xpos, ypos, furniture) == true
	/// </summary>
	/// <param name="xpos">Xpos.</param>
	/// <param name="ypos">Ypos.</param>
	/// <param name="furniture">Furniture.</param>
	public void PlaceFurniture (int xpos, int ypos, Furniture furniture) {
		Game g = Game.current;

		if (g != null)
			g.AddFurnitureToShop (furniture);
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

	public bool IsXEdgeEntryWay (IntPair v) {
		return v.x == numGridX - 1 && v.y >= 0 && v.y < 4;
	}

	public IntPair[] FindPath (IntPair start, IntPair end) {
		return Pathfinding.FindPath ((x,y) => GetGrid (x,y), numGridX, numGridY, start, end);
	}



	public bool DoesOccupySquare (IntPair offset) {
		return false; // shop doesn't occupy squares
	}

	public bool DoesOccupyXEdge (IntPair offset) {
		if (IsXEdgeEntryWay (offset))
			return false;
		else
			return (offset.x == numGridX - 1 && offset.y >= 0 && offset.y < numGridY)
				|| (offset.x == -1 && offset.y >= 0 && offset.y < numGridY);
	}

	public bool DoesOccupyYEdge (IntPair offset) {
		return (offset.x >= 0 && offset.x < numGridX && (offset.y == numGridY - 1 || offset.y == -1));
	}


	public IEnumerable<IntPair> GetOccupiedSquares () {
		yield break;
	}

	public IEnumerable<IntPair> GetOccupiedXEdges () {
		for (int y = 4; y < numGridY; y++) {
			yield return new IntPair (-1, y);
			yield return new IntPair (numGridX - 1, y);
		}
	}

	public IEnumerable<IntPair> GetOccupiedYEdges () {
		for (int x = 0; x < numGridX; x++) {
			yield return new IntPair (x, -1);
			yield return new IntPair (x, numGridY - 1);
		}
	}
}
