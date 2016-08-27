using UnityEngine;
using System.Collections;

/// <summary>
/// Kills the player instantly if he collides with it.
/// </summary>
public class InstantKill : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Player")		
			coll.gameObject.GetComponent<Character>().Die();
		
	}
}
