using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DialogContinueButton : EventTrigger {

	public delegate void ContinueDelegate ();
	public ContinueDelegate continueDelegate;

	public override void OnPointerClick (PointerEventData eventData) {
		continueDelegate ();
	}
}
