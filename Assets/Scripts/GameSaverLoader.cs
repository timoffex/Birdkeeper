using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class GameSaverLoader {
	private static readonly string savedGamesPath = Application.persistentDataPath + "/savedGames";

	private static List<Game> savedGames;

	private static void Load () {
		if (File.Exists (savedGamesPath)) {
			FileStream savedGamesFile = File.Open (savedGamesPath, FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter ();

			savedGames = (List<Game>)bf.Deserialize (savedGamesFile);
			savedGamesFile.Close ();
		}
	}

	private static void Save () {
		FileStream savedGamesFile = File.Create (savedGamesPath);
		BinaryFormatter bf = new BinaryFormatter ();

		bf.Serialize (savedGamesFile, savedGames);
		savedGamesFile.Close ();
	}

}
