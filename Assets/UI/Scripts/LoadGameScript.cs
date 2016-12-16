using UnityEngine;
using System.Collections;
using System.IO;

public class LoadGameScript : MonoBehaviour {

	public string saveFileName;

	public void LoadGame () {
		string gamePath = Path.Combine (Application.persistentDataPath, saveFileName);
		Debug.Log (gamePath);
		Game.current.Load (gamePath);
	}

}

