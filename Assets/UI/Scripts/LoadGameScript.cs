using UnityEngine;
using System.Collections;
using System.IO;

public class LoadGameScript : MonoBehaviour {

	public void LoadGame () {
		Game.current.Load (File.OpenRead (Application.persistentDataPath + "/savefile.sg1"));
	}

}

