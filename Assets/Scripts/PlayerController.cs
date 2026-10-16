using UnityEngine;
using System.Collections;

public enum PlayerState : int {None = 0, Aggressive, Hysteric, Ecstatic};
public enum PlayerAbility : int {None = 0, Punch, Growl, SuperJump};

public class PlayerController : MonoBehaviour
{
	public GameObject projectileArchetype = null;
	//public GameObject qteArchetype = null;
	public bool autoMove = true;
	
	public bool crouching = false;
	
	public int coins = 0;
	public int bullets = 1;
	// sounds
	public AudioClip soundJump;
	public AudioClip soundCollectPill;
	public AudioClip soundPunch;
	public AudioClip soundGrawl;
	public AudioClip soundStep;
	public AudioClip soundDeath;
	
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
	public float fallLimit = -30f;    // below this the cat has fallen off (the lowest ground is at about -10)

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;
	
	private GameLogic _gameLogic;
	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;
	private AudioSource _audioSource;
	private BalanceBehavior _balanceBehavior;
	//private BoxCollider2D _collider;
	
	// input
	private bool _right;
	private bool _left;
	private bool _up;
	private bool _down;
	private bool _action;
	private bool _consume;
	private bool _secondaryAction;
	private bool _balanceUp;
	private bool _balanceDown;
	private bool _fireCannon;
	private bool _fell;
	
	void Awake()
	{
		_gameLogic = GameObject.Find ("GameManager").GetComponent<GameLogic>();
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
		_audioSource = GetComponent<AudioSource>();
		_balanceBehavior = GameObject.Find ("BalanceControl").GetComponentInChildren<BalanceBehavior>();
		
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
	}
	
	void Start() 
	{
	}
	
	void onControllerCollider( RaycastHit2D hit )
	{
	
		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
//		if (hit.collider.gameObject.tag == "Damage")
//		{
//			if (!inAction || _currentAbility != PlayerAbility.Punch)
//			{
//				_gameLogic.OnPlayerDead();
//			}
//			else if (_currentAbility == PlayerAbility.Punch)
//			{
//				//Debug.Log( "Killed enemy : " + hit.collider.gameObject.name);
//				Destroy (hit.collider.gameObject);
//			}
//		}
//		else if (hit.collider.gameObject.name == "Coin")
//		{
//			_gameLogic.OnCoinCollected();
//			Destroy(hit.collider.gameObject);
//		}
	}

	void CoinCounter ()
	{
		if (coins >= 20)
		{
			coins -= 20;
			bullets += 1;
		}
	}

	void onTriggerEnterEvent( Collider2D col )
	{
		string _tag = col.gameObject.tag;
		if (_tag == "Damage" || _tag == "Destroyable")
		{
			_gameLogic.OnPlayerDead();
		}
		else if (_tag == "Collectables")
		{
			coins++;
			Destroy(col.gameObject);
		}
		else if (_tag == "MustCrouch")
		{
			if (!crouching)
				_gameLogic.OnPlayerDead();
		}
		else if (_tag == "QTE")
		{
			QTE newQTE = new QTE();
			newQTE.player = this;
			newQTE.qteTimer = 3;
		}
	}
	
	void onTriggerExitEvent( Collider2D col )
	{
		string _tag = col.gameObject.tag;
		if (_tag == "MustCrouch")
		{
			if (!crouching)
				_gameLogic.OnPlayerDead();
		}
	}
	
