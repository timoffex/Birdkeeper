

public interface IGrid2DOccupant {

	bool OccupiesSquare (IntPair pos);

	/// <summary>
	/// Assumes the vertices are adjacent (i.e. differ by exactly 1 in exactly 1 coordinate).
	/// </summary>
	bool OccupiesEdgeBetween (IntPair v1, IntPair v2);

	bool CanOccupyPosition (Grid2D grid, IntPair pos);
}
