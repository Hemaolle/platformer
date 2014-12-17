using UnityEngine;
using System.Collections;

/// <summary>
/// A basic enemy. Deals damage to the player if he collides with it.
/// </summary>
public class Mob : Character {

	public float collisionDamage = 1f;
	public bool facingRight;
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D other) {
		Character target = other.gameObject.GetComponent<Character> ();
		if (target != null && target.alignment == Alignment.Friendly) 
		{
			target.TakeDamage(collisionDamage);
		}
	}
}
