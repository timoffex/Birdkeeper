using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ShopEventSystem : EventTrigger {

	private static ShopEventSystem instance;
	public static ShopEventSystem Instance () { 
		if (instance == null)
			instance = GameObject.FindObjectOfType<ShopEventSystem> ();
		return instance;
	}


	public delegate void ListenerCallback ();
	public class Listener {
		public Listener (Collider2D c, ListenerCallback e) {
			collider = c;
			callback = e;
		}

		public Collider2D collider;
		public ListenerCallback callback;
	}


	private List<Listener> clickListeners = new List<Listener> ();

	


	



	public Listener RegisterClickListener (Collider2D collider, ListenerCallback callback) {
		Listener ls = new Listener (collider, callback);

		clickListeners.Add (ls);

		return ls;
	}


	public override void OnPointerClick (PointerEventData eventData) {
		Debug.Log ("Detected click!");
		if (MouseClick ()) {
			eventData.Use ();
		}
	}



	public bool MouseClick () {
		return MouseClickAt (Input.mousePosition);
	}

	public bool MouseClickAt (Vector2 screenPosition) {
		bool used = false;

		List<Listener> toRemove = new List<Listener> ();
		foreach (Listener ls in clickListeners) {
			if (ls.collider != null && ls.collider.gameObject != null) {
				if (ls.collider.OverlapPoint (Camera.main.ScreenToWorldPoint (screenPosition))) {
					ls.callback ();
					used = true;
					break;
				}
			} else
				toRemove.Add (ls);
		}

		foreach (Listener ls in toRemove)
			clickListeners.Remove (ls);

		return used;
	}


}
