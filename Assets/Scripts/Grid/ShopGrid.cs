using UnityEngine;
using System;
using System.Collections.Generic;

public class ShopGrid {

	private int shopSizeX;
	private int shopSizeY;

	private int entranceYMin;
	private int entranceYMax;


	private int[,] xClearance;
	private int[,] yClearance;
	private bool[,] occupied;

	private bool clearanceValid;



	public ShopGrid (int shopSizeX, int shopSizeY, int shopEntranceYMin, int shopEntranceYMax) {
		this.shopSizeX = shopSizeX;
		this.shopSizeY = shopSizeY;
		entranceYMin = shopEntranceYMin;
		entranceYMax = shopEntranceYMax;


		xClearance = new int[shopSizeX, shopSizeY];
		yClearance = new int[shopSizeX, shopSizeY];
		occupied = new bool[shopSizeX, shopSizeY];
		clearanceValid = false;
	}



	public void AddFurnitureRectangle (RectangularGridObject rect, IntPair pos) {
		AddFurnitureRectangle (new Rect (pos.x, pos.y, rect.gridSizeX, rect.gridSizeY));
	}

	public void AddFurnitureRectangle (Rect rectangle) {
		int minX = Mathf.Max (0, (int)rectangle.min.x);
		int minY = Mathf.Max (0, (int)rectangle.min.y);
		int maxX = Mathf.Min (shopSizeX - 1, (int)rectangle.max.x);
		int maxY = Mathf.Min (shopSizeY - 1, (int)rectangle.max.y);


		lock (occupied)
			for (int x = minX; x <= maxX; x++)
				for (int y = minY; y <= maxY; y++)
					occupied [x, y] = true;

		clearanceValid = false;
	}


	public bool GetGrid (int x, int y) {
		return occupied [x, y];
	}



	private void RecalculateClearance () {

		lock (occupied)
			for (int x = shopSizeX - 1; x >= 0; x--) {
				for (int y = shopSizeY - 1; y >= 0; y--) {

					if (occupied [x, y]) {
						xClearance [x, y] = 0;
						yClearance [x, y] = 0;
					} else {
						xClearance [x, y] = x < shopSizeX - 1 ? (xClearance [x + 1, y] + 1) : 1;
						yClearance [x, y] = y < shopSizeY - 1 ? (yClearance [x, y + 1] + 1) : 1;
					}

				}
			}
		
		clearanceValid = true;
	}


	private void GetNearestEntrancePosition (RectangularGridObject rect, IntPair pos, out IntPair outter, out IntPair inner) {
		int yMax = entranceYMax - rect.gridSizeY + 1;
		int yMin = entranceYMin;

		if (pos.y > yMax) {
			outter = new IntPair (shopSizeX, yMax);
			inner = new IntPair (shopSizeX - 1, yMax);
		} else if (pos.y < yMin) {
			outter = new IntPair (shopSizeX, yMin);
			inner = new IntPair (shopSizeX - 1, yMin);
		} else {
			outter = new IntPair (shopSizeX, pos.y);
			inner = new IntPair (shopSizeX - 1, pos.y);
		}
	}


	public bool IsOutsideShop (IntPair p) {
		return p.x < 0 || p.y < 0 || p.x >= shopSizeX || p.y >= shopSizeY;
	}

	public bool IsInsideShop (IntPair p) {
		return !IsOutsideShop (p);
	}


	public IntPath FindPath (RectangularGridObject rectangle, IntPair start, IntPair end) {
		if (IsOutsideShop (start)) {
			if (IsOutsideShop (end))
				return FindOutsidePath (rectangle, start, end);
			else
				return FindOutToInPath (rectangle, start, end);
		} else {
			if (IsInsideShop (end))
				return FindInsidePath (rectangle, start, end);
			else
				return FindInToOutPath (rectangle, start, end);
		}
	}

	private IntPath FindOutToInPath (RectangularGridObject rect, IntPair start, IntPair end) {
		IntPair outterEntrance;
		IntPair innerEntrance;

		GetNearestEntrancePosition (rect, start, out outterEntrance, out innerEntrance);

		return FindOutsidePath (rect, start, outterEntrance)
			+ IntPathFactory.MakeXLinePath (outterEntrance, innerEntrance)
			+ FindInsidePath (rect, innerEntrance, end);
	}

	private IntPath FindInToOutPath (RectangularGridObject rect, IntPair start, IntPair end) {
		IntPair outterEntrance;
		IntPair innerEntrance;

		GetNearestEntrancePosition (rect, start, out outterEntrance, out innerEntrance);

		return FindInsidePath (rect, start, innerEntrance)
			+ IntPathFactory.MakeXLinePath (innerEntrance, outterEntrance)
			+ FindOutsidePath (rect, outterEntrance, end);
	}


