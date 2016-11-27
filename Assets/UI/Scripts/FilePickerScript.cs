using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class FilePickerScript : MonoBehaviour {

	public Button saveFileButtonPrefab;
	public Button directoryButtonPrefab;

	private string directoryPath;

	// Use this for initialization
	void OnEnable () {
		if (directoryPath == null)
			directoryPath = Application.persistentDataPath;
		
		DisplayDirectory (directoryPath);
	}

	void DisplayDirectory (string path) {
		directoryPath = path;

		Clean ();

		// Directory "up" button
		Button directoryUp = GameObject.Instantiate (directoryButtonPrefab, transform) as Button;
		directoryUp.GetComponentInChildren<Text> ().text = "..";
		directoryUp.onClick.AddListener (delegate {
			DisplayDirectory (Directory.GetParent (directoryPath).FullName);
		});


		string[] directoryPaths = Directory.GetDirectories (path);
		string[] saveFilePaths = Directory.GetFiles (path, "*.sg1");

		foreach (string dirPath in directoryPaths) {
			Button directoryBtn = GameObject.Instantiate (directoryButtonPrefab, transform) as Button;
			directoryBtn.GetComponentInChildren<Text> ().text = "> " + Path.GetDirectoryName (dirPath);
			directoryBtn.onClick.AddListener (DisplayDirectoryDelegate (dirPath));
		}
	}

	UnityEngine.Events.UnityAction DisplayDirectoryDelegate (string path) {
		return () => DisplayDirectory (path);
	}

	void Clean () {
		foreach (Transform child in transform)
			Destroy (child.gameObject);
	}

}
