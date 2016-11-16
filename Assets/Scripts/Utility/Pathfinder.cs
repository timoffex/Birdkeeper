using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding {

	public delegate bool GridDelegate (int x, int y);

	/// <summary>
	/// Pre-conditions: start and end are within the grid, and grid is not null and not empty.
	/// </summary>
	/// <returns>An array of consecutive grid positions leading to end, OR null.</returns>
	/// <param name="grid">Grid.</param>
	/// <param name="start">Start position.</param>
	/// <param name="end">End position.</param>
	public static IntPair[] FindPath (bool[,] grid, IntPair start, IntPair end) {
		return FindPath ((x, y) => grid [x, y], grid.GetLength (0), grid.GetLength (1), start, end);
	}

	public static IntPair[] FindPath (GridDelegate grid, int gridSizeX, int gridSizeY, IntPair start, IntPair end) {

		AStar<IntPair>.NeighborDelegate neighborDelegate = node => astarNeighbors (node, grid, gridSizeX, gridSizeY);

		Func<IntPair, float> heuristic = node => (float) taxicab (node, end);

		return AStar<IntPair>.Solve (neighborDelegate, heuristic, start, end);
	}

	private static IEnumerable<AStar<IntPair>.EdgeType> astarNeighbors(IntPair node, GridDelegate grid, int maxX, int maxY) {
		foreach (IntPair neighbor in neighborsOf (node, maxX, maxY))
			if (!grid (neighbor.x, neighbor.y))
				yield return new AStar<IntPair>.EdgeType (node, neighbor, 1);
	}

	private static IEnumerable<IntPair> neighborsOf(IntPair center, int maxX, int maxY) {
		for (int offx = -1; offx <= 1; offx++) {
			for (int offy = -1; offy <= 1; offy++) {
				if ((offx != 0 || offy != 0) && (offx == 0 || offy == 0)) {
					var newX = center.x + offx;
					var newY = center.y + offy;

					if (newX < 0 || newX >= maxX || newY < 0 || newY >= maxY)
						continue;

					yield return new IntPair (newX, newY);
				}
			}
		}
	}

	private static int taxicab(IntPair i1, IntPair i2) {
		return Mathf.Abs (i1.x - i2.x) + Mathf.Abs (i1.y - i2.y);
	}
}
