using System;
using UnityEngine;
using UnityEngine.UI;

public class DayPhaseTimerScript : MonoBehaviour {

	public GamePhase nextPhase;
	public float maximumTime = 60;
	private float startTime;


	public Text timePassedText;

	void Start () {
		startTime = Time.time;
	}

	void Update () {
		float timeDelta = Time.time - startTime;

		if (timeDelta >= maximumTime)
			Game.current.SwitchToPhase (nextPhase);

		if (timePassedText != null)
			timePassedText.text = string.Format ("{0} / {1}", (int)timeDelta, maximumTime);
	}
}


