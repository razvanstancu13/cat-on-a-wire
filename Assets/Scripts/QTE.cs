using UnityEngine;
using System.Collections;

public class QTE : MonoBehaviour {
	public float qteTimer = 5;
	public PlayerController player = null;
	bool solved = false;
	public GameObject qtePrompt = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Timer ();
		if (qteTimer <= 0)
			GameLogic.instance.OnPlayerDead ();
		if (solved)
			Destroy (this.gameObject);

	}

	void Timer () 
	{
		float timeStep = Time.deltaTime;
		qteTimer -= timeStep;
		if (qteTimer <= 0)
			qteTimer = 0;
	}
}
