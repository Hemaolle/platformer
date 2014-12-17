using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the player damage taking and dying.
/// </summary>
public class Player : Character {

	public Transform HPbar;	
	private Vector3 originalHPBarScale;	
	protected bool alive = true;

	public AudioSource hurtAudio;

	// Use this for initialization
	new void Start () {
		base.Start ();
		alignment = Alignment.Friendly;
		originalHPBarScale = HPbar.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	override public void TakeDamage(float damage) {
		base.TakeDamage (damage);
		HPbar.localScale = new Vector3 (originalHPBarScale.x * ((float) currentHP / maxHP), originalHPBarScale.y, originalHPBarScale.z);
		hurtAudio.Play();
	}

	override public void Die() {
		if (alive) {
			GetComponent<Animator> ().SetTrigger ("Die");
			GetComponent<PlayerControl> ().enabled = false;
			transform.Find ("Main Camera").parent = null;
			collider2D.isTrigger = true;
			rigidbody2D.velocity = new Vector2(0, 10);
			alive = false;
			hurtAudio.Play();
			Invoke("RestartLevel", 2);
		}
	}

	void RestartLevel() {
		Application.LoadLevel (Application.loadedLevel);
	}
}
