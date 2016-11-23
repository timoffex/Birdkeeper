using UnityEngine;
using System.Collections;

public class ShopMoverGrid : ShopMover {


	private Shop shop;
	private Animator animator;

	private IntPair farCorner = new IntPair (0, 0);
	private IntPair nearCorner {
		get {
			return new IntPair (farCorner.x + gridWidth - 1, farCorner.y + gridHeight - 1);
		}
	}
	private IntPair middlePosition {
		get {
			return new IntPair ((farCorner.x + nearCorner.x) / 2, (farCorner.y + nearCorner.y) / 2);
		}
	}

	private Vector2 nearCornerFloat {
		get {
			var farPos = GetShop ().worldToShopCoordinatesFloat ((Vector2)transform.position + gridOffset);
			return farPos + new Vector2 (gridWidth - 1, gridHeight - 1);
		}
	}


	public Vector2 gridOffset = new Vector2 (0, 0);
	public int gridWidth = 1;
	public int gridHeight = 1;


	/// <summary>
	/// In grid squares per second.
	/// </summary>
	public float speed = 10f;


	private SpriteRenderer spriteRenderer;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		shop = GameObject.Find ("Room").GetComponent<Shop> (); // TODO SHOP
		StartCoroutine (LateStart ());

	}

	void Update () {
		var ncf = nearCornerFloat;
		spriteRenderer.sortingOrder = (int) Mathf.Floor (2 * (ncf.x + ncf.y) - 2);
	}

	IEnumerator LateStart () {
		yield return new WaitForEndOfFrame ();
		transform.position = GetShop ().shopToWorldCoordinates (farCorner) - gridOffset;
	}




	public override Shop GetShop () {
		return shop;
	}


	public override IntPair GetPosition () {
		return middlePosition;
	}



	public override IEnumerator MoveToPosition (IntPair pos, SuccessCallback callback = null) {

		if (animator != null) animator.SetBool (AnimationStandards.IS_MOVING, true);

		IntPair endPoint = pos;//new IntPair (pos.x - (gridWidth+1)/2, pos.y - (gridHeight+1)/2);

		if (!shop.IsPositionInGrid (endPoint)) {
			callback (false);
			yield break;
		}

		int cGridWidth = shop.numGridX - gridWidth + 1;
		int cGridHeight = shop.numGridY - gridHeight + 1;
		bool[,] condensedGrid = new bool[cGridWidth, cGridHeight];

		for (int x = 0; x < cGridWidth; x++) {
			for (int y = 0; y < cGridHeight; y++) {

				condensedGrid [x, y] = false;
				for (int a = x; a < x + gridWidth && !condensedGrid [x, y]; a++)
					for (int b = y; b < y + gridHeight  && !condensedGrid [x, y]; b++)
						if (GetShop ().GetGrid (a, b))
							condensedGrid [x, y] = true;

			}
		}


		IntPair[] path = Pathfinding.FindPath (condensedGrid, farCorner, endPoint);

		if (path == null) {
			if (animator != null) animator.SetBool (AnimationStandards.IS_MOVING, false);
			callback (false);
		} else {
			
			for (int i = 0; i < path.GetLength (0); i++) {


				/* Gliding script */
				int dx = path [i].x - farCorner.x;
				int dy = path [i].y - farCorner.y;
				float dist = Mathf.Sqrt (dx * dx + dy * dy);

				Vector3 target = (Vector3)GetShop ().shopToWorldCoordinates (path [i]) - (Vector3)gridOffset;
				Vector3 original = transform.position;

				if (animator != null) {
					if (dx == 0) {
						if (dy > 0) {
							animator.SetBool (AnimationStandards.FACING_FRONT, true);
							animator.SetBool (AnimationStandards.FACING_RIGHT, false);
						} else if (dy < 0) {
							animator.SetBool (AnimationStandards.FACING_FRONT, false);
							animator.SetBool (AnimationStandards.FACING_RIGHT, true);
						}
					} else if (dy == 0) {
						if (dx > 0) {
							animator.SetBool (AnimationStandards.FACING_FRONT, true);
							animator.SetBool (AnimationStandards.FACING_RIGHT, true);
						} else if (dx < 0) {
							animator.SetBool (AnimationStandards.FACING_FRONT, false);
							animator.SetBool (AnimationStandards.FACING_RIGHT, false);
						}
					}
				}

				float timeStart = Time.time;
				float p = 0;

				while (p < 1) {
					p = Mathf.Min (1, (Time.time - timeStart) * speed / dist);
					transform.position = (1 - p) * original + p * target;

					yield return new WaitForEndOfFrame ();
				}
				/* Gliding script */


				farCorner = path [i];
			}


			if (animator != null) animator.SetBool (AnimationStandards.IS_MOVING, false);
			callback (true);

		}
	}
}
