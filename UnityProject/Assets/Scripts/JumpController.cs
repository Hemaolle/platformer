using UnityEngine;
using System.Collections;

public class JumpController : MonoBehaviour {

	public float jumpForce = 1000f;
	public int maxForceAddTimes = 5;
	public Animator anim;
	public AudioSource jumpAudio;			
	public PlayerControl playerControl;

	private bool jumpButtonPressed = false;
	private Rigidbody2D rigidbody;
	private bool wasJumpButtonReleasedInBetween = true;

	private IJumpState currentJumpState;

	private IJumpState groundedState = new Grounded();
	private IJumpState airbornAddingForceState = new AirbornAddingForce();
	private IJumpState airbornNotAddingForceState = new AirbornNotAddingForce();

	private void Awake()
	{
		currentJumpState = groundedState;
		rigidbody = GetComponent<Rigidbody2D>();
	}

    private void Update() {
        StoreInput();
    }

    private void StoreInput()
    {
        jumpButtonPressed = Input.GetButton("Jump");
        if (Input.GetButtonUp("Jump"))
            wasJumpButtonReleasedInBetween = true;
    }

	private void FixedUpdate() {
		currentJumpState.ProcessInputs(playerControl.Grounded, jumpButtonPressed, this);
        ResetInput();
	}

    private void ResetInput()
    {
        jumpButtonPressed = false;
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
}
