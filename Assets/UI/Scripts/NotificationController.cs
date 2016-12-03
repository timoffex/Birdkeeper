using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NotificationController : MonoBehaviour {


	public NotificationObject notificationPrefab;


	public void ShowNotification (string text, float duration = 2) {
		var notification = GameObject.Instantiate (notificationPrefab, transform) as NotificationObject;
		notification.ShowTextForTime (text, duration);
	}

	
}
