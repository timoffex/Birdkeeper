using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ShopMover))]
public class MoveToMouseClickShopOnly : MonoBehaviour {

	private ShopMover mover;
	private IEnumerator movingCoroutine = null;

	private float lastDown = -1;
	public float clickDelay = .5f;

	// Use this for initialization
	void Start () {
		mover = GetComponent<ShopMover> ();
	}

	private void MoveCallback (bool success) {
		movingCoroutine = null;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0))
			lastDown = Time.time;

		if (Input.GetMouseButtonUp (0) && Time.time - lastDown < clickDelay) {
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
