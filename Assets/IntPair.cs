using UnityEngine;

public class IntPair {
	public int x, y;

	public IntPair (int x, int y) {
		this.x = x;
		this.y = y;
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
}
