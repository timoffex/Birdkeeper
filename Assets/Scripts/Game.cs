using UnityEngine;
using System;


[System.Serializable]
public class Game {

	private static Game _current;
	public static Game current {
		get {
			if (_current == null)
				_current = new Game ();

			return _current;
		}
	}

	public Shop shop;

}