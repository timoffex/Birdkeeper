using System;
using System.Collections.Generic;

using UnityEngine;

public class ShopFurnitureGrid {
	private Furniture[,] obstructionGrid;
	private List<Furniture> containedFurniture;

	private int numGridX, numGridY;


	public ShopFurnitureGrid (int gridX, int gridY) {
		obstructionGrid = new Furniture[gridX, gridY];
		containedFurniture = new List<Furniture> ();

		numGridX = gridX;
		numGridY = gridY;
	}

	/// <summary>
	/// precondition: CanPlaceFurniture (xpos, ypos, furniture) == true
	/// </summary>
	/// <param name="xpos">Xpos.</param>
	/// <param name="ypos">Ypos.</param>
	/// <param name="furniture">Furniture.</param>
	public void PlaceFurniture (int xpos, int ypos, Furniture furniture) {
		containedFurniture.Add (furniture);
		Game.current.AddFurnitureToShop (furniture);

		for (int x = 0; x < furniture.gridX; x++)
			for (int y = 0; y < furniture.gridY; y++)
				if (furniture.GetGrid (x, y))
					obstructionGrid [xpos + x, ypos + y] = furniture;
	}


	public bool CanPlaceFurniture (int xpos, int ypos, Furniture furniture) {
		for (int x = 0; x < furniture.gridX; x++)
			for (int y = 0; y < furniture.gridY; y++)
				if (GetGrid (xpos + x, ypos + y) && furniture.GetGrid (x, y))
					return false;

		return true;
	}

	/// <summary>
	/// Returns true if occupied at the given position, false if unoccupied.
	/// </summary>
	public bool GetGrid (int x, int y) {
		if (!IsPositionInGrid (new IntPair (x, y)))
			return true;

		return obstructionGrid [x, y] != null;
	}

	public bool IsPositionInGrid (IntPair position) {
		return position.x >= 0 && position.x < numGridX && position.y >= 0 && position.y < numGridY;
	}


	public int GetFurnitureAmount () {
		return containedFurniture.Count;
	}

	public IEnumerable<Furniture> GetFurniture () {
		return containedFurniture;
	}

	public Furniture GetFurnitureAtIndex (int idx) {
		return containedFurniture [idx];
	}
}
