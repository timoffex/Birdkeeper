using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DoSomethingOnClick : EventTrigger {

	public delegate void ClickDelegate ();
	public ClickDelegate clickDelegate;

	public override void OnPointerClick (PointerEventData eventData) {
		clickDelegate ();
	}
}
