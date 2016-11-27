using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class DisplayLoadedGames : MonoBehaviour {

	/// <summary>
	/// Should have a Text child and a LoadGameScript component.
	/// </summary>
	public GameObject loadPrefab;

	/// <summary>
	/// Should have a CreateEmptyGameScript component.
	/// </summary>
	public GameObject newGamePrefab;


	void Start () {


		CreateNewGameButton ();



		string folderPath = Application.persistentDataPath;

		string[] filePaths = Directory.GetFiles (folderPath, "*.sg1");

		foreach (string filePath in filePaths) {
			CreateEntryFor (filePath);
		}
	}


	private void CreateNewGameButton () {
		GameObject.Instantiate (newGamePrefab, transform);
	}

	private void CreateEntryFor (string filePath) {
		GameObject loadEntryInst = GameObject.Instantiate (loadPrefab, transform) as GameObject;

		Text txt = loadEntryInst.GetComponentInChildren<Text> ();
		txt.text = filePath.Split ('/').Last ();

		LoadGameScript loadScript = loadEntryInst.GetComponent<LoadGameScript> ();
		loadScript.pathToGame = filePath;
	}

}
