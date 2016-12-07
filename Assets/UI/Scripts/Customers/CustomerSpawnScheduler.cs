using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustomerSpawnScheduler : MonoBehaviour {


	public CustomerSpawner customerSpawner;


	void Start () {
		StartCoroutine (SpawningRoutine ());
	}


	private IEnumerator SpawningRoutine () {
		while (true) {
			GameObject customerObj = InstantiateACustomer ();

			if (customerObj != null) {
				ShopMover customer = customerObj.GetComponent<ShopMover> ();

				if (customer != null)
					customerSpawner.SpawnCustomer (customer);
				else
					Debug.LogFormat ("Customer object {0} does not have a ShopMover component and will not be spawned.", customerObj.name);
			} else
				Debug.Log ("No customers to spawn!");
			
			yield return new WaitForSeconds (10 + Random.value * 10);
		}
	}

	private GameObject InstantiateACustomer () {
		Game g = Game.current;

		if (g != null) {
			Dictionary<uint, float> weights = new Dictionary<uint, float> ();

			foreach (var finfo in g.furnitureInShop) {
				var fref = finfo.furnitureRef;

				if (fref != null) {
					foreach (var kv in fref.GetAttractedCustomers ()) {
						if (!weights.ContainsKey (kv.Key))
							weights [kv.Key] = kv.Value;
						else
							weights [kv.Key] += kv.Value;
					}
				}
			}

			if (weights.Count == 0)
				return null;


			float totalWeight = weights.Aggregate (0, (float total, KeyValuePair<uint, float> wgt) => total + wgt.Value);

			List<uint> ids = weights.Select ((kv) => kv.Key).ToList ();
			List<float> probabilities = weights.Select ((kv) => kv.Value / totalWeight).ToList ();


			float value = Random.value;

			float accumulated = 0;
			for (int i = 1; i < ids.Count; i++) {
				float nextCutoff = accumulated + probabilities [i - 1];
				if (value >= accumulated && value < nextCutoff) {
					var prefab = MetaInformation.Instance ().GetCustomerPrefabByID (ids [i - 1]);
					if (prefab != null)
						return GameObject.Instantiate (prefab);
					else
						Debug.LogFormat ("Customer with ID {0} does not exist!", ids [i - 1]);
				} 

				accumulated = nextCutoff;
			}

			var lastPrefab = MetaInformation.Instance ().GetCustomerPrefabByID (ids [ids.Count - 1]);
			if (lastPrefab != null)
				return GameObject.Instantiate (lastPrefab);
			else {
				Debug.LogFormat ("Customer with ID {0} does not exist!", ids [ids.Count - 1]);
				return null;
			}
		} else
			return null;
	}
}
