using UnityEngine;
using System.Collections;
using System.IO;

public class LoadGameScript : MonoBehaviour {

	public string saveFileName;

	public void LoadGame () {
		Game.current.Load (File.OpenRead (Path.Combine (Application.persistentDataPath, saveFileName)));
	}

}

