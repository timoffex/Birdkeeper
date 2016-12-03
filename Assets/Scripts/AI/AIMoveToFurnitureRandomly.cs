using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ShopMover))]
public class AIMoveToFurnitureRandomly : MonoBehaviour {

	private ShopMover shopMover;

	private float nextMoveTime;
	private bool isMoving;

	// Use this for initialization
	void Start () {
		shopMover = GetComponent<ShopMover> ();
		nextMoveTime = Time.time;
		isMoving = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isMoving && Time.time > nextMoveTime) {
			Shop shop = shopMover.GetShop ();

			int numFurniture = shop.GetFurnitureAmount ();

			if (numFurniture > 0) {
				int idx = Random.Range (0, numFurniture);
				Furniture furniture = shop.GetFurnitureAtIndex (idx);

				isMoving = true;
				StartCoroutine (shopMover.MoveToPosition (furniture.GetStandingPosition (), 
					(s) => {
						isMoving = false;
						nextMoveTime = Time.time + 2 + Random.value * 3;
					}));
			}
		}
	}
}
