using UnityEngine;
using System.Collections;

public class CustomerSpawnScheduler : MonoBehaviour {


	public CustomerSpawner customerSpawner;
	public ShopMover customerPrefab;


	void Start () {
		StartCoroutine (SpawningRoutine ());
	}


	private IEnumerator SpawningRoutine () {
		while (true) {
			ShopMover customer = GameObject.Instantiate (customerPrefab) as ShopMover;
			customerSpawner.SpawnCustomer (customer);
			yield return new WaitForSeconds (10 + Random.value * 10);
		}
	}
}
