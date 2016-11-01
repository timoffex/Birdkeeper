using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ShopMover))]
public class MoveToMouseClick : MonoBehaviour {

	private ShopMover mover;
	private IEnumerator movingCoroutine = null;

	// Use this for initialization
	void Start () {
		mover = GetComponent<ShopMover> ();
	}

	private void MoveCallback (bool success) {
		movingCoroutine = null;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			Shop shop = mover.GetShop ();

			IntPair shopCoords = shop.worldToShopCoordinates (Camera.main.ScreenToWorldPoint (Input.mousePosition));

			if (shop.IsPositionInGrid (shopCoords)) {
				if (movingCoroutine != null)
					StopCoroutine (movingCoroutine);
			
				movingCoroutine = mover.MoveToPosition (shopCoords, MoveCallback);
				StartCoroutine (movingCoroutine);
			}
		}
	}
}
