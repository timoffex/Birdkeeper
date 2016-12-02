using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;


/// <summary>
/// This class contains all meta-information associated with our game.
/// It links furniture IDs to furniture prefabs, item IDs to item scripts, and general IDs to general prefabs.
/// </summary>
public class MetaInformation : MonoBehaviour {
	[SerializeField] private FurnitureIDMapType idToFurniturePrefab;
	[SerializeField] private ItemTypeIDMapType idToItemType;
	[SerializeField] private GeneralIDMapType idToGeneral;



	public Vector2 tileXVector;
	public Vector2 tileYVector;
	public int numGridSquaresPerTile;

	public GameObject roomPrefab;
	public GameObject playerPrefab;
	public GameObject eventSystemPrefab;
	public GameObject shopEditorCanvasPrefab;
	public GameObject shopPhaseDayCanvasPrefab;
	public GameObject shopPhaseEditCanvasPrefab;


	void Awake () {
		SetCurrent ();
	}



	/// <summary>
	/// Preferred in editor scripts over finding the shop object.
	/// </summary>
	/// <returns>The to shop coordinates.</returns>
	/// <param name="world">World.</param>
	public IntPair WorldToShopVector (Vector2 world) {
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
		ItemType val;

		if (idToItemType.TryGetValue (id, out val))
			return val;
		else
			return null;
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

		bool remakeDict = false;

		List<KeyValuePair<uint, ItemType>> mySanitizedEntries = new List<KeyValuePair<uint, ItemType>> ();
		foreach (var entry in idToItemType)
			if (entry.Key != 0 && entry.Value != null)
				mySanitizedEntries.Add (entry);
			else 
				remakeDict = true;


		if (remakeDict) {
			Debug.Log ("MetaInformation found a null or 0-id entry.");
			idToItemType = new ItemTypeIDMapType ();

			foreach (var entry in mySanitizedEntries)
				idToItemType [entry.Key] = entry.Value;
		}
		

		return mySanitizedEntries;
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


	private string GetSaveFilePath () {
		return System.IO.Path.Combine (Application.dataPath, "MetaInformationSavefile.metainfo");
	}


	private void ApplyPrefabChanges () {
		PrefabUtility.ReplacePrefab (gameObject,
			PrefabUtility.GetPrefabParent (gameObject),
			ReplacePrefabOptions.ConnectToPrefab);
	}


	private string FieldValueToString (object obj) {
		if (obj is int)
			return string.Format ("int {0}", obj.ToString ());
		else if (obj is Vector2)
			return string.Format ("Vector2 ({0},{1})", ((Vector2)obj).x, ((Vector2)obj).y);
		else if (obj is GameObject)
			return string.Format ("Prefab {0}", AssetDatabase.GetAssetPath (PrefabUtility.GetPrefabObject ((Object)obj)));
		else {
			Debug.LogErrorFormat ("FieldValueToString doesn't support object! {0}", obj.ToString ());
			return string.Format ("UNKNOWN {0}", obj.ToString ());
		}
	}

	private object StringValueToObject (string objString) {
		int firstSpace = objString.IndexOf (' ');
		string type = objString.Substring (0, firstSpace);
		string value = objString.Substring (firstSpace + 1);

		if (type.Equals ("int"))
			return int.Parse (value);
		if (type.Equals ("Vector2")) {
			int commaIdx = value.IndexOf (',');
			int lastIdx = value.Length - 1;
			return new Vector2 (float.Parse (value.Substring (1, commaIdx - 1)),
				float.Parse (value.Substring (commaIdx + 1, lastIdx - commaIdx - 1)));
		} else if (type.Equals ("Prefab"))
			return AssetDatabase.LoadAssetAtPath<GameObject> (value);
		else {
			Debug.LogErrorFormat ("Unimplemented MetaInformation type: {0}", type);
			return null;
		}
	}


	public void Save () {
		StreamWriter writer;


		if (!File.Exists (GetSaveFilePath ())) {
			Debug.Log ("Creating meta information save file.");
			writer = File.CreateText (GetSaveFilePath ());
		} else {
			writer = new StreamWriter (GetSaveFilePath ());
		}


		/* save all public variables */
		foreach (var field in this.GetType ().GetFields ())
			if (field.IsPublic)
				writer.WriteLine ("FIELD {0} {1}", field.Name, FieldValueToString (field.GetValue (this)));


		writer.WriteLine ("Furniture Mappings");
		foreach (var kv in idToFurniturePrefab) {
			uint furnitureID = kv.Key;
			Object prefab = PrefabUtility.GetPrefabObject (kv.Value);
			string prefabPath = AssetDatabase.GetAssetPath (prefab);

			writer.WriteLine (string.Format ("\t{0} {1}", furnitureID, prefabPath));
		}

		writer.WriteLine ("Item Type Mappings");
		foreach (var kv in idToItemType) {
			uint itemTypeID = kv.Key;
			ItemType type = kv.Value;

			string itemName = type.Name;
			string spritePath = "null";
			if (type.Icon != null) spritePath = AssetDatabase.GetAssetPath (type.Icon);

			writer.WriteLine (string.Format ("\t{0} '{1}' {2}", itemTypeID, itemName, spritePath));
			foreach (var stack in type.Recipe.GetRequiredItems ())
				writer.WriteLine (string.Format ("\t\t{0} {1}", stack.ItemTypeID, stack.Count));
		}

		writer.WriteLine ("General Mappings");
		foreach (var kv in idToGeneral) {
			uint genID = kv.Key;
			Object prefab = PrefabUtility.GetPrefabObject (kv.Value);
			string prefabPath = AssetDatabase.GetAssetPath (prefab);

			writer.WriteLine (string.Format ("\t{0} {1}", genID, prefabPath));
		}

		writer.Close ();
	}


	public void Load () {
		if (!File.Exists (GetSaveFilePath ()))
			return;
		

		StreamReader reader = new StreamReader (GetSaveFilePath ());

		Clean ();
		string line = reader.ReadLine ();
		while (line != null) {

			if (line.StartsWith ("FIELD"))
				line = LoadProcessFieldLine (reader, line);
			else if (line.Equals ("Furniture Mappings"))
				line = LoadProcessFurnitureMappings (reader, line);
			else if (line.Equals ("Item Type Mappings"))
				line = LoadProcessItemTypeMappings (reader, line);
			else if (line.Equals ("General Mappings"))
				line = LoadProcessGeneralMappings (reader, line);
			else if (!string.IsNullOrEmpty (line)) {
				Debug.LogFormat ("Ignoring line: {0}", line);
				line = reader.ReadLine ();
			}
		}
	}

	private void Clean () {
		idToFurniturePrefab.Clear ();
		idToItemType.Clear ();
		idToGeneral.Clear ();
	}


	private string LoadProcessFieldLine (StreamReader reader, string line) {
		int space1 = line.IndexOf (' ');
		int space2 = line.IndexOf (' ', space1 + 1);

		string name = line.Substring (space1 + 1, space2 - space1 - 1);
		string typeAndValue = line.Substring (space2 + 1);

		var myField = typeof(MetaInformation).GetField ("name");
		if (myField != null) {
			var val = StringValueToObject (typeAndValue);

			try {
				myField.SetValue (this, val);
			} catch {
				Debug.LogErrorFormat ("Wrong type in MetaInformation savefile. Ignoring {0}", name);
			}
		}

		return reader.ReadLine ();
	}

	private string LoadProcessFurnitureMappings (StreamReader reader, string line) {

		string nextLine = reader.ReadLine ();

		while (nextLine != null && nextLine.StartsWith ("\t")) {
			int firstIdx = 1;
			int space1 = nextLine.IndexOf (' ');

			uint fid = uint.Parse (nextLine.Substring (firstIdx, space1 - firstIdx));
			string prefabPath = nextLine.Substring (space1 + 1);
			GameObject furniturePrefab = AssetDatabase.LoadAssetAtPath<GameObject> (prefabPath);


			if (furniturePrefab != null)
				idToFurniturePrefab [fid] = furniturePrefab;
			else
				Debug.LogErrorFormat ("Could not locate prefab at {0}", prefabPath);

			nextLine = reader.ReadLine ();
		}

		return nextLine;
	}

	private string LoadProcessItemTypeMappings (StreamReader reader, string line) {

		string nextLine = reader.ReadLine ();

		while (nextLine != null && nextLine.StartsWith ("\t")) {
			int firstIdx = 1;
			int space1 = nextLine.IndexOf (' ');
			int quote1 = nextLine.IndexOf ('\'');
			int quote2 = nextLine.IndexOf ('\'', quote1 + 1);
			int space2 = nextLine.IndexOf (' ', quote2 + 1);

			uint itemID = uint.Parse (nextLine.Substring (firstIdx, space1 - firstIdx));
			string itemName = nextLine.Substring (quote1 + 1, quote2 - quote1 - 1);
			string spritePath = nextLine.Substring (space2 + 1);

			Sprite sprite;
			if (spritePath.Equals ("null"))
				sprite = null;
			else
				sprite = AssetDatabase.LoadAssetAtPath<Sprite> (spritePath);

			ItemType type = new ItemType (itemName, itemID, sprite);

			nextLine = reader.ReadLine ();

			List<ItemStack> recipeStacks = new List<ItemStack> ();
			while (nextLine != null && nextLine.StartsWith ("\t\t")) {
				// Read in recipe for item
				firstIdx = 2;
				space1 = nextLine.IndexOf (' ');

				uint stackItemID = uint.Parse (nextLine.Substring (firstIdx, space1 - firstIdx));
				int stackCount = int.Parse (nextLine.Substring (space1 + 1));

				recipeStacks.Add (new ItemStack (stackItemID, stackCount));
				nextLine = reader.ReadLine ();
			}

			type.SetRecipe (new ItemRecipe (recipeStacks.ToArray ()));

			idToItemType [itemID] = type;
		}

		return nextLine;
	}

	private string LoadProcessGeneralMappings (StreamReader reader, string line) {

		string nextLine = reader.ReadLine ();

		while (nextLine != null && nextLine.StartsWith ("\t")) {
			int firstIdx = 1;
			int space1 = nextLine.IndexOf (' ');

			uint genID = uint.Parse (nextLine.Substring (firstIdx, space1 - firstIdx));
			string prefabPath = nextLine.Substring (space1 + 1);
			GameObject generalPrefab = AssetDatabase.LoadAssetAtPath<GameObject> (prefabPath);


			if (generalPrefab != null)
				idToGeneral [genID] = generalPrefab;
			else
				Debug.LogErrorFormat ("Could not locate prefab at {0}", prefabPath);
			
			nextLine = reader.ReadLine ();
		}

		return nextLine;
	}
}