	// the Update loop only gathers input. Actual movement is handled in FixedUpdate because we are using the Physics system for movement
	void Update()
	{
		// the level had no check for falling through a gap: the cat fell for ever; now it restarts at the checkpoint
		if (transform.position.y < fallLimit && !_fell)
		{
			_fell = true;
			_gameLogic.OnPlayerDead();
		}
		CoinCounter ();
		_fireCannon = _fireCannon || Input.GetKeyDown (KeyCode.Space);
		if (_fireCannon.Equals (true) && bullets > 0)
		{
			GameObject _newProjectile = (GameObject)Instantiate (projectileArchetype, new Vector3(this.transform.position.x, this.transform.position.y - 0.65f, this.transform.position.z), Quaternion.identity);
			_newProjectile.GetComponent<Projectile>().thePlayer = this.gameObject;
			_fireCannon = false;
			bullets -= 1;
		}
		// a minor bit of trickery here. FixedUpdate sets _up to false so to ensure we never miss any jump presses we leave _up
		// set to true if it was true the previous frame
		if (Input.GetKeyDown(KeyCode.Escape))
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		
		_up = _up || Input.GetKeyDown( KeyCode.W );
		if (autoMove)
		{
			_right = true;
			_left = false;
		}
		else
		{
			_right = Input.GetKey( KeyCode.D );
			_left = Input.GetKey( KeyCode.A );
		}
		_down = Input.GetKey ( KeyCode.S );
		_action = Input.GetKey( KeyCode.Space);
		_consume = Input.GetKey(KeyCode.Return);
		_secondaryAction = Input.GetKey(KeyCode.LeftControl);	
		
		_balanceUp = Input.GetKeyDown(KeyCode.P);
		_balanceDown = Input.GetKeyDown(KeyCode.L);
		
	}
	
	void FixedUpdate()
	{
		float orientation = transform.localScale.x < 0f ? -1f : 1f;
		
		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;
		
		//Debug.Log (_velocity.x + " @@@ " + _velocity.y);
		
		if (_balanceUp)
			_balanceBehavior.ApplyUserInput(true);
		else if (_balanceDown)
			_balanceBehavior.ApplyUserInput(false);
		
		if( _controller.isGrounded )
			_velocity.y = 0;
		
		if (_down && !crouching)
		{
			crouching = true;
//			BoxCollider2D collider = this.GetComponent<BoxCollider2D>();
//			collider.size = new Vector2(10,4);
//			collider.center = new Vector2(0,-3);
//			this.GetComponent<CharacterController2D>().recalculateDistanceBetweenRays();
//			this.GetComponent<Rigidbody2D>().velocity = _velocity;
		}
		else if (!_down &&  crouching)
		{
			crouching = false;	
//			BoxCollider2D collider = this.GetComponent<BoxCollider2D>();
//			collider.size = new Vector2(5.58f,10.25f);
//			collider.center = new Vector2(0.8f,0);
//			this.GetComponent<CharacterController2D>().recalculateDistanceBetweenRays();
//			this.GetComponent<Rigidbody2D>().velocity = _velocity;
		}
		
		if( _right )
		{
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
			
			if( _controller.isGrounded)
			{
				if (_down)
				{
					_animator.Play( Animator.StringToHash( "CatCrouch" ) );
				}
				else
				{
					_animator.Play( Animator.StringToHash( "CatRun" ) );
				}
				//_audioSource.PlayOneShot(soundStep, 1f);
			}
		}
		else if( _left )
		{
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
			
			if( _controller.isGrounded)
			{
				if (_down)
				{
					_animator.Play( Animator.StringToHash( "CatCrouch" ) );
				}
				else
				{
					_animator.Play( Animator.StringToHash( "CatRun" ) );
				}
				//_audioSource.PlayOneShot(soundStep, 1f);
			}
		}
		else
		{
			normalizedHorizontalSpeed = 0;
			
			if( _controller.isGrounded)
				_animator.Play( Animator.StringToHash( "CatIdle" ) );
		}
		
		
		// we can only jump whilst grounded
		if( _controller.isGrounded && _up )
		{
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
			_animator.Play( Animator.StringToHash( "CatJump" ) );
			_audioSource.PlayOneShot(soundJump, 1f);
			
		}
			
		// apply horizontal speed smoothing it
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.fixedDeltaTime * smoothedMovementFactor );
		
		// apply gravity before moving
		_velocity.y += gravity * Time.fixedDeltaTime;
		
		_controller.move( _velocity * Time.fixedDeltaTime );
		
		// reset input
		_up = false;
		
	}
}
