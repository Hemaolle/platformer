using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour {

	public float groundCheckXDistance = 1f;	// How far to left and right should additional ground checks be done.
	public Transform groundCheck;
	public float jumpForce = 1000f;
	public int maxForceAddTimes = 5;
	public Animator anim;
	public AudioSource jumpAudio;			

	bool jumpButtonPressed = false;
	bool grounded = true;

	IJumpState currentJumpState;

	IJumpState groundedState = new Grounded();
	IJumpState airbornAddingForceState = new AirbornAddingForce();
	IJumpState airbornNotAddingForceState = new AirbornAddingForce();

	private void Awake()
	{
		currentJumpState = groundedState;
	}

	private void FixedUpdate() {
		jumpButtonPressed = Input.GetButton("Jump");
		grounded = DoGroundCheck();

		currentJumpState.ProcessInputs(grounded, jumpButtonPressed, this);
	}

	private void AddJumpForce() {
		GetComponent<Rigidbody2D>().AddForce (new Vector2 (0f, jumpForce));
	}

	interface IJumpState
	{
		void ProcessInputs(bool grounded, bool jumpButtonPressed, JumpController jumpController);
	}

	private class Grounded : IJumpState
	{
		public void ProcessInputs (bool grounded, bool jumpButtonPressed, JumpController jumpController)
		{
			if (jumpButtonPressed)
			{
				jumpController.anim.SetBool("Jump", true);
				jumpController.jumpAudio.Play();
				jumpController.AddJumpForce();
				jumpController.currentJumpState = jumpController.airbornAddingForceState;
			}
		}
	}

	private class AirbornAddingForce : IJumpState
	{
		private int forceAddedCount;

		public void ProcessInputs (bool grounded, bool jumpButtonPressed, JumpController jumpController)
		{
			// The player can quickly get grounded if the ceiling is low for example
			if (grounded)
			{
				SetJumpControllerState(jumpController, jumpController.groundedState);
			}
			else if (jumpButtonPressed) {				
				forceAddedCount++;
				if (forceAddedCount > jumpController.maxForceAddTimes)
				{
					SetJumpControllerState(jumpController, jumpController.airbornNotAddingForceState);
				}
				jumpController.AddJumpForce();
			}

			// jump button not pressed
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
			if (grounded) {
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
