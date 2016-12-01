using UnityEngine;
using System.Collections;

public class SwitchPhaseScript : MonoBehaviour {


	public GamePhase phase;

	public void SwitchToPhase () {
		Game.current.SwitchToPhase (phase);
	}
}
