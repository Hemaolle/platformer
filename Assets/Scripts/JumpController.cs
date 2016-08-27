using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour {

	public float groundCheckXDistance = 1f;	// How far to left and right should additional ground checks be done.
	public Transform groundCheck;
	public float jumpForce = 1000f;
	public int maxForceAddTimes = 5;
	public Animator anim;
	public AudioSource jumpAudio;			

	private bool jumpButtonPressed = false;
	private bool grounded = true;
	private Rigidbody2D rigidbody;
	private bool wasJumpButtonReleasedInBetween = true;

	IJumpState currentJumpState;

	IJumpState groundedState = new Grounded();
	IJumpState airbornAddingForceState = new AirbornAddingForce();
	IJumpState airbornNotAddingForceState = new AirbornNotAddingForce();

	private void Awake()
	{
		currentJumpState = groundedState;
		rigidbody = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate() {
		jumpButtonPressed = Input.GetButton("Jump");
		grounded = DoGroundCheck();
		if (Input.GetButtonUp("Jump"))
			wasJumpButtonReleasedInBetween = true;
		currentJumpState.ProcessInputs(grounded, jumpButtonPressed, this);
	}

	private void AddJumpForce() {
		rigidbody.AddForce(new Vector2(0f, jumpForce));
	}

	interface IJumpState
	{
		void ProcessInputs(bool grounded, bool jumpButtonPressed, JumpController jumpController);
	}

	private class Grounded : IJumpState
	{
		public void ProcessInputs (bool grounded, bool jumpButtonPressed, JumpController jumpController)
		{
			if (jumpButtonPressed &&				
				jumpController.wasJumpButtonReleasedInBetween)
			{
				jumpController.anim.SetBool("Jump", true);
				jumpController.jumpAudio.Play();
				jumpController.AddJumpForce();
				jumpController.currentJumpState = jumpController.airbornAddingForceState;
				jumpController.wasJumpButtonReleasedInBetween = false;
			}
			else {
				jumpController.anim.SetBool("Jump", false);
			}
		}
	}

	private class AirbornAddingForce : IJumpState
	{
		private int forceAddedCount;

		public void ProcessInputs (bool grounded, bool jumpButtonPressed, JumpController jumpController)
		{
			Debug.Log (forceAddedCount + " maxForceAddTimes " + jumpController.maxForceAddTimes);
			// The player can quickly get grounded if the ceiling is low for example
			if (grounded && jumpController.rigidbody.velocity.y <= 0)
			{
				SetJumpControllerState(jumpController, jumpController.groundedState);
			}
			else if (jumpButtonPressed && forceAddedCount < jumpController.maxForceAddTimes) {				
				forceAddedCount++;
				jumpController.AddJumpForce();
			}
			else {
				SetJumpControllerState(jumpController, jumpController.airbornNotAddingForceState);
			}
		}

		private void SetJumpControllerState(JumpController jumpController, IJumpState newState)
		{
			forceAddedCount = 0;
			jumpController.currentJumpState = newState;
		}
	}

	private class AirbornNotAddingForce : IJumpState {
		
		public void ProcessInputs (bool grounded, bool jumpButtonPressed, JumpController jumpController)
		{
			if (grounded && jumpController.rigidbody.velocity.y <= 0) {
				jumpController.currentJumpState = jumpController.groundedState;
			}
		}
	}

	/// <summary>
	/// Checks if the player stands on the ground.
	/// </summary>
	private bool DoGroundCheck ()
	{
		Vector3 groundCheckMiddleStart = transform.position;
		Vector3 groundCheckLeftStart = transform.position - Vector3.right * groundCheckXDistance;
		Vector3 groundCheckRightStart = transform.position + Vector3.right * groundCheckXDistance;
		Vector3 groundCheckMiddleEnd = groundCheck.position;
		Vector3 groundCheckLeftEnd = groundCheck.position - (Vector3.right * groundCheckXDistance);
		Vector3 groundCheckRightEnd = groundCheck.position + (Vector3.right * groundCheckXDistance);

		// The player is grounded if three linecasts from player position to the groundcheck position hits anything on
		// the ground layer.
		bool grounded = Physics2D.Linecast (groundCheckMiddleStart, groundCheckMiddleEnd, 
			1 << LayerMask.NameToLayer ("Ground")) 
			|| Physics2D.Linecast (groundCheckLeftStart, groundCheckLeftEnd, 1 << LayerMask.NameToLayer ("Ground")) 
			|| Physics2D.Linecast (groundCheckRightStart, groundCheckRightEnd, 1 << LayerMask.NameToLayer ("Ground"));
		Debug.DrawRay (groundCheckMiddleStart, groundCheckMiddleEnd - groundCheckMiddleStart);
		Debug.DrawRay (groundCheckLeftStart, groundCheckLeftEnd - groundCheckLeftStart);
		Debug.DrawRay (groundCheckRightStart, groundCheckRightEnd - groundCheckRightStart);

		return grounded;
	}
}
