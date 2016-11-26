using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[Serializable]
public static class StaticGameInfo {

	/// <summary>
	/// A mapping from object names to their prefab paths.
	/// </summary>
	[SerializeField] private static Dictionary<string, string> nameToPrefabPath;


	/// <summary>
	/// Associates <c>name</c> with the parent prefab of <c>obj</c>.
	/// </summary>
	public static void MemorizePrefabName (GameObject obj, string name) {
		string path = AssetDatabase.GetAssetPath (PrefabUtility.GetPrefabParent (obj));
		Debug.Log (string.Format ("Associating '%s' with: %s", name, path));

		nameToPrefabPath [name] = path;
	}

	public static GameObject InstantiateObject (string name) {
		return PrefabUtility.InstantiatePrefab (AssetDatabase.LoadAssetAtPath<GameObject> (nameToPrefabPath [name])) as GameObject;
	}
}
