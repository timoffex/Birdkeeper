using UnityEngine;
using System.Collections;


public class ShopMoverBasic : ShopMover {
	private Shop shop;
	public IntPair position = new IntPair(0, 0);

	/// <summary>
	/// In grid squares per second.
	/// </summary>
	public float speed = 1f;

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		StartCoroutine (LateStart ());
	}

	void Update () {
		spriteRenderer.sortingOrder = 2 * (position.x + position.y);
	}

	IEnumerator LateStart () {
		yield return new WaitForEndOfFrame ();
		transform.position = GetShop ().shopToWorldCoordinates (position);
	}


	public override Shop GetShop () {
		if (!shop)
			shop = GameObject.Find ("Room").GetComponent<Shop> ();
		
		return shop;
	}

	public override IntPair GetPosition () {
		return position;
	}

	public override void SetPosition (IntPair pos) {
		position = pos;
	}


	public override IEnumerator MoveToPosition (IntPair targetPos, SuccessCallback callback) {
		
		IntPair[] path = GetShop ().FindPath (position, targetPos);

		if (path == null) {
			callback (false);
		} else {

			for (int i = 0; i < path.GetLength (0); i++) {


				/* Gliding script */
				float dx = position.x - path [i].x;
				float dy = position.y - path [i].y;
				float dist = Mathf.Sqrt (dx * dx + dy * dy);

				Vector3 target = (Vector3)GetShop ().shopToWorldCoordinates (path [i]);
				Vector3 original = transform.position;

				float timeStart = Time.time;
				float p = 0;

				while (p < 1) {
					p = Mathf.Min (1, (Time.time - timeStart) * speed / dist);
					transform.position = (1 - p) * original + p * target;

					yield return new WaitForEndOfFrame ();
				}
				/* Gliding script */


				position = path [i];
			}

			callback (true);
		}
	}

	private IEnumerator GlideTo (IntPair pos) {
		float dx = position.x - pos.x;
		float dy = position.y - pos.y;
		float dist = Mathf.Sqrt (dx * dx + dy * dy);

		Vector3 target = (Vector3)GetShop ().shopToWorldCoordinates (position);


		int numItr = 1;
		float p = Time.deltaTime * speed / dist;
		transform.position = (1 - p) * transform.position + p * target;

		while (p < 1) {
			yield return new WaitForEndOfFrame ();

			p = Mathf.Min (1, (++numItr) * Time.deltaTime * speed / dist);
			transform.position = (1 - p) * transform.position + p * target;
		}

		yield return null;
	}
}
