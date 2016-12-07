

public static class IntPathFactory {
	
	public static IntPath MakePathFromArray (IntPair[] path) {
		return new ArrayPath (path);
	}

	public static IntPath MakeXLinePath (IntPair start, IntPair end) {
		return new XLinePath (start, end.x - start.x);
	}

	public static IntPath MakeYLinePath (IntPair start, IntPair end) {
		return new YLinePath (start, end.y - start.y);
	}

	public static IntPath MakeCornerPath (IntPair start, IntPair end) {
		return MakeCornerPathXFirst (start, end);
	}

	public static IntPath MakeCornerPathXFirst (IntPair start, IntPair end) {
		IntPair mid = new IntPair (end.x, start.y);

		return new XLinePath (start, mid.x - start.x) + new YLinePath (mid, end.y - mid.y);
	}

	public static IntPath MakeCornerPathYFirst (IntPair start, IntPair end) {
		IntPair mid = new IntPair (start.x, end.y);

		return new YLinePath (start, mid.y - start.y) + new XLinePath (mid, end.x - mid.x);
	}



	public static IntPath MakeCPath (IntPair start, IntPair mid, IntPair end) {
		bool dominatesX = (mid.x > start.x && mid.x > end.x) || (mid.x < start.x && mid.x < end.x);
		bool dominatesY = (mid.y > start.y && mid.y > end.y) || (mid.y < start.y && mid.y < end.y);

		if (dominatesX) {
			if (dominatesY)
				return MakeCornerPathXFirst (start, mid) + MakeCornerPathXFirst (mid, end);
			else
				return MakeCornerPathXFirst (start, mid) + MakeCornerPathYFirst (mid, end);
		} else {
			if (dominatesY)
				return MakeCornerPathYFirst (start, mid) + MakeCornerPathXFirst (mid, end);
			else
				return MakeCornerPathYFirst (start, mid) + MakeCornerPathYFirst (mid, end);
		}
	}



	public static IntPath AddPaths (IntPath p1, IntPath p2) {
		return new CombinedPath (p1, p2);
	}



	private class CombinedPath : IntPath {
		private IntPath path1;
		private IntPath path2;


		public override int Length {
			get {
				return path1.Length + path2.Length;
			}
		}


		public override IntPair this [int i] {
			get {
				if (i < path1.Length)
					return path1 [i];
				else
					return path2 [i - path1.Length];
			}
		}


		public CombinedPath (IntPath p1, IntPath p2) {
			path1 = p1;
			path2 = p2;
		}
	}

	private class XLinePath : IntPath {
		private IntPair startingPoint;
		private int pathLength;
		private int direction;

		public override int Length {
			get {
				return pathLength;
			}
		}

		public override IntPair this [int i] {
			get {
				return new IntPair (startingPoint.x + (i + 1) * direction, startingPoint.y);
			}
		}


		public XLinePath (IntPair start, int directionalLength) {
			startingPoint = start;
			pathLength = System.Math.Abs (directionalLength);
			direction = System.Math.Sign (directionalLength);
		}
	}

	private class YLinePath : IntPath {
		private IntPair startingPoint;
		private int pathLength;
		private int direction;

		public override int Length {
			get {
				return pathLength;
			}
		}

		public override IntPair this [int i] {
			get {
				return new IntPair (startingPoint.x, startingPoint.y + (i + 1) * direction);
			}
		}


		public YLinePath (IntPair start, int directionalLength) {
			startingPoint = start;
			pathLength = System.Math.Abs (directionalLength);
			direction = System.Math.Sign (directionalLength);
		}
	}

	private class ArrayPath : IntPath {
		private IntPair[] underlyingArray;

		public override int Length {
			get {
				return underlyingArray.Length;
			}
		}

		public override IntPair this [int i] {
			get {
				return underlyingArray [i];
			}
		}


		public ArrayPath (IntPair[] arr) {
			underlyingArray = arr;
		}
	}
}
