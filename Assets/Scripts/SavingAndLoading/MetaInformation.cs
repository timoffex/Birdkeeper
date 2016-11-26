using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// This class contains all meta-information associated with our game.
/// It links furniture IDs and furniture prefabs.
/// </summary>
public class MetaInformation : MonoBehaviour {
	/// <summary>
	/// Should not be changed during runtime. Maps Furniture IDs to prefabs.
	/// </summary>
	[SerializeField]
	private FurnitureIDMapType idToFurniturePrefab;


	public GameObject roomPrefab;
	public GameObject playerPrefab;
	public GameObject shopEditorCanvasPrefab;
	public GameObject eventSystemPrefab;


	void Awake () {
		if (Instance () == null) {
			instance = this;
		} else if (instance != this) {
			Debug.Log ("Destroying self!");
			DestroyImmediate (gameObject);
		}
	}


	public GameObject GetFurniturePrefabByID (uint id) {
		return idToFurniturePrefab [id];
	}

	public void AddMappingForFurniture (uint id, GameObject furniturePrefab) {
		if (Application.isEditor)
			idToFurniturePrefab.Add (id, furniturePrefab);
		else {
			Debug.LogError ("Cannot add ID mappings during runtime!");
			throw new UnityException ("The code tried doing something illegal. Please tell Tima at timoffex@gmail.com.");
		}
	}

	
	public IEnumerable<KeyValuePair<uint, GameObject>> GetFurnitureMappings () {
		if (idToFurniturePrefab == null) {
			idToFurniturePrefab = new FurnitureIDMapType ();
			instance = this;
		}
		
		return idToFurniturePrefab;
	}


	public uint GetUnusedFurnitureID () {
		uint id = (uint) Random.Range (1, int.MaxValue);

		while (idToFurniturePrefab.ContainsKey (id))
			id = (uint)Random.Range (1, int.MaxValue); // TODO: technically, a little unsafe

		return id;
	}

	public bool ContainsMappingForFurnitureNamed (string name) {
		return idToFurniturePrefab.Values.Where ((obj) => obj.name.Equals (name)).Count () > 0;
	}





	[System.Serializable] public class FurnitureIDMapType : SerializableDictionary<uint, GameObject> { }
	private static MetaInformation instance;
	public static MetaInformation Instance () {
		return instance;
	}
}

