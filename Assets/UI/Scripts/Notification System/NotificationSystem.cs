using UnityEngine;
using System;

public class NotificationSystem {

	private static NotificationController notificationController;

	public static void ShowNotificationIfPossible (string text, float timeSeconds = 2) {

		if (notificationController == null)
			notificationController = GameObject.FindObjectOfType<NotificationController> ();

		if (notificationController != null)
			notificationController.ShowNotification (text, timeSeconds);
		
	}

}

