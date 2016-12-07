using UnityEngine;
using System.Collections;
using System.Linq;


[RequireComponent (typeof (RectangularGridObject))]
public class ShopMoverGrid : ShopMover {

	private Animator animator;



	private RectangularGridObject _internalGrid;
	public RectangularGridObject MyGrid {
		get {
			if (_internalGrid == null)
				_internalGrid = GetComponent<RectangularGridObject> ();
			if (_internalGrid == null) {
				_internalGrid = gameObject.AddComponent<RectangularGridObject> ();
				Debug.Log ("ShopMover did not have RectangularGridObject.. Adding automatically!");
			}

			return _internalGrid;
		}
	}


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
	public int gridWidth {
		get {
			return MyGrid.gridSizeX;
		}
	}
		
	public int gridHeight {
		get {
			return MyGrid.gridSizeY;
		}
	}


	/// <summary>
	/// In grid squares per second.
	/// </summary>
	public float speed = 10f;


	private SpriteRenderer spriteRenderer;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();

		transform.position = GetShop ().shopToWorldCoordinates (farCorner) - gridOffset;
	}

	void Update () {
		var ncf = nearCornerFloat;
		spriteRenderer.sortingOrder = (int) Mathf.Floor (2 * (ncf.x + ncf.y) - 2);
	}




	public override Shop GetShop () {
		return Game.current.shop;
	}


	public override IntPair GetPosition () {
		return middlePosition;
	}

	public override void SetPosition (IntPair pos) {
		farCorner = pos;
	}



	public override IEnumerator MoveToPosition (IntPair pos, SuccessCallback callback = null) {

		if (callback == null)
			callback = (s) => {};

		if (animator != null) animator.SetBool (AnimationStandards.IS_MOVING, true);

		IntPair endPoint = pos;



		Game game = Game.current;

		if (game != null) {
			ShopGrid grid = game.shopGrid;

			if (grid != null) {
				bool done = false;


				while (!done) {
					IntPath path = grid.FindPath (MyGrid, farCorner, endPoint);

					if (path == null) {
						#region callback(false) and exit loop
						if (animator != null)
							animator.SetBool (AnimationStandards.IS_MOVING, false);
						callback (false);
						done = true;
						#endregion
					} else {
						#region try to follow path; if path succeeds, callback(true) and exit
						bool pathSucceeded = true;
						for (int i = 0; i < path.Length; i++) {
							yield return Glide (farCorner, path [i]);
							farCorner = path [i];
						}


						if (pathSucceeded) {
							if (animator != null)
								animator.SetBool (AnimationStandards.IS_MOVING, false);
							callback (true);
							done = true;
						}
						#endregion
					}
				}
			}
		} else
			callback (false);
	}

	private IEnumerator Glide (IntPair startPos, IntPair pos) {
		
		Vector3 target = (Vector3)GetShop ().shopToWorldCoordinates (pos) - (Vector3)gridOffset;
		Vector3 original = transform.position;

		IntPair dif = new IntPair (pos.x - startPos.x, pos.y - startPos.y);
		float dist = Mathf.Sqrt (dif.x * dif.x + dif.y * dif.y);

		if (dist != 0) {
			if (animator != null) {
				if (dif.x == 0) {
					if (dif.y > 0) {
						animator.SetBool (AnimationStandards.FACING_FRONT, true);
						animator.SetBool (AnimationStandards.FACING_RIGHT, false);
					} else if (dif.y < 0) {
						animator.SetBool (AnimationStandards.FACING_FRONT, false);
						animator.SetBool (AnimationStandards.FACING_RIGHT, true);
					}
				} else if (dif.y == 0) {
					if (dif.x > 0) {
						animator.SetBool (AnimationStandards.FACING_FRONT, true);
						animator.SetBool (AnimationStandards.FACING_RIGHT, true);
					} else if (dif.x < 0) {
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
		}
	}
}
