using UnityEngine;
using System;
using System.Runtime.Serialization;

using SavingLoading;


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




	public void Save () {
		ObjectGraph graph = ObjectGraph.CreateObjectGraph (this);

		graph.PrintDebug ();
	}
}