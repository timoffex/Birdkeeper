using UnityEngine;
using UnityEditor;

public class WizardCreateFurniture : ScriptableWizard {
	[MenuItem ("GameObject/Create Furniture")]
	static void CreateWizard () {
		ScriptableWizard.DisplayWizard<WizardCreateFurniture> ("Furniture Creation!", "Create");
	}



	public string furnitureName = "";
	public string hoveringName {
		get {
			return furnitureName + "_hovering";
		}
	}


	void OnWizardCreate () {
		GameObject myFurniture = new GameObject (furnitureName);
		GameObject myFurnitureHovering = new GameObject (hoveringName);



		Furniture furniture = myFurniture.AddComponent<Furniture> ();
		Furniture_hovering hovering = myFurnitureHovering.AddComponent<Furniture_hovering> ();


		furniture.hoveringPrefab = myFurniture;
		hovering.originalFurniture = furniture;
	}

	void OnWizardUpdate () {
		helpString = "Enter name of new furniture.";

		errorString = "";
		if (string.IsNullOrEmpty (furnitureName))
			errorString += "Please enter a furniture name.";
	}

	protected override bool DrawWizardGUI () {
		bool propChanged = false;

		EditorGUI.BeginChangeCheck ();
		var newPrefabName = EditorGUILayout.TextField ("Furniture Name", furnitureName);
		if (EditorGUI.EndChangeCheck ()) {
			furnitureName = newPrefabName;
			propChanged = true;
		}

		EditorGUILayout.LabelField ("Hovering Object Name", hoveringName);



		return propChanged;
	}
}
