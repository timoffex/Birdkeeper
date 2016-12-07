using UnityEngine;
using UnityEditor;

public class CustomerSpawner : ScriptableObject {

	public IntPair spawnPosition;

	void Awake () {
		spawnPosition = new IntPair (0, 0);
	}

	/// <summary>
	/// Places the given ShopMover in the spawn position.
	/// </summary>
	/// <param name="customer">Customer.</param>
	public void SpawnCustomer (ShopMover customer) {
		customer.SetPosition (spawnPosition);
	}

	[MenuItem ("Assets/Create/Create Customer Spawner")]
	public static void CreateCustomerSpawner () {
		CustomerSpawner spawner = CreateInstance<CustomerSpawner> ();

		AssetDatabase.CreateAsset (spawner, "Assets/NewCustomerSpawner.asset");
		AssetDatabase.SaveAssets ();

		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = spawner;
	}
}
