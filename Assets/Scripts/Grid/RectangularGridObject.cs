using UnityEngine;
using System.Collections;


/// <summary>
/// Add to objects that should take up space in the game's grid.
/// </summary>
public class RectangularGridObject : MonoBehaviour, IGrid2DOccupant {
	
	private IntPair position;

	public int gridSizeX;
	public int gridSizeY;


	// Use this for initialization
	void Start () {
		if (position == null)
			position = new IntPair (0, 0);

		Game g = Game.current;
		if (g != null)
			g.grid.RegisterOccupant (this);
	}

	void OnDestroy () {
		Game g = Game.current;
		if (g != null)
			g.grid.UnregisterOccupant (this);
	}


	/// <summary>
	/// Returns true if the grid was not occupied at that spot and position could be
	/// set successfully, else returns false.
	/// </summary>
	public bool TrySetPosition (IntPair newPos) {
		Game g = Game.current;
		if (g != null && CanOccupyPosition (g.grid, newPos)) {
			position = newPos;
			return true;
		}

		return false;
	}

	public IntPair GetPosition () {
		return position;
	}


	public IntPair[] FindPath (IntPair start, IntPair end) {
		Game g = Game.current;
		if (g != null) {
			return g.grid.FindPathFor (this, start, end);
		} else
			return null;
	}


	public bool OccupiesSquare (IntPair pos) {
		return pos.x >= position.x && pos.x < position.x + gridSizeX
		    && pos.y >= position.y && pos.y < position.y + gridSizeY;
	}

	public bool OccupiesEdgeBetween (IntPair v1, IntPair v2) {
		return OccupiesSquare (v1) || OccupiesSquare (v2);
	}

	public bool CanOccupyPosition (Grid2D grid, IntPair pos) {
		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				if (grid.IsVertexOccupied (new IntPair (x + pos.x, y + pos.y), this))
					return false;

				if (x < gridSizeX - 1 && grid.IsEdgeOccupied (new IntPair (x + pos.x, y + pos.y), new IntPair (x + 1 + pos.x, y + pos.y), this))
					return false;

				if (y < gridSizeY - 1 && grid.IsEdgeOccupied (new IntPair (x + pos.x, y + pos.y), new IntPair (x + pos.x, y + 1 + pos.y), this))
					return false;
			}
		}

		return true;
	}
		
}
