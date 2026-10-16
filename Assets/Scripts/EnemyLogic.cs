using UnityEngine;
using System.Collections;

public enum MoveDirection : int {Left = 0, Right};

public class EnemyLogic : MonoBehaviour
{
	// movement config
	public float moveSpeed = 0.01f;
	public float moveRadius = 5f;
	public Vector3 _velocity;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;
	private float initialPositionX;
	private MoveDirection direction;

	
	private Animator _animator;

	void Awake()
	{
		_animator = GetComponent<Animator>();
		direction = MoveDirection.Left;
		normalizedHorizontalSpeed = 1;
	}

	void Start()
	{
		initialPositionX = transform.position.x;
	}
	
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.name == "Growl")
		{
			Destroy(coll.gameObject);
			_animator.Play(Animator.StringToHash("EskimoIdle"));
		}
	}	

	void Update()
	{
		if (direction == MoveDirection.Right)
		{
			if (transform.localScale.x > 0f)
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			
			_animator.Play(Animator.StringToHash("DogPatrol"));
			if (transform.position.x < initialPositionX - moveRadius)
			{
				direction = MoveDirection.Left;
				normalizedHorizontalSpeed = 1;
			}
		}
		else
		{
			if (transform.localScale.x < 0f)
				transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			
			_animator.Play(Animator.StringToHash("DogPatrol"));
			if (transform.position.x > initialPositionX + moveRadius)
			{
				direction = MoveDirection.Right;
				normalizedHorizontalSpeed = -1;
			}
		}
		
		transform.position = new Vector3(transform.position.x + normalizedHorizontalSpeed * moveSpeed, transform.position.y, 0f);
	}
}
