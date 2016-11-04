using UnityEngine;
using System.Collections;

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
		float[,] minDist = new float[gridSizeX, gridSizeY];
		IntPair[,] previous = new IntPair[gridSizeX, gridSizeY];
	
		for (int i = 0; i < gridSizeX; i++)
			for (int j = 0; j < gridSizeY; j++)
				minDist [i, j] = float.PositiveInfinity;



		PriorityQueue queue = new PriorityQueue ();

		previous [start.x, start.y] = null;
		minDist [start.x, start.y] = 0;
	

		foreach (IntPair neighbor in neighborsOf (start, gridSizeX, gridSizeY)) {
			var p = 1 + taxicab (neighbor, end);
			queue.Enqueue (neighbor, -p);

			minDist [neighbor.x, neighbor.y] = 1;
			previous [neighbor.x, neighbor.y] = start;
		}


		while (queue.GetSize () > 0) {
			var el = queue.Dequeue () as IntPair;

			if (el == end) {
				break;
			}

			if (!grid (el.x, el.y)) {
				// If not occupied..

				// For every neighbor....
				foreach (IntPair neighbor in neighborsOf (el, gridSizeX, gridSizeY)) {
					var newDist = minDist [el.x, el.y] + 1;

					// only if newDist is less than previous minimum distance
					if (newDist < minDist [neighbor.x, neighbor.y]) {
						var p = newDist + taxicab (neighbor, end);
						queue.Enqueue (neighbor, -p);

						minDist [neighbor.x, neighbor.y] = newDist;
						previous [neighbor.x, neighbor.y] = el;
					}
				}
			}
		}

		IntPair last = end;
		if (previous [last.x, last.y] == null) {
			return null;
		}


		IntPair[] path = new IntPair[(int) minDist [last.x, last.y]];

		for (int i = (int)minDist [last.x, last.y] - 1; i >= 0; i--) {
			path [i] = last;
			last = previous [last.x, last.y];
		}

		return path;
	}


	private static IEnumerable neighborsOf(IntPair center, int maxX, int maxY) {
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
