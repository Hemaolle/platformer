using UnityEngine;
using System.Collections;

/// <summary>
/// Player movement and attacking.
/// </summary>
public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool attack = false;				// Condition for whether the player should attack.
	
	public GameObject punchObject;
	
	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.

	public float projectileSpeed = 5f;		// Speed of the the projectile of the projectile attack.
	public float groundCheckXDistance = 1f;	// How far to left and right should additional ground checks be done.
	public float walkAudioInterval = 0.3f;	// How often should the sound of footsteps be played.

	public AudioSource walkAudio;			
	public AudioSource shootAudio;

	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.	

	private float lastWalkSound;
	
	void Awake()
	{
		// Setting up references.
		anim = GetComponent<Animator>();
	}
	
	
	void Update()
	{
		DoGroundCheck();
		if(Input.GetButtonDown("Fire1"))
			attack = true;		
	}	
	
	void FixedUpdate ()
	{
		HandleWalking();		
		HandleAttacking();	
	}

	/// <summary>
	/// Checks if the player stands on the ground.
	/// </summary>
	void DoGroundCheck ()
	{
		Vector3 groundCheckMiddleStart = transform.position;
		Vector3 groundCheckLeftStart = transform.position - Vector3.right * groundCheckXDistance;
		Vector3 groundCheckRightStart = transform.position + Vector3.right * groundCheckXDistance;
		Vector3 groundCheckMiddleEnd = groundCheck.position;
		Vector3 groundCheckLeftEnd = groundCheck.position - (Vector3.right * groundCheckXDistance);
		Vector3 groundCheckRightEnd = groundCheck.position + (Vector3.right * groundCheckXDistance);

		// The player is grounded if three linecasts from player position to the groundcheck position hits anything on
		// the ground layer.
		grounded = Physics2D.Linecast (groundCheckMiddleStart, groundCheckMiddleEnd, 
		                               1 << LayerMask.NameToLayer ("Ground")) 
			|| Physics2D.Linecast (groundCheckLeftStart, groundCheckLeftEnd, 1 << LayerMask.NameToLayer ("Ground")) 
			|| Physics2D.Linecast (groundCheckRightStart, groundCheckRightEnd, 1 << LayerMask.NameToLayer ("Ground"));
		Debug.DrawRay (groundCheckMiddleStart, groundCheckMiddleEnd - groundCheckMiddleStart);
		Debug.DrawRay (groundCheckLeftStart, groundCheckLeftEnd - groundCheckLeftStart);
		Debug.DrawRay (groundCheckRightStart, groundCheckRightEnd - groundCheckRightStart);
	}

	void HandleWalking ()
	{
		// Cache the horizontal input.
		float horizontalInput = Input.GetAxis("Horizontal");
		
		// If the input is moving the player right and the player is facing left...
		if(horizontalInput > 0 && !facingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(horizontalInput < 0 && facingRight)
			// ... flip the player.
			Flip();

		// The Speed animator parameter is set to the absolute value of the horizontal input.
		if (Mathf.Abs (GetComponent<Rigidbody2D>().velocity.x) > 0.1f) {
			anim.SetBool ("Running", true);
			if (grounded && Time.timeSinceLevelLoad - lastWalkSound > walkAudioInterval) {
				walkAudio.Play ();
				lastWalkSound = Time.timeSinceLevelLoad;
			}
		}
		else {
			anim.SetBool ("Running", false);
		}

		GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalInput * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
	}
	
	void HandleAttacking() {
		if(attack)
		{
			GameObject newProjectile = (GameObject) Instantiate (punchObject, transform.position, Quaternion.identity);
			newProjectile.GetComponent<Rigidbody2D>().velocity = facingRight ? new Vector2(projectileSpeed,0) 
				: new Vector2(-projectileSpeed,0);
			if (facingRight)
				newProjectile.transform.Find ("Sprite").GetComponent<Spin> ().angularVelocity *= -1; 
			attack = false;
			shootAudio.Play();
		}
	}
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
