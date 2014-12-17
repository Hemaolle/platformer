using UnityEngine;
using System.Collections;

/// <summary>
/// Destroys the gameobject when it collides with the target.
/// </summary>
public class DestroyWhenHittingTarget : MonoBehaviour {

	public GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D other) {
		GameObject collision = other.gameObject;
		if (target.gameObject.GetInstanceID() == collision.GetInstanceID()) 
		{
			Destroy(gameObject);
		}
	}
}
