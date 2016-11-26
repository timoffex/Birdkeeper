using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization;
using System.IO;

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

		string line = string.Format ("Shop: {0}x{1}", shop.numTilesX, shop.numTilesY);
		saveFile.WriteLine (line);

		foreach (Furniture f in shop.GetFurniture ()) {
			var pos = f.GetPosition ();

			line = string.Format ("F {0} ({1},{2})", f.FurnitureTypeID, pos.x, pos.y);
			saveFile.WriteLine (line);
		}

		saveFile.Close ();
	}


	public void Load (Stream file) {

		StreamReader saveFile = new StreamReader (file);


		string gameName = saveFile.ReadLine ();
		
		Scene newScene = SceneManager.CreateScene ("GENERATED SCENE");
		Scene currentScene = SceneManager.GetActiveScene ();
		SceneManager.UnloadScene (currentScene);
		SceneManager.SetActiveScene (newScene);



		string line1 = saveFile.ReadLine ();
		string[] shopSizes = line1.Substring (6).Split ('x');

		int shopSizeX = int.Parse (shopSizes [0]);
		int shopSizeY = int.Parse (shopSizes [1]);

		GameObject roomObj = GameObject.Instantiate (MetaInformation.Instance ().roomPrefab);
		Shop shop = roomObj.GetComponent<Shop> ();
		shop.numTilesX = shopSizeX;
		shop.numTilesY = shopSizeY;
		this.shop = shop;


		GameObject.Instantiate (MetaInformation.Instance ().shopEditorCanvasPrefab);
		GameObject.Instantiate (MetaInformation.Instance ().eventSystemPrefab);

		string line;
		while ((line = saveFile.ReadLine ()) != null && line.Length > 0) {
			if (line.StartsWith ("F ")) {
				string[] furnitureParams = line.Substring (2).Split (' ');
				uint fid = uint.Parse (furnitureParams [0]);

				string[] furniturePos = furnitureParams [1].Substring (1, furnitureParams [1].Length - 2).Split (',');
				IntPair position = new IntPair (int.Parse (furniturePos [0]), int.Parse (furniturePos [1]));

				var furnitureObj = Furniture.InstantiateFurnitureByID (fid);
				var furniture = furnitureObj.GetComponent<Furniture> ();

				if (!furniture.PlaceAtLocation (shop, position)) {
					Debug.LogError ("Cannot place furniture at given location! Cannot load game properly..");
					throw new UnityException ("Cannot load game properly.");
				}
			}
		}


		GameObject.Instantiate (MetaInformation.Instance ().playerPrefab);
	}
}