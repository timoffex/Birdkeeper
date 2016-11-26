using UnityEngine;
using System.Collections;
using System.IO;

public class LoadGameScript : MonoBehaviour {

	public string pathToGame;

	public void LoadGame () {
		Game.current.Load (File.OpenRead (pathToGame));
	}

}