	private IntPath FindOutsidePath (RectangularGridObject rect, IntPair start, IntPair end) {

		int firstXAboveShop = shopSizeX;
		int firstXBelowShop = -rect.gridSizeX;
		int firstYAboveShop = shopSizeY;
		int firstYBelowShop = -rect.gridSizeY;


		if (end.x > firstXBelowShop && end.x < 0 && end.y < firstYAboveShop && end.y > firstYBelowShop)
			end = new IntPair (firstXBelowShop, end.y);

		if (end.y > firstYBelowShop && end.y < 0 && end.x < firstXAboveShop && end.x > firstXBelowShop)
			end = new IntPair (end.x, firstYBelowShop);


		Func<IntPair, bool> corner00 = (f) => f.x <= firstXBelowShop && f.y <= firstYBelowShop;
		Func<IntPair, bool> cornerX0 = (f) => f.x >= firstXAboveShop && f.y <= firstYBelowShop;
		Func<IntPair, bool> corner0Y = (f) => f.x <= firstXBelowShop && f.y >= firstYAboveShop;
		Func<IntPair, bool> cornerXY = (f) => f.x >= firstXAboveShop && f.y >= firstYAboveShop;

		Func<IntPair, bool> corner = (f) => corner00 (f) || cornerX0 (f) || corner0Y (f) || cornerXY (f);


		bool sameX = (start.x <= firstXBelowShop && end.x <= firstXBelowShop) || (start.x >= firstXAboveShop && end.x >= firstXAboveShop);
		bool sameY = (start.y <= firstYBelowShop && end.y <= firstYBelowShop) || (start.y >= firstYAboveShop && end.y >= firstYAboveShop);


		bool oppX = (start.x <= firstXBelowShop && end.x >= firstXAboveShop) || (start.x >= firstXAboveShop && end.x <= firstXBelowShop);
		bool oppY = (start.y <= firstYBelowShop && end.y >= firstYAboveShop) || (start.y >= firstYAboveShop && end.y <= firstYBelowShop);


		if (sameX || sameY)
			return IntPathFactory.MakeCornerPath (start, end);


		if (corner (start)) {
			if (corner (end))
				return IntPathFactory.MakeCornerPath (start, end);

			if (oppX)
				return IntPathFactory.MakeCornerPathXFirst (start, end);
			else
				return IntPathFactory.MakeCornerPathYFirst (start, end);
		}

		if (corner (end)) {
			if (oppX)
				return IntPathFactory.MakeCornerPathYFirst (start, end);
			else
				return IntPathFactory.MakeCornerPathXFirst (start, end);
		}

		if (oppX) {
			// go around shop on the Y axis
			int minYPath = Mathf.Abs (start.y - firstYBelowShop) + Mathf.Abs (end.y - firstYBelowShop);
			int maxYPath = Mathf.Abs (start.y - firstYAboveShop) + Mathf.Abs (end.y - firstYAboveShop);

			IntPair midY;
			if (minYPath < maxYPath)
				midY = new IntPair (start.x, firstYBelowShop);
			else
				midY = new IntPair (start.x, firstYAboveShop);
			
			return IntPathFactory.MakeCPath (start, midY, end);
		} else if (oppY) {

			// go around shop on the X axis
			int minXPath = Mathf.Abs (start.y - firstYBelowShop) + Mathf.Abs (end.y - firstYBelowShop);
			int maxXPath = Mathf.Abs (start.y - firstYAboveShop) + Mathf.Abs (end.y - firstYAboveShop);

			IntPair midX;
			if (minXPath < maxXPath)
				midX = new IntPair (firstXBelowShop, start.y);
			else
				midX = new IntPair (firstXAboveShop, start.y);

			return IntPathFactory.MakeCPath (start, midX, end);
		}

		if (start.x <= firstXBelowShop || start.x >= firstXAboveShop)
			return IntPathFactory.MakeCornerPathYFirst (start, end);
		else
			return IntPathFactory.MakeCornerPathXFirst (start, end);
	}


	private IntPath FindInsidePath (RectangularGridObject rect, IntPair start, IntPair end) {
		if (!clearanceValid)
			RecalculateClearance ();


		
		Func<IntPair, bool> isClear = (p) => xClearance [p.x, p.y] >= rect.gridSizeX && yClearance [p.x, p.y] >= rect.gridSizeY;

		IntPair[] path = null;
		lock (occupied) {
			path = AStar<IntPair>.Solve ((p) => {
				var p1 = new IntPair (p.x + 1, p.y);
				var p2 = new IntPair (p.x - 1, p.y);
				var p3 = new IntPair (p.x, p.y + 1);
				var p4 = new IntPair (p.x, p.y - 1);

				List<AStar<IntPair>.EdgeType> edges = new List<AStar<IntPair>.EdgeType> ();

				if (IsInsideShop (p1) && isClear (p1)) edges.Add (new AStar<IntPair>.EdgeType (p, p1, 1));
				if (IsInsideShop (p2) && isClear (p2)) edges.Add (new AStar<IntPair>.EdgeType (p, p2, 1));
				if (IsInsideShop (p3) && isClear (p3)) edges.Add (new AStar<IntPair>.EdgeType (p, p3, 1));
				if (IsInsideShop (p4) && isClear (p4)) edges.Add (new AStar<IntPair>.EdgeType (p, p4, 1));

				return edges;
			}, (p) => Mathf.Abs (p.x - end.x) + Mathf.Abs (p.y - end.y), start, end);
		}

		return IntPathFactory.MakePathFromArray (path);
	}
}

