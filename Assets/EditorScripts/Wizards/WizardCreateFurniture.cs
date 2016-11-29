using UnityEngine;
using UnityEditor;

public class WizardCreateFurniture : ScriptableWizard {
	[MenuItem ("GameObject/Create Furniture")]
	static void CreateWizard () {
		ScriptableWizard.DisplayWizard<WizardCreateFurniture> ("Furniture Creation!", "Create");
	}



	public string prefabName = "";
	public string hoveringPrefabName {
		get {
			return prefabName + "_hovering";
		}
	}
	public string folderPath = "Assets/";


	void OnWizardCreate () {
		GameObject myFurniture = new GameObject (prefabName);
		GameObject myFurnitureHovering = new GameObject (hoveringPrefabName);


		GameObject myFurniturePrefab = PrefabUtility.CreatePrefab (System.IO.Path.Combine (folderPath, prefabName + ".prefab"), myFurniture, ReplacePrefabOptions.ConnectToPrefab);
		GameObject myFurnitureHoveringPrefab = PrefabUtility.CreatePrefab (System.IO.Path.Combine (folderPath, hoveringPrefabName + ".prefab"), myFurnitureHovering, ReplacePrefabOptions.ConnectToPrefab);


		Furniture furniture = myFurniture.AddComponent<Furniture> ();
		Furniture_hovering hovering = myFurnitureHovering.AddComponent<Furniture_hovering> ();


		furniture.hoveringPrefab = myFurnitureHoveringPrefab;
		hovering.originalFurniture = PrefabUtility.ReplacePrefab (myFurniture, myFurniturePrefab).GetComponent<Furniture> ();

		PrefabUtility.ReplacePrefab (myFurnitureHovering, myFurnitureHoveringPrefab);
	}

	void OnWizardUpdate () {
		helpString = "Enter name of new furniture.";

		errorString = "";
		if (string.IsNullOrEmpty (prefabName))
			errorString += "Please enter a prefab name.";
		if (string.IsNullOrEmpty (folderPath))
			errorString += "\nPlease select a folder.";
	}

	protected override bool DrawWizardGUI () {
		bool propChanged = false;

		EditorGUI.BeginChangeCheck ();
		var newPrefabName = EditorGUILayout.TextField ("Prefab Name", prefabName);
		if (EditorGUI.EndChangeCheck ()) {
			prefabName = newPrefabName;
			propChanged = true;
		}

		EditorGUILayout.LabelField ("Hovering Prefab Name", hoveringPrefabName);

		EditorGUI.BeginChangeCheck ();
		var newFolderPath = EditorGUILayout.TextField ("Will be saved to:", folderPath);
		if (EditorGUI.EndChangeCheck ()) {
			folderPath = newFolderPath;
			propChanged = true;
		}



		return propChanged;
	}
}
