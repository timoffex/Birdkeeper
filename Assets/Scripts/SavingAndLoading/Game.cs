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




	public void Save () {
		StreamWriter saveFile = new StreamWriter (Application.persistentDataPath + "/savedGame");

		string line = string.Format ("Shop: {0}x{1}", shop.numTilesX, shop.numTilesY);
		saveFile.WriteLine (line);
		Debug.Log (line);

		foreach (Furniture f in shop.GetFurniture ()) {
			var pos = f.GetPosition ();

			line = string.Format ("F {0} ({1},{2})", f.FurnitureTypeID, pos.x, pos.y);
			saveFile.WriteLine (line);
			Debug.Log (line);
		}

		saveFile.Close ();
	}


	public void Load () {
		
		Scene scene = SceneManager.CreateScene ("GENERATED SCENE");
		SceneManager.UnloadScene (SceneManager.GetActiveScene ());
		SceneManager.SetActiveScene (scene);

		StreamReader saveFile = new StreamReader (Application.persistentDataPath + "/savedGame");

		string line1 = saveFile.ReadLine ();
		string[] shopSizes = line1.Substring (6).Split ('x');

		int shopSizeX = int.Parse (shopSizes [0]);
		int shopSizeY = int.Parse (shopSizes [1]);

		GameObject roomObj = GameObject.Instantiate (MetaInformation.Instance ().roomPrefab);
		Shop shop = roomObj.GetComponent<Shop> ();
		shop.numTilesX = shopSizeX;
		shop.numTilesY = shopSizeY;

		string line;
		while ((line = saveFile.ReadLine ()) != null && line.Length > 0) {
			if (line.StartsWith ("F ")) {
				string[] furnitureParams = line.Substring (2).Split (' ');
				uint fid = uint.Parse (furnitureParams [0]);

				string[] furniturePos = furnitureParams [1].Substring (1, furnitureParams [1].Length - 2).Split (',');
				IntPair position = new IntPair (int.Parse (furniturePos [0]), int.Parse (furniturePos [1]));

				var furnitureObj = Furniture.InstantiateFurnitureByID (fid);
				var furniture = furnitureObj.GetComponent<Furniture> ();
				furniture.PlaceAtLocation (shop, position);
			}
		}
	}
}