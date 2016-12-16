using UnityEngine;
using System.Collections;
using System.IO;

public class SaveGameScript : MonoBehaviour {

	public void SaveGame () {

		string path = Application.persistentDataPath + string.Format ("/savefile{0}.sg1", System.DateTime.Now.ToString ().Replace ('/', '-'));

		Debug.Log (string.Format ("Saving to path: {0}", path));
		Game.current.Save (path);
	}

}
