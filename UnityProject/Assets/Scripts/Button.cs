using UnityEngine;
using System.Collections;


/// <summary>
/// A trigger that stops the crusher when the cube1 object is pushed to it.
/// </summary>
public class Button : MonoBehaviour {

	public Crusher crusher;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.name == "cube1") 
		{
			crusher.Stop();
			collider.GetComponent<Rigidbody2D>().isKinematic = true;
		}
	}
}
