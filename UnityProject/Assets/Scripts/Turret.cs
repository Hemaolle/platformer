using UnityEngine;
using System.Collections;

/// <summary>
/// A mob that shoots projectiles if it detects that the player is directly in front of it.
/// </summary>
public class Turret : Mob {

	public GameObject projectile;			// The projectile for the projectile attack.
	public float projectileSpeed = 10f;		// The speed of the projeticle of the projectile attack.
	public float detectDistance = 10f;		// The Turret will start shooting if the player is directly in front of the
											// Turret and closer than the detect distance.

	public float firstAttackWait = 0.5f;	// Determines how long to wait before shooting when the player is detected.
	public float attackInterval = 2f;		// Determines how long to wait between shots.

	private Vector2 lookDirection;			// Left of right.
	private bool playerInSight = false;		// Indicates if the player has been detected.

	new void Start() {
		base.Start();

	}

	// Update is called once per frame
	void Update () {
		DetectPlayer ();
	}

	void DetectPlayer ()
	{
		// Raycast will stop if it hits the player or the ground layer.
		int layerMask = (1 << LayerMask.NameToLayer ("Ground")) | (1 << LayerMask.NameToLayer ("Player"));
		Debug.DrawRay (transform.position, (facingRight ? Vector2.right : -Vector2.right) * detectDistance, Color.red);
		RaycastHit2D hit = Physics2D.Raycast (transform.position, facingRight ? Vector2.right : -Vector2.right,
		                                      detectDistance, layerMask);

		if (hit.transform != null && hit.transform.gameObject.layer == LayerMask.NameToLayer ("Player")) {
			if (!playerInSight) {
				playerInSight = true;
				InvokeRepeating ("Attack", firstAttackWait, attackInterval);
			}
		}
		else {
			if (playerInSight) {
				playerInSight = false;
				CancelInvoke ("Attack");
			}
		}
	}

	void Attack()
	{
		GameObject newProjectile = (GameObject) Instantiate (projectile, transform.position, Quaternion.identity);
		newProjectile.GetComponent<Rigidbody2D>().velocity = facingRight ? new Vector2(projectileSpeed,0) 
			: new Vector2(-projectileSpeed,0);
		if (facingRight)
		{
			Vector3 flippedScale = newProjectile.transform.localScale;
			flippedScale.x *= -1;
			newProjectile.transform.localScale = flippedScale;
   		}
	}
}
