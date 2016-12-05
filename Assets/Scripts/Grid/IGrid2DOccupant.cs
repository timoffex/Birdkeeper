using System.Collections.Generic;

public interface IGrid2DOccupant {

	bool OccupiesSquare (IntPair pos);

	/// <summary>
	/// Assumes the vertices are adjacent (i.e. differ by exactly 1 in exactly 1 coordinate).
	/// </summary>
	bool OccupiesEdgeBetween (IntPair v1, IntPair v2);

	bool CanOccupyPosition (Grid2D grid, IntPair pos);


	IntPair GetPosition ();

	/// <summary>
	/// Generates the squares that would be occupied if the occupant was in the given position.
	/// </summary>
	IEnumerable<IntPair> GetOccupiedSquares (IntPair position);

	/// <summary>
	/// Generates the x-edges that would be occupied if the occupant was in the given position.
	/// An x-edge has the coordinates of the adjacent square with minimum x value.
	/// </summary>
	IEnumerable<IntPair> GetOccupiedXEdges (IntPair position);

	/// <summary>
	/// Generates the y-edges that would be occupied if the occupant was in the given position.
	/// A y-edge has the coordinates of the adjacent square with minimum y value.
	/// </summary>
	IEnumerable<IntPair> GetOccupiedYEdges (IntPair position);
}
