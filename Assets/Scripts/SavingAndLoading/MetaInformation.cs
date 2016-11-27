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

	[SerializeField]
	private ItemTypeIDMapType idToItemType;


	public GameObject roomPrefab;
	public GameObject playerPrefab;
	public GameObject eventSystemPrefab;
	public GameObject shopEditorCanvasPrefab;
	public GameObject shopPhaseDayCanvasPrefab;


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
		return GetUnusedIDFor (idToFurniturePrefab);
	}

	public bool ContainsMappingForFurnitureNamed (string name) {
		return idToFurniturePrefab.Values.Where ((obj) => obj.name.Equals (name)).Count () > 0;
	}




	public ItemType GetItemTypeByID (uint id) {
		return idToItemType [id];
	}

	public void AddMappingForItemType (uint id, ItemType itemType) {
		if (Application.isEditor)
			idToItemType.Add (id, itemType);
		else {
			Debug.LogError ("Cannot add ID mappings during runtime!");
			throw new UnityException ("The code tried doing something illegal. Please tell Tima at timoffex@gmail.com.");
		}
	}

	public IEnumerable<KeyValuePair<uint, ItemType>> GetItemTypeMappings () {
		if (idToItemType == null) {
			idToItemType = new ItemTypeIDMapType ();
			instance = this;
		}

		return idToItemType;
	}

	public uint GetUnusedItemTypeID () {
		return GetUnusedIDFor (idToItemType);
	}





	private uint GetUnusedIDFor (System.Collections.IDictionary dict) {
		uint id = (uint) Random.Range (1, int.MaxValue);

		while (dict.Contains (id))
			id = (uint)Random.Range (1, int.MaxValue); // TODO: technically, a little unsafe

		return id;
	}




	[System.Serializable] public class FurnitureIDMapType : SerializableDictionary<uint, GameObject> { }
	[System.Serializable] public class ItemTypeIDMapType : SerializableDictionary<uint, ItemType> { }

	private static MetaInformation instance;
	public static MetaInformation Instance () {
		return instance;
	}
}

