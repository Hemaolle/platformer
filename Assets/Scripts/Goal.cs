using UnityEngine;
using System.Collections;

/// <summary>
/// Loads the next level when reached by the player.
/// </summary>
public class Goal : MonoBehaviour {

	public string nextLevel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Player")		
			Application.LoadLevel (nextLevel);
		
	}
}
