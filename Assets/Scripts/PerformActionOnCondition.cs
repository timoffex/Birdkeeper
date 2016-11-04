using UnityEngine;
using System.Collections;

public class PerformActionOnCondition : MonoBehaviour {

	public delegate void Action ();
	public delegate bool Condition ();

	private Action action;
	private Condition condition;

	public PerformActionOnCondition (Condition cond, Action act) {
		action = act;
		condition = cond;
	}

	// Update is called once per frame
	void Update () {
		if (condition ())
			action ();
	}
}
