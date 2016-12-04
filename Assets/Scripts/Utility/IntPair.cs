using UnityEngine;

public class IntPair {

	private int _x, _y;

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
}
