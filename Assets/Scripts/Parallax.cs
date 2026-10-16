using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	public Transform foreGround1;
	public Transform foreGround2;
	public Transform middleGround1;
	public Transform middleGround2;
	public float foreGroundSpeed = 0.1f;
	public float middleGroundSpeed = 0.05f;

	private float foregroundX1;
	private float foregroundX2;
	private float middleGroundX1;
	private float middleGroundX2;

	// Use this for initialization
	void Start ()
	{
		foregroundX1 = foreGround1.position.x;
		foregroundX2 = foreGround2.position.x;
		middleGroundX1 = middleGround1.position.x;
		middleGroundX2 = middleGround2.position.x;
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreGround1.position = new Vector3(foreGround1.position.x - foreGroundSpeed, foreGround1.position.y, foreGround1.position.z);
		foreGround2.position = new Vector3(foreGround2.position.x - foreGroundSpeed, foreGround2.position.y, foreGround2.position.z);
		if (foreGround2.position.x <= foregroundX1)
		{
			foreGround1.position = new Vector3(foregroundX1, foreGround1.position.y, foreGround1.position.z);
			foreGround2.position = new Vector3(foregroundX2, foreGround2.position.y, foreGround2.position.z);
		}

		middleGround1.position = new Vector3(middleGround1.position.x - middleGroundSpeed, middleGround1.position.y, middleGround1.position.z);
		middleGround2.position = new Vector3(middleGround2.position.x - middleGroundSpeed, middleGround2.position.y, middleGround2.position.z);
		if (middleGround2.position.x <= middleGroundX1)
		{
			middleGround1.position = new Vector3(middleGroundX1, middleGround1.position.y, middleGround1.position.z);
			middleGround2.position = new Vector3(middleGroundX2, middleGround2.position.y, middleGround2.position.z);
		}
	}
}
