using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// This class contains all meta-information associated with our game.
/// It links furniture IDs to furniture prefabs, item IDs to item scripts, and general IDs to general prefabs.
/// </summary>
public class MetaInformation : MonoBehaviour {
	/// <summary>
	/// Should not be changed during runtime. Maps Furniture IDs to prefabs.
	/// </summary>
	[SerializeField]
	private FurnitureIDMapType idToFurniturePrefab;

	[SerializeField]
	private ItemTypeIDMapType idToItemType;


	[SerializeField]
	private GeneralIDMapType idToGeneral;



	public Vector2 tileXVector;
	public Vector2 tileYVector;
	public int numGridSquaresPerTile;

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



	/// <summary>
	/// Preferred in editor scripts over finding the shop object.
	/// </summary>
	/// <returns>The to shop coordinates.</returns>
	/// <param name="world">World.</param>
	public IntPair WorldToShopVector (Vector2 world) {
		var shop = GameObject.FindObjectOfType<Shop> ();

		Vector2 dif = world;


		var xvec = tileXVector / numGridSquaresPerTile;
		var yvec = tileYVector / numGridSquaresPerTile;

		// a*XV + b*YV = dif
		// [XV | YV] * [a,b]' = dif
		// [a,b]' = [Xx Yx; Xy Yy]^-1 * dif
		// [a,b]' = [Yy -Yx; -Xy Xx] * dif / (XxYy - YxXy)

		var shopVec = new Vector2 (yvec.y * dif.x - yvec.x * dif.y, xvec.x * dif.y - xvec.y * dif.x)
			/ (xvec.x * yvec.y - yvec.x * xvec.y);

		return new IntPair (
			(int)Mathf.Floor (shopVec.x),
			(int)Mathf.Floor (shopVec.y));
	}



	public GameObject GetFurniturePrefabByID (uint id) {
		return idToFurniturePrefab [id];
	}

	public void AddMappingForFurniture (uint id, GameObject furniturePrefab) {
		if (Application.isEditor) {
			idToFurniturePrefab.Add (id, furniturePrefab);
			ApplyPrefabChanges ();
		} else {
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

	public ItemType GetItemTypeByName (string name) {
		return idToItemType.Where ((kv) => kv.Value.Name.Equals (name)).FirstOrDefault ().Value;
	}

	public void AddMappingForItemType (uint id, ItemType itemType) {
		if (Application.isEditor) {
			idToItemType.Add (id, itemType);
			ApplyPrefabChanges ();
		} else {
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

	public int GetNumberOfRegisteredItems () {
		if (idToItemType == null) {
			idToItemType = new ItemTypeIDMapType ();
			instance = this;
		}

		
		return idToItemType.Count;
	}

	public uint GetUnusedItemTypeID () {
		return GetUnusedIDFor (idToItemType);
	}




	public GameObject GetGeneralObjectByID (uint id) {
		return idToGeneral [id];
	}

	public void AddMappingForGeneralObject (uint id, GameObject gameObject) {
		if (Application.isEditor) {
			idToGeneral.Add (id, gameObject);
			ApplyPrefabChanges ();
		} else {
			Debug.LogError ("Cannot add ID mappings during runtime!");
			throw new UnityException ("The code tried doing something illegal. Please tell Tima at timoffex@gmail.com.");
		}
	}

	public void RemoveGeneralMappingForID (uint id) {
		if (Application.isEditor) {
			idToGeneral.Remove (id);
			ApplyPrefabChanges ();
		} else {
			Debug.LogError ("Cannot add ID mappings during runtime!");
			throw new UnityException ("The code tried doing something illegal. Please tell Tima at timoffex@gmail.com.");
		}
	}

	public IEnumerable<KeyValuePair<uint, GameObject>> GetGeneralIDMappings () {
		if (idToGeneral == null) {
			idToGeneral = new GeneralIDMapType ();
			instance = this;
		}

		return idToGeneral;
	}

	public uint GetUnusedGeneralID () {
		return GetUnusedIDFor (idToGeneral);
	}




	private uint GetUnusedIDFor (System.Collections.IDictionary dict) {
		uint id = (uint) Random.Range (1, int.MaxValue);

		while (dict.Contains (id))
			id = (uint)Random.Range (1, int.MaxValue); // TODO: technically, a little unsafe

		return id;
	}




	[System.Serializable] public class FurnitureIDMapType : SerializableDictionary<uint, GameObject> { }
	[System.Serializable] public class ItemTypeIDMapType : SerializableDictionary<uint, ItemType> { }
	[System.Serializable] public class GeneralIDMapType : SerializableDictionary<uint, GameObject> { }

	private static MetaInformation instance;
	public static MetaInformation Instance () {
		if (instance != null)
			return instance;
		

		MetaInformation foundInstance = GameObject.FindObjectOfType<MetaInformation> ();
		if (foundInstance != null) {
			instance = foundInstance;
			return instance;
		}


		if (Application.isEditor) {
			// Attempt to add our MetaInformation object to the scene!
			string[] guids = AssetDatabase.FindAssets ("MetaInformation t:prefab", new string[] {"Assets/Prefabs"});

			string[] paths = guids.Select ((s) => AssetDatabase.GUIDToAssetPath (s)).ToArray ();

			if (paths.Length == 1) {
				Debug.LogFormat ("Loading MetaInformation from path: ", paths [0]);

				GameObject instancePrefab = AssetDatabase.LoadAssetAtPath<GameObject> (paths [0]);
				GameObject instanceGO = GameObject.Instantiate (instancePrefab);
				instanceGO = PrefabUtility.ConnectGameObjectToPrefab (instanceGO, instancePrefab);

				instance = instanceGO.GetComponent<MetaInformation> ();
				if (instance == null) {
					Debug.LogError ("Loaded MetaInformation prefab but did not find MetaInformation script... destroying loaded object.");
					DestroyImmediate (instanceGO);
				}

				return instance;
			} else if (paths.Length > 1) {
				Debug.LogError ("Multiple MetaInformation prefabs found:\n" + paths.Aggregate ((s1, s2) => s1 + "\n" + s2));
			} else {
				Debug.LogError ("Could not find MetaInformation prefab.");
			}
		}

		return null;
	}

	public void SetCurrent () {
		instance = this;
	}

	/// <summary>
	/// If the instance is null, will not try to instantiate it. Useful in OnDestroy () scripts.
	/// </summary>
	/// <returns>The safe.</returns>
	public static MetaInformation InstanceSafe () {
		return instance;
	}


	private void ApplyPrefabChanges () {
		PrefabUtility.ReplacePrefab (gameObject,
			PrefabUtility.GetPrefabParent (gameObject),
			ReplacePrefabOptions.ConnectToPrefab);
	}
}

