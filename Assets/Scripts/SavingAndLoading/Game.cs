using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;

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



	public class FurnitureInfo {
		public FurnitureInfo (Furniture f) {
			position = f.GetPosition ();
			furnitureID = f.FurnitureTypeID;
		}

		public FurnitureInfo (uint fid, IntPair pos) {
			furnitureID = fid;
			position = pos;
		}

		public IntPair position;
		public uint furnitureID;
	}



	/* Useful references. */

	public Shop shop;



	/* Persistent game-related parameters. */

	public int shopSizeX = 6; // default value is 6
	public int shopSizeY = 6; // default value is 6

	public List<FurnitureInfo> furnitureInShop = new List<FurnitureInfo> (); // empty by default






	public void AddFurnitureToShop (Furniture f) {
		furnitureInShop.Add (new FurnitureInfo (f));
	}




	public void SwitchToPhase (GamePhase phase) {
		switch (phase) {
		case GamePhase.DayPhase:
			SwitchToShopPhase ();
			break;
		}
	}

	/// <summary>
	/// Assumes the shop script is already created, and just creates a scene from
	/// the information within it.
	/// </summary>
	private void SwitchToShopPhase () {
		string validSceneName = "GENERATED SCENE";

		while (SceneManager.GetSceneByName (validSceneName).IsValid ())
			validSceneName = validSceneName + "1";

		Scene newScene = SceneManager.CreateScene (validSceneName);
		Scene currentScene = SceneManager.GetActiveScene ();
		SceneManager.UnloadScene (currentScene);
		SceneManager.SetActiveScene (newScene);

		GameObject roomObj = GameObject.Instantiate (MetaInformation.Instance ().roomPrefab) as GameObject;
		Shop newShop = roomObj.GetComponent<Shop> ();
		shop = newShop;


		FurnitureInfo[] oldFurniture = new FurnitureInfo[furnitureInShop.Count];
		furnitureInShop.CopyTo (oldFurniture);
		furnitureInShop.Clear ();

		foreach (FurnitureInfo f in oldFurniture) {
			var newFurnitureObj = Furniture.InstantiateFurnitureByID (f.furnitureID);
			var newFurniture = newFurnitureObj.GetComponent<Furniture> ();

			newFurniture.PlaceAtLocation (newShop, f.position);
		}


		GameObject.Instantiate (MetaInformation.Instance ().eventSystemPrefab);
		GameObject.Instantiate (MetaInformation.Instance ().playerPrefab);
	}



	/* GAME SAVING / LOADING / CREATION */

	/// <summary>
	/// Creates a game with default values.
	/// </summary>
	public void CreateEmpty () {
		Scene newScene = SceneManager.CreateScene ("GENERATED SCENE");
		Scene currentScene = SceneManager.GetActiveScene ();
		SceneManager.UnloadScene (currentScene);
		SceneManager.SetActiveScene (newScene);


		GameObject roomObj = GameObject.Instantiate (MetaInformation.Instance ().roomPrefab);
		shop = roomObj.GetComponent<Shop> ();
		GameObject.Instantiate (MetaInformation.Instance ().shopEditorCanvasPrefab);
		GameObject.Instantiate (MetaInformation.Instance ().eventSystemPrefab);
		GameObject.Instantiate (MetaInformation.Instance ().playerPrefab);
	}


	public void Save (Stream file) {
		StreamWriter saveFile = new StreamWriter (file);

		saveFile.WriteLine (SceneManager.GetActiveScene ().name);

		string line = string.Format ("Shop: {0}x{1}", shopSizeX, shopSizeY);
		saveFile.WriteLine (line);

		foreach (FurnitureInfo f in furnitureInShop) {
			var pos = f.position;

			line = string.Format ("F {0} ({1},{2})", f.furnitureID, pos.x, pos.y);
			saveFile.WriteLine (line);
		}

		saveFile.Close ();
	}


	public void Load (Stream file) {

		StreamReader saveFile = new StreamReader (file);


		string gameName = saveFile.ReadLine ();

		string line1 = saveFile.ReadLine ();
		string[] shopSizes = line1.Substring (6).Split ('x');

		shopSizeX = int.Parse (shopSizes [0]);
		shopSizeY = int.Parse (shopSizes [1]);

		string line;
		while ((line = saveFile.ReadLine ()) != null && line.Length > 0) {
			if (line.StartsWith ("F ")) {
				string[] furnitureParams = line.Substring (2).Split (' ');
				uint fid = uint.Parse (furnitureParams [0]);

				string[] furniturePos = furnitureParams [1].Substring (1, furnitureParams [1].Length - 2).Split (',');
				IntPair position = new IntPair (int.Parse (furniturePos [0]), int.Parse (furniturePos [1]));

				furnitureInShop.Add (new FurnitureInfo (fid, position));
			}
		}


		SwitchToPhase (GamePhase.DayPhase);
	}
}