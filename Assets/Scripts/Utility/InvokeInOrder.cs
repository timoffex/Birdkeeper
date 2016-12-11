using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


/// <summary>
/// Primarily to use with EventTriggers and Buttons to invoke a sequence
/// of events in order.
/// </summary>
public class InvokeInOrder : MonoBehaviour {

	public EventTrigger.TriggerEvent[] thingsInOrder;


	public void InvokeAll () {
		for (int i = 0; i < thingsInOrder.Length; i++)
			thingsInOrder [i].Invoke (null);
	}
}
