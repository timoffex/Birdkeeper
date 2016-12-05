using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Add to objects that should take up space in the game's grid.
/// </summary>
public class RectangularGridObject : MonoBehaviour, IGrid2DShape {
	
	private IntPair position;

	public int gridSizeX;
	public int gridSizeY;


	// Use this for initialization
	void Start () {
		if (position == null)
			position = new IntPair (0, 0);

		Game g = Game.current;
		if (g != null) {
			g.grid.RemoveShape (this);
			foreach (var pos in GenerateSpiral (position))
				if (g.grid.CanOccupyPosition (this, pos)) {
					position = pos;
					break;
				}
			g.grid.AddShape (this, position);
		}
	}

	void OnDestroy () {
		Game g = Game.current;
		if (g != null)
			g.grid.RemoveShape (this);
	}



	public IntPair[] FindShortestPathToNearby (Grid2D grid, IntPair start, IntPair end) {
		IntPair endPoint = null;
		grid.RemoveShape (this);
		foreach (var pos in GenerateSpiral (end)) {
			if (grid.CanOccupyPosition (this, pos)) {
				endPoint = pos;
				break;
			}
		}
		grid.AddShape (this, position);

		if (endPoint == null)
			return null;
		else
			return grid.FindShortestPathFor (this, start, endPoint);
	}

	private IEnumerable<IntPair> GenerateSpiral (IntPair center, int maxIter = 15) {
		int numToAdd = 1;
		int index = 0;

		IntPair pos = center;
		yield return pos;

		while (true) {
			if (index % 2 == 0)
				for (int i = 0; i < numToAdd; i++) {
					pos = new IntPair (pos.x + 1, pos.y);
					yield return pos;
				}
			else {
				for (int i = 0; i < numToAdd; i++) {
					pos = new IntPair (pos.x, pos.y + numToAdd);
					yield return pos;
				}
				numToAdd++;
			}

			index++;

			if (index >= maxIter)
				break;
		}
	}


	/// <summary>
	/// Returns true if the grid was not occupied at that spot and position could be
	/// set successfully, else returns false.
	/// </summary>
	public bool TrySetPosition (IntPair newPos) {
		Game g = Game.current;
		if (g != null) {

			g.grid.RemoveShape (this);
			if (g.grid.CanOccupyPosition (this, newPos)) {
				g.grid.MoveShape (this, newPos);
				position = newPos;
				return true;
			}
			g.grid.AddShape (this, position);

		}

		return false;
	}

	public IntPair GetPosition () {
		return position;
	}


	public bool DoesOccupySquare (IntPair offset) {
		return offset.x >= 0 && offset.x < gridSizeX
		    && offset.y >= 0 && offset.y < gridSizeY;
	}

	public bool DoesOccupyXEdge (IntPair offset) {
		return DoesOccupySquare (offset) && DoesOccupySquare (offset + new IntPair (1, 0));
	}

	public bool DoesOccupyYEdge (IntPair offset) {
		return DoesOccupySquare (offset) && DoesOccupySquare (offset + new IntPair (0, 1));
	}


	public IEnumerable<IntPair> GetOccupiedSquares () {
		for (int x = 0; x < gridSizeX; x++)
			for (int y = 0; y < gridSizeY; y++)
				yield return new IntPair (x, y);
	}

	public IEnumerable<IntPair> GetOccupiedXEdges () {
		for (int x = 0; x < gridSizeX-1; x++)
			for (int y = 0; y < gridSizeY; y++)
				yield return new IntPair (x, y);
	}

	public IEnumerable<IntPair> GetOccupiedYEdges () {
		for (int x = 0; x < gridSizeX; x++)
			for (int y = 0; y < gridSizeY-1; y++)
				yield return new IntPair (x, y);
	}
}
