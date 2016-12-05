using UnityEngine;
using System;


[Serializable]
public class IntPair {

	[SerializeField] private int _x;
	[SerializeField] private int _y;

	public int x { get { return _x; } }
	public int y { get { return _y; } }

	public IntPair (int x, int y) {
		_x = x;
		_y = y;
	}

	public override bool Equals(object obj) {
		if (obj.GetType () != typeof(IntPair))
			return false;

		IntPair l = obj as IntPair;
		return x == l.x && y == l.y;
	}

	public override int GetHashCode() {
		return (int) (Mathf.Pow (2, x) * Mathf.Pow (3, y));
	}

	public override string ToString ()
	{
		return string.Format ("({0}, {1})", x, y);
	}


	public static IntPair operator - (IntPair p1, IntPair p2) {
		return new IntPair (p1.x - p2.x, p1.y - p2.y);
	}

	public static IntPair operator + (IntPair p1, IntPair p2) {
		return new IntPair (p1.x + p2.x, p1.y + p2.y);
	}
}
