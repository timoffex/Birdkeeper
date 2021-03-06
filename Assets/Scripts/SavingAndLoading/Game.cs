﻿using UnityEngine;
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
			if (_current == null) {
				Debug.Log ("Game was null. Creating the Game object..");
				_current = new Game ();
			}

			return _current;
		}
	}



	public class FurnitureInfo {
		public FurnitureInfo (Furniture f) {
			furnitureRef = f;
			position = f.GetPosition ();
			furnitureID = f.FurnitureTypeID;
		}

		public FurnitureInfo (uint fid, IntPair pos) {
			furnitureID = fid;
			position = pos;
		}

		public Furniture furnitureRef;
		public IntPair position;
		public uint furnitureID;
	}



	public class FurnitureItemStack {
		public FurnitureItemStack (uint fid, uint count) {
			this.fid = fid;
			this.count = count;
		}

		public uint fid;
		public uint count;
	}



	/* Useful references. */

	public Shop shop;



	/* Persistent game-related parameters. */

	public int shopSizeX = 6; // default value is 6
	public int shopSizeY = 6; // default value is 6
	public int shopGridSizeX { get { return shopSizeX * MetaInformation.Instance ().numGridSquaresPerTile; } }
	public int shopGridSizeY { get { return shopSizeY * MetaInformation.Instance ().numGridSquaresPerTile; } }

	public List<FurnitureInfo> furnitureInShop = new List<FurnitureInfo> (); // empty by default

	public Inventory inventory = new Inventory (); // empty by default
	public FurnitureInventory furnitureInventory = new FurnitureInventory (); // empty by default

	public List<GameObject> generalObjectPrefabs = new List<GameObject> ();



	/* Useful objects. */
	public ShopGrid shopGrid;


	private GamePhase phase;
	public GamePhase Phase { get { return phase; } }



	/// <summary>
	/// Adds the furniture to the furnitureInShop list. This is used for game saving.
	/// </summary>
	/// <param name="f">The furniture.</param>
	public void AddFurnitureToShop (Furniture f) {
		furnitureInShop.Add (new FurnitureInfo (f));
	}

	/// <summary>
	/// Removes the furniture from the furnitureInShop list.
	/// </summary>
	/// <param name="f">The furniture.</param>
	public void RemoveFurnitureFromShop (Furniture f) {
		furnitureInShop.RemoveAll ((info) => info.furnitureRef == f);
	}


	/// <summary>
	/// Add the furniture object to the furniture inventory.
	/// </summary>
	/// <param name="f">The furniture.</param>
	public void AddFurnitureToInventory (Furniture f) {
		furnitureInventory.Add (new FurnitureStack (f.FurnitureTypeID, 1));
	}






	public void SwitchToPhase (GamePhase phase) {

		this.phase = phase;

		/* Auto save! */
		Save (Path.Combine (Application.persistentDataPath, "AutoSave.sg1"));

		switch (phase) {
		case GamePhase.DayPhase:
			SwitchToShopPhase ();
			break;
		case GamePhase.EditPhase:
			SwitchToEditPhase ();
			break;
		}
	}


	private void SwitchToShopPhase () {
		string validSceneName = "Shop Phase Scene";

		while (SceneManager.GetSceneByName (validSceneName).IsValid ())
			validSceneName = validSceneName + "1";

		Scene newScene = SceneManager.CreateScene (validSceneName);
		Scene currentScene = SceneManager.GetActiveScene ();
		SceneManager.UnloadScene (currentScene);
		SceneManager.SetActiveScene (newScene);

		GameObject roomObj = GameObject.Instantiate (MetaInformation.Instance ().roomPrefab) as GameObject;
		Shop newShop = roomObj.GetComponent<Shop> ();
		shop = newShop;


		shopGrid = new ShopGrid (shopGridSizeX, shopGridSizeY, 0, 4);


		FurnitureInfo[] oldFurniture = new FurnitureInfo[furnitureInShop.Count];
		furnitureInShop.CopyTo (oldFurniture);
		furnitureInShop.Clear ();

		foreach (FurnitureInfo f in oldFurniture) {
			var newFurnitureObj = Furniture.InstantiateFurnitureByID (f.furnitureID);
			var newFurniture = newFurnitureObj.GetComponent<Furniture> ();

			if (!newFurniture.PlaceAtLocation (newShop, f.position)) {
				Debug.Log ("A furniture was in an invalid position in the savefile!");
				GameObject.Destroy (newFurnitureObj);
			}
		}

		GameObject.Instantiate (MetaInformation.Instance ().shopPhaseDayCanvasPrefab);
		GameObject.Instantiate (MetaInformation.Instance ().playerPrefab);

		foreach (GameObject prefab in generalObjectPrefabs)
			GameObject.Instantiate (prefab);
	}


	private void SwitchToEditPhase () {
		string validSceneName = "Edit Phase Scene";

		while (SceneManager.GetSceneByName (validSceneName).IsValid ())
			validSceneName = validSceneName + "1";

		Scene newScene = SceneManager.CreateScene (validSceneName);
		Scene currentScene = SceneManager.GetActiveScene ();
		SceneManager.UnloadScene (currentScene);
		SceneManager.SetActiveScene (newScene);

		GameObject roomObj = GameObject.Instantiate (MetaInformation.Instance ().roomPrefab) as GameObject;
		Shop newShop = roomObj.GetComponent<Shop> ();
		shop = newShop;

		shopGrid = new ShopGrid (shopGridSizeX, shopGridSizeY, 0, 4);

		FurnitureInfo[] oldFurniture = new FurnitureInfo[furnitureInShop.Count];
		furnitureInShop.CopyTo (oldFurniture);
		furnitureInShop.Clear ();

		foreach (FurnitureInfo f in oldFurniture) {
			var newFurnitureObj = Furniture.InstantiateFurnitureByID (f.furnitureID);
			var newFurniture = newFurnitureObj.GetComponent<Furniture> ();

			if (!newFurniture.PlaceAtLocation (newShop, f.position)) {
				Debug.Log ("A furniture was in an invalid position in the savefile!");
				GameObject.Destroy (newFurnitureObj);
			}
		}

		GameObject.Instantiate (MetaInformation.Instance ().shopPhaseEditCanvasPrefab);

	}


	/* GAME SAVING / LOADING / CREATION */

	/// <summary>
	/// Creates a game with default values.
	/// </summary>
	public void CreateEmpty () {
		// Add 1 cauldron, 1 tea n' honey, 1 log chair
		furnitureInventory.Add (new FurnitureStack (41568608, 1));
		furnitureInventory.Add (new FurnitureStack (1829519687, 1));
		furnitureInventory.Add (new FurnitureStack (435603281, 1));


		// Add 1 feather, 1 bottle, 1 apple, 2 venom
		inventory.AddStack (new ItemStack (858003622, 2));
		inventory.AddStack (new ItemStack (1626868489, 2));
		inventory.AddStack (new ItemStack (957089515, 2));
		inventory.AddStack (new ItemStack (856784391, 2));



		SwitchToPhase (GamePhase.EditPhase);
	}


	public void Save (string filePath) {
		StreamWriter saveFile = new StreamWriter (File.Open (filePath, FileMode.Truncate, FileAccess.Write));

		saveFile.WriteLine (SceneManager.GetActiveScene ().name);

		string line = string.Format ("Shop: {0}x{1}", shopSizeX, shopSizeY);
		saveFile.WriteLine (line);

		foreach (FurnitureInfo f in furnitureInShop) {
			var pos = f.position;

			line = string.Format ("F {0} ({1},{2})", f.furnitureID, pos.x, pos.y);
			saveFile.WriteLine (line);
		}

		foreach (ItemStack stack in inventory.GetItemStacks ())
			saveFile.WriteLine (string.Format ("IS {0} {1}", stack.ItemType.ItemTypeID, stack.Count));

		foreach (FurnitureStack fis in furnitureInventory.GetFurnitureStacks ())
			saveFile.WriteLine (string.Format ("FI {0} {1}", fis.FurnitureID, fis.Count));


		foreach (GameIDHolder obj in GameObject.FindObjectsOfType<GameIDHolder> ())
			saveFile.WriteLine (string.Format ("GEN {0}", obj.GameID));

		saveFile.Close ();
	}


	public void Load (string filePath) {


		StreamReader saveFile = new StreamReader (File.Open (filePath, FileMode.Open, FileAccess.Read));


		saveFile.ReadLine (); // Game name

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
			} else if (line.StartsWith ("IS ")) {
				string[] itemStackParams = line.Substring (3).Split (' ');
				uint id = uint.Parse (itemStackParams [0]);
				int ct = int.Parse (itemStackParams [1]);
				inventory.AddStack (new ItemStack (id, ct));
			} else if (line.StartsWith ("GEN ")) {
				string[] myParams = line.Substring (4).Split (' ');
				uint id = uint.Parse (myParams [0]);

				generalObjectPrefabs.Add (MetaInformation.Instance ().GetGeneralObjectByID (id));
			} else if (line.StartsWith ("FI ")) {
				string[] myParams = line.Substring (3).Split (' ');
				uint fid = uint.Parse (myParams [0]);
				uint count = uint.Parse (myParams [1]);

				furnitureInventory.Add (new FurnitureStack (fid, (int)count));
			}
		}

		saveFile.Close ();

		SwitchToPhase (GamePhase.EditPhase);
	}
}