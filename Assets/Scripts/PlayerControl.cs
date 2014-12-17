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
	public bool jump = false;				// Condition for whether the player should jump.
	[HideInInspector]
	public bool attack = false;				// Condition for whether the player should attack.
	
	public GameObject punchObject;
	
	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	public int jumpForceTimes = 5;			// Maximum amount of FixedUpdates in which the jump force will be applied.
											// Jump force can be applied multiple times during the same jump to achieve
											// jumping higher when the jump button is pressed for a longer time.
	public float projectileSpeed = 5f;		// Speed of the the projectile of the projectile attack.
	public float groundCheckXDistance = 1f;	// How far to left and right should additional ground checks be done.
	public float walkAudioInterval = 0.3f;	// How often should the sound of footsteps be played.

	public AudioSource jumpAudio;			
	public AudioSource walkAudio;			
	public AudioSource shootAudio;

	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.	
	private bool jumpButtonReleased = true;
	
	
	private float lastWalkSound;
	private float jumpStartTime;
	private int jumpForceCount = 0;
	
	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		anim = GetComponent<Animator>();
	}
	
	
	void Update()
	{
		DoGroundCheck ();

		// If the jump button is pressed and the player is grounded then the player should jump.
		if (Input.GetButton ("Jump")) {
			jump = true;
			// If the player stands on the ground and the jump button was just pressed start a new jump.
			if(grounded && jumpButtonReleased)
			{
				jumpForceCount = 0;
				jumpAudio.Play();
				jumpButtonReleased = false;
			}
		}
		
		if (Input.GetButtonUp("Jump"))
			jumpButtonReleased = true;
		
		if(Input.GetButtonDown("Fire1"))
			attack = true;		
	}	
	
	void FixedUpdate ()
	{
		HandleWalking();		
		HandleJumping();	
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
		if (Mathf.Abs (rigidbody2D.velocity.x) > 0.1f) {
			anim.SetBool ("Running", true);
			if (grounded && Time.timeSinceLevelLoad - lastWalkSound > walkAudioInterval) {
				walkAudio.Play ();
				lastWalkSound = Time.timeSinceLevelLoad;
			}
		}
		else {
			anim.SetBool ("Running", false);
		}

		rigidbody2D.velocity = new Vector2(horizontalInput * maxSpeed, rigidbody2D.velocity.y);
	}

	void HandleJumping ()
	{
		if (grounded)
		{
			anim.SetBool ("Jump", false);
		}

		// If the jump button is released during a jump, no more jump forces can be applied.
		if (jumpForceCount > 0 && jump == false)
			jumpForceCount = jumpForceTimes;

		// Check if jump button is pressed and maximum number of jump forces hasn't been applied
		if (jump && (jumpForceCount < jumpForceTimes)) {
			// Set the Jump animator trigger parameter.
			if (!anim.GetBool ("Jump"))
				anim.SetBool ("Jump", true);
			// Add a vertical force to the player.
			rigidbody2D.AddForce (new Vector2 (0f, jumpForce));
			jump = false;
			jumpForceCount++;
		}
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
