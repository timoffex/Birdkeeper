using UnityEngine;
using System.Collections;
using System.IO;

public class SaveGameScript : MonoBehaviour {

	public void SaveGame () {
		Game.current.Save (File.OpenWrite (Application.persistentDataPath + "/savefile.sg1"));
	}

}
