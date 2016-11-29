using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class GameIDHolder : MonoBehaviour {
	[SerializeField, HideInInspector] private uint myID = 0;

	public uint GameID {
		get {
			return myID;
		}
	}



	void Awake () {

		if (Application.isEditor) {
			// If my ID is not instantiated, set it up!
			if (myID == 0) {
				var info = MetaInformation.Instance ();
				var prefabType = PrefabUtility.GetPrefabType (gameObject);


				if (info == null) {
					Debug.LogError ("MetaInformation.Instance() returned null. The MetaInformation object is necessary.");
					DestroyImmediate (this);
				} else if (prefabType != PrefabType.PrefabInstance) {
					Debug.LogError ("This script can only be put on an object that is linked to a prefab.");
					DestroyImmediate (this);
				} else {

					GameObject myPrefab = PrefabUtility.GetPrefabParent (gameObject) as GameObject;

					uint newID = info.GetUnusedGeneralID ();


					myID = newID;
					info.AddMappingForGeneralObject (myID, myPrefab);


					Debug.LogFormat ("Setting ID for {0} to {1}", myPrefab.name, myID);
				}
			}
		} else if (myID == 0) {
			Debug.LogError ("ID is zero but application is in play mode!");
		}
	}


	public void RemoveMyIDMapping () {
		if (Application.isEditor) {
			if (myID != 0) {
				var info = MetaInformation.Instance ();

				if (info == null)
					Debug.LogError ("MetaInformation.Instance() returned null. The MetaInformation object is necessary.");
				else {
					Debug.LogFormat ("Removing ID mapping for {0}", gameObject.name);
					info.RemoveGeneralMappingForID (myID);
				}
			}
		}
	}
}
