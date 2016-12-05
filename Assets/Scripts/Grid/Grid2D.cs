using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Grid2D {
	private bool[,] squareOccupied;
	private bool[,] xEdgeOccupied;
	private bool[,] yEdgeOccupied;

	private Dictionary<IGrid2DShape, IntPair> occupantPositions;

	public int minX;
	public int maxX;
	public int minY;
	public int maxY;

	public IntPair min { get { return new IntPair (minX, minY); } }
	public IntPair max { get { return new IntPair (maxX, maxY); } }

	public int width { get { return maxX - minX + 1; } }
	public int height { get { return maxY - minY + 1; } }

	public Grid2D (int minX, int minY, int maxX, int maxY) {

		this.minX = minX;
		this.minY = minY;
		this.maxX = maxX;
		this.maxY = maxY;

		squareOccupied = new bool[width, height];
		xEdgeOccupied = new bool[width - 1, height];
		yEdgeOccupied = new bool[width, height - 1];
		occupantPositions = new Dictionary<IGrid2DShape, IntPair> ();

	}


	private IntPair GetPositionIndices (IntPair square) {
		return new IntPair (square.x - minX, square.y - minY);
	}



	private IEnumerable<IntPair> GetOccupiedSquareIndices (IGrid2DShape shape, IntPair pos) {
		return ToIndices (shape.GetOccupiedSquares (), pos);
	}

	private IEnumerable<IntPair> GetOccupiedXEdgeIndices (IGrid2DShape shape, IntPair pos) {
		return ToIndices (shape.GetOccupiedXEdges (), pos);
	}

	private IEnumerable<IntPair> GetOccupiedYEdgeIndices (IGrid2DShape shape, IntPair pos) {
		return ToIndices (shape.GetOccupiedYEdges (), pos);
	}



	private IEnumerable<IntPair> ToIndices (IEnumerable<IntPair> offsets, IntPair pos) {
		foreach (var offset in offsets)
			yield return GetPositionIndices (pos + offset);
	}


	private void OccupyArrays (IGrid2DShape shape, IntPair position) {
		lock (squareOccupied) {
			foreach (var indices in GetOccupiedSquareIndices (shape, position))
				if (IsSquareInRange (indices + min))
					squareOccupied [indices.x, indices.y] = true;

			foreach (var indices in GetOccupiedXEdgeIndices (shape, position))
				if (IsXEdgeInRange (indices + min))
					xEdgeOccupied [indices.x, indices.y] = true;

			foreach (var indices in GetOccupiedYEdgeIndices (shape, position))
				if (IsYEdgeInRange (indices + min))
					yEdgeOccupied [indices.x, indices.y] = true;
		}
	}


	private void RemoveArrays (IGrid2DShape shape, IntPair position) {
		IntPair shapeIndices = GetPositionIndices (position);

		lock (squareOccupied) {
			foreach (var indices in GetOccupiedSquareIndices (shape, position))
				if (IsSquareInRange (indices + min))
					squareOccupied [indices.x, indices.y] = occupantPositions.Any (
						(kv) => (kv.Key != shape) && (kv.Key.DoesOccupySquare (indices - GetPositionIndices (kv.Value))));

			foreach (var indices in GetOccupiedXEdgeIndices (shape, position))
				if (IsXEdgeInRange (indices + min))
					xEdgeOccupied [indices.x, indices.y] = occupantPositions.Any (
						(kv) => (kv.Key != shape) && (kv.Key.DoesOccupyXEdge (indices - GetPositionIndices (kv.Value))));

			foreach (var indices in GetOccupiedYEdgeIndices (shape, position))
				if (IsYEdgeInRange (indices + min))
					yEdgeOccupied [indices.x, indices.y] = occupantPositions.Any (
						(kv) => (kv.Key != shape) && (kv.Key.DoesOccupyYEdge (indices - GetPositionIndices (kv.Value))));
		}
	}

	




	public void AddShape (IGrid2DShape shape, IntPair position) {
		lock (squareOccupied) {
			occupantPositions [shape] = position;
			OccupyArrays (shape, position);
		}
	}

	public void RemoveShape (IGrid2DShape shape) {
		lock (squareOccupied) {
			if (occupantPositions.ContainsKey (shape)) {
				var position = occupantPositions [shape];
				occupantPositions.Remove (shape);
				RemoveArrays (shape, position);
			}
		}
	}


	public void MoveShape (IGrid2DShape shape, IntPair newPosition) {
		lock (squareOccupied) {
			if (occupantPositions.ContainsKey (shape)) {
				var oldPosition = occupantPositions [shape];
				RemoveArrays (shape, oldPosition);
			}


			OccupyArrays (shape, newPosition);
			occupantPositions [shape] = newPosition;
		}
	}



	public void Clear () {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				squareOccupied [x, y] = false;
				if (x < width - 1) xEdgeOccupied [x, y] = false;
				if (y < height - 1) yEdgeOccupied [x, y] = false;
			}
		}

		occupantPositions.Clear ();
	}



	public bool IsSquareInRange (IntPair v) {
		return v.x >= minX && v.x <= maxX && v.y >= minY && v.y <= maxY;
	}

	public bool IsXEdgeInRange (IntPair v) {
		return v.x >= minX && v.x < maxX && v.y >= minY && v.y <= maxY;
	}

	public bool IsYEdgeInRange (IntPair v) {
		return v.x >= minX && v.x <= maxX && v.y >= minY && v.y < maxY;
	}

	public bool IsSquareOccupied (IntPair v) {
		IntPair i = GetPositionIndices (v);
		return IsSquareInRange (v) ? squareOccupied [i.x, i.y] : true;
	}

	public bool IsXEdgeOccupied (IntPair v) {
		IntPair i = GetPositionIndices (v);
		return IsXEdgeInRange (v) ? xEdgeOccupied [i.x, i.y] : true;
	}

	public bool IsYEdgeOccupied (IntPair v) {
		IntPair i = GetPositionIndices (v);
		return IsYEdgeInRange (v) ? yEdgeOccupied [i.x, i.y] : true;
	}



	public bool CanOccupyPosition (IGrid2DShape shape, IntPair position) {

		foreach (var indices in ToIndices (shape.GetOccupiedSquares (), position)) {
			var pos = indices + min;

			if (!IsSquareInRange (pos) || squareOccupied [indices.x, indices.y])
				return false;
		}

		foreach (var indices in ToIndices (shape.GetOccupiedXEdges (), position)) {
			var pos = indices + min;

			if (!IsXEdgeInRange (pos) || xEdgeOccupied [indices.x, indices.y])
				return false;
		}

		foreach (var indices in ToIndices (shape.GetOccupiedYEdges (), position)) {
			var pos = indices + min;

			if (!IsYEdgeInRange (pos) || yEdgeOccupied [indices.x, indices.y])
				return false;
		}


		return true;
	}



	private IEnumerable<AStar<IntPair>.EdgeType> EnumerateNeighborsFor (IGrid2DShape shape, IntPair v) {
		IntPair v1 = new IntPair (v.x+1, v.y);
		IntPair v2 = new IntPair (v.x-1, v.y);
		IntPair v3 = new IntPair (v.x, v.y+1);
		IntPair v4 = new IntPair (v.x, v.y-1);

		IntPair i = GetPositionIndices (v);
		IntPair i1 = GetPositionIndices (v1);
		IntPair i2 = GetPositionIndices (v2);
		IntPair i3 = GetPositionIndices (v3);
		IntPair i4 = GetPositionIndices (v4);

		if (occupantPositions.ContainsKey (shape))
			RemoveArrays (shape, occupantPositions [shape]);
		bool canOccupy1 = CanOccupyPosition (shape, v1);
		bool canOccupy2 = CanOccupyPosition (shape, v2);
		bool canOccupy3 = CanOccupyPosition (shape, v3);
		bool canOccupy4 = CanOccupyPosition (shape, v4);

		bool isReachable1 = IsXEdgeInRange (v) && IsSquareInRange (v1) && !xEdgeOccupied [i.x, i.y] && !squareOccupied [i1.x, i1.y];
		bool isReachable2 = IsXEdgeInRange (v2) && IsSquareInRange (v2) && !xEdgeOccupied [i2.x, i2.y] && !squareOccupied [i2.x, i2.y];
		bool isReachable3 = IsYEdgeInRange (v) && IsSquareInRange (v3) && !yEdgeOccupied [i.x, i.y] && !squareOccupied [i3.x, i3.y];
		bool isReachable4 = IsYEdgeInRange (v4) && IsSquareInRange (v4) && !yEdgeOccupied [i4.x, i4.y] && !squareOccupied [i4.x, i4.y];
		if (occupantPositions.ContainsKey (shape))
			OccupyArrays (shape, occupantPositions [shape]);



		if (canOccupy1 && isReachable1) yield return new AStar<IntPair>.EdgeType (v, v1, 1);
		if (canOccupy2 && isReachable2) yield return new AStar<IntPair>.EdgeType (v, v2, 1);
		if (canOccupy3 && isReachable3) yield return new AStar<IntPair>.EdgeType (v, v3, 1);
		if (canOccupy4 && isReachable4) yield return new AStar<IntPair>.EdgeType (v, v4, 1);
	}

	private IEnumerable<AStar<IntPair>.EdgeType> MemoizedEnumerateNeighborsFor (IGrid2DShape shape, IntPair v, bool[,] isComputed, bool[,] canOccupy) {
		IntPair v1 = new IntPair (v.x+1, v.y);
		IntPair v2 = new IntPair (v.x-1, v.y);
		IntPair v3 = new IntPair (v.x, v.y+1);
		IntPair v4 = new IntPair (v.x, v.y-1);

		IntPair i = GetPositionIndices (v);
		IntPair i1 = GetPositionIndices (v1);
		IntPair i2 = GetPositionIndices (v2);
		IntPair i3 = GetPositionIndices (v3);
		IntPair i4 = GetPositionIndices (v4);

		if (occupantPositions.ContainsKey (shape))
			RemoveArrays (shape, occupantPositions [shape]);
		bool canOccupy1 = IsSquareInRange (v1);
		bool canOccupy2 = IsSquareInRange (v2);
		bool canOccupy3 = IsSquareInRange (v3);
		bool canOccupy4 = IsSquareInRange (v4);

		if (canOccupy1) {
			if (isComputed [i1.x, i1.y])
				canOccupy1 = canOccupy [i1.x, i1.y];
			else {
				canOccupy1 = canOccupy [i1.x, i1.y] = CanOccupyPosition (shape, v1);
				isComputed [i1.x, i1.y] = true;
			}
		}

		if (canOccupy2) {
			if (isComputed [i2.x, i2.y])
				canOccupy2 = canOccupy [i2.x, i2.y];
			else {
				canOccupy2 = canOccupy [i2.x, i2.y] = CanOccupyPosition (shape, v2);
				isComputed [i2.x, i2.y] = true;
			}
		}

		if (canOccupy3) {
			if (isComputed [i3.x, i3.y])
				canOccupy3 = canOccupy [i3.x, i3.y];
			else {
				canOccupy3 = canOccupy [i3.x, i3.y] = CanOccupyPosition (shape, v3);
				isComputed [i3.x, i3.y] = true;
			}
		}

		if (canOccupy4) {
			if (isComputed [i4.x, i4.y])
				canOccupy4 = canOccupy [i4.x, i4.y];
			else {
				canOccupy4 = canOccupy [i4.x, i4.y] = CanOccupyPosition (shape, v4);
				isComputed [i4.x, i4.y] = true;
			}
		}


		bool isReachable1 = IsXEdgeInRange (v) && !xEdgeOccupied [i.x, i.y];
		bool isReachable2 = IsXEdgeInRange (v2) && !xEdgeOccupied [i2.x, i2.y];
		bool isReachable3 = IsYEdgeInRange (v) && !yEdgeOccupied [i.x, i.y];
		bool isReachable4 = IsYEdgeInRange (v4) && !yEdgeOccupied [i4.x, i4.y];
		if (occupantPositions.ContainsKey (shape))
			OccupyArrays (shape, occupantPositions [shape]);



		if (canOccupy1 && isReachable1) yield return new AStar<IntPair>.EdgeType (v, v1, 1);
		if (canOccupy2 && isReachable2) yield return new AStar<IntPair>.EdgeType (v, v2, 1);
		if (canOccupy3 && isReachable3) yield return new AStar<IntPair>.EdgeType (v, v3, 1);
		if (canOccupy4 && isReachable4) yield return new AStar<IntPair>.EdgeType (v, v4, 1);
	}

	private AStar<IntPair>.NeighborDelegate NeighborFunction (IGrid2DShape shape) {
		bool[,] isComputed = new bool[width, height];
		bool[,] canOccupy = new bool[width, height];

		return (v) => MemoizedEnumerateNeighborsFor (shape, v, isComputed, canOccupy);
	}

	private float TaxicabDistance (IntPair v1, IntPair v2) {
		return Mathf.Abs (v1.x - v2.x) + Mathf.Abs (v1.y - v2.y);
	}

	/// <summary>
	/// Returns a path between the two positions that the occupant can traverse, if one exists.
	/// Else, returns null.
	/// </summary>
	public IntPair[] FindShortestPathFor (IGrid2DShape shape, IntPair start, IntPair end) {
		if (!CanOccupyPosition (shape, end))
			return null;

//		return null;
		lock (squareOccupied) {
			return AStar<IntPair>.Solve (NeighborFunction (shape), (v) => TaxicabDistance (v, end), start, end);
		}
	}
}
