using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public float projectileSpeed = 0f;
	public GameObject thePlayer = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.transform.Translate (Vector2.right * Time.smoothDeltaTime * projectileSpeed);
		this.transform.RotateAround (this.transform.position, Vector2.right, projectileSpeed * Time.smoothDeltaTime);
		if (thePlayer != null)
			if (Vector3.Distance (thePlayer.transform.position, this.transform.position) >= 20)
			{
				Destroy(this.gameObject);
			}
	}

	void OnTriggerEnter2D (Collider2D c) 
	{
		switch (c.tag)
		{
		case "Destroyable" : 
		{
			Destroy (this.gameObject);
			Destroy (c.gameObject);
			break;
		}
		}
	}
	void OnCollisionEnter2D (Collision2D c)
	{
		switch (c.gameObject.tag)
		{
		case "Destroyable" : 
		{
			Destroy (this);
			Destroy (c.gameObject);
			break;
		}
		}
	}
}
