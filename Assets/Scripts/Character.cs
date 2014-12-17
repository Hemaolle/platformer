using UnityEngine;
using System.Collections;

/// <summary>
/// Character is something that can take damage. For example
/// the player, a mob or a boss.
/// </summary>
public class Character : MonoBehaviour {
		
	public float maxHP = 10;								
	public float invulnerabilityTime = 0.5f;				// How long the character should stay invulnerable after
															// getting hit.
	public float invulnerabilityFlashFrequency = 0.05f;		// The frequency of the flashing that indicates that the
															// character is invulnerable.

	[HideInInspector]
	public Alignment alignment = Alignment.Enemy;

	public enum Alignment
	{
		Friendly, Enemy
	};

	protected float currentHP;
	private bool invulnerable = false;

	// Use this for initialization
	public void Start () {
		currentHP = maxHP;
	}

	public virtual void TakeDamage(float damage) {
		if(!invulnerable) {
			invulnerable = true;
			currentHP -= damage;
			if (currentHP <= 0)
							Die ();
			else {
				Invoke ("stopInvulnerability", invulnerabilityTime);
				StartCoroutine ("flashInvulnerable");
			}
		}
	}

	public virtual void Die() {
		Destroy (gameObject);
	}


	private IEnumerator flashInvulnerable() {
		while (true) {
						GetComponent<SpriteRenderer> ().enabled = false;
						yield return new WaitForSeconds (invulnerabilityFlashFrequency / 2);
						GetComponent<SpriteRenderer> ().enabled = true;
						yield return new WaitForSeconds (invulnerabilityFlashFrequency / 2);
				}
	}

	private void stopInvulnerability() {
		invulnerable = false;
		StopCoroutine ("flashInvulnerable");
		GetComponent<SpriteRenderer> ().enabled = true;
	}
}
