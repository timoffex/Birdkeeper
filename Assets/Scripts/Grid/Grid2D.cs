using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid2D {
	private List<IGrid2DOccupant> occupants;

	public int minX;
	public int maxX;
	public int minY;
	public int maxY;

	public Grid2D (int minX, int minY, int maxX, int maxY) {
		occupants = new List<IGrid2DOccupant> ();

		this.minX = minX;
		this.minY = minY;
		this.maxX = maxX;
		this.maxY = maxY;
	}

	private void ValidateOccupants () {
		occupants.RemoveAll ((occ) => occ == null);
	}



	public void RegisterOccupant (IGrid2DOccupant occupant) {
		occupants.Add (occupant);
	}

	public void UnregisterOccupant (IGrid2DOccupant occupant) {
		occupants.RemoveAll ((occ) => occ == occupant);
	}

	public void Clear () {
		occupants.Clear ();
	}

	/// <summary>
	/// Assumes v1 and v2 are adjacent.
	/// </summary>
	public bool IsEdgeOccupied (IntPair v1, IntPair v2, IGrid2DOccupant ignore = null) {
		ValidateOccupants ();
		if (!IsVertexInRange (v2))
			return true;

		if (occupants.Count == 0)
			return false;
		else
			return !occupants.TrueForAll ((occ) => !occ.OccupiesEdgeBetween (v1, v2) || occ == ignore);
	}

	public bool IsVertexOccupied (IntPair v, IGrid2DOccupant ignore = null) {
		ValidateOccupants ();
		if (!IsVertexInRange (v))
			return true;

		if (occupants.Count == 0)
			return false;
		else
			return !occupants.TrueForAll ((occ) => !occ.OccupiesSquare (v) || occ == ignore);
	}

	/// <summary>
	/// Assumes v1 and v2 are adjacent. Returns true if and only if
	/// v2 is not occupied and the edge between v and v2 is not occupied.
	/// </summary>
	public bool IsVertexReachableFrom (IntPair v, IntPair v2, IGrid2DOccupant ignore = null) {
		ValidateOccupants ();
		if (!IsVertexInRange (v2))
			return false;

		if (occupants.Count == 0)
			return true;
		else
			return occupants.TrueForAll ((occ) => (!occ.OccupiesSquare (v2) && !occ.OccupiesEdgeBetween (v, v2)) || occ == ignore);
	}

	public bool IsVertexInRange (IntPair v) {
		return v.x >= minX && v.x <= maxX && v.y >= minY && v.y <= maxY;
	}





	private IEnumerable<AStar<IntPair>.EdgeType> EnumerateNeighbors (IntPair v) {
		IntPair v1 = new IntPair (v.x+1, v.y);
		IntPair v2 = new IntPair (v.x-1, v.y);
		IntPair v3 = new IntPair (v.x, v.y+1);
		IntPair v4 = new IntPair (v.x, v.y-1);

		if (IsVertexReachableFrom (v, v1)) yield return new AStar<IntPair>.EdgeType (v, v1, 1);
		if (IsVertexReachableFrom (v, v2)) yield return new AStar<IntPair>.EdgeType (v, v2, 1);
		if (IsVertexReachableFrom (v, v3)) yield return new AStar<IntPair>.EdgeType (v, v3, 1);
		if (IsVertexReachableFrom (v, v4)) yield return new AStar<IntPair>.EdgeType (v, v4, 1);
	}

	private IEnumerable<AStar<IntPair>.EdgeType> EnumerateNeighborsFor (IGrid2DOccupant occ, IntPair v) {
		IntPair v1 = new IntPair (v.x+1, v.y);
		IntPair v2 = new IntPair (v.x-1, v.y);
		IntPair v3 = new IntPair (v.x, v.y+1);
		IntPair v4 = new IntPair (v.x, v.y-1);

		if (occ.CanOccupyPosition (this, v1) && IsVertexReachableFrom (v, v1, occ)) yield return new AStar<IntPair>.EdgeType (v, v1, 1);
		if (occ.CanOccupyPosition (this, v2) && IsVertexReachableFrom (v, v2, occ)) yield return new AStar<IntPair>.EdgeType (v, v2, 1);
		if (occ.CanOccupyPosition (this, v3) && IsVertexReachableFrom (v, v3, occ)) yield return new AStar<IntPair>.EdgeType (v, v3, 1);
		if (occ.CanOccupyPosition (this, v4) && IsVertexReachableFrom (v, v4, occ)) yield return new AStar<IntPair>.EdgeType (v, v4, 1);
	}

	private float TaxicabDistance (IntPair v1, IntPair v2) {
		return Mathf.Abs (v1.x - v2.x) + Mathf.Abs (v1.y - v2.y);
	}
		
	/// <summary>
	/// Returns a path between the two positions that the occupant can traverse, if one exists.
	/// Else, returns null.
	/// </summary>
	public IntPair[] FindPathFor (IGrid2DOccupant occ, IntPair start, IntPair end) {
		return AStar<IntPair>.Solve ((v) => EnumerateNeighborsFor (occ, v), (v) => TaxicabDistance (v, end), start, end);
	}
}
