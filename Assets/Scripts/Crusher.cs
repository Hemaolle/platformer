using UnityEngine;
using System.Collections;

/// <summary>
/// A crusher that moves up and down.
/// </summary>
public class Crusher : MonoBehaviour {

	public float comeDownSpeed;
	public float goUpSpeed;
	public float timeWaitingUp;			// Determines how long to stay at up position.
	public float timeWaitingDown;		// Determines how long to stay at down position.

	public GameObject downPosition;		// Position to stop at when coming down.

	public enum State { WaitingUp, GoingDown, WaitingDown, GoingUp };

	private State state;
	private Vector2 originalPosition;
	private Vector2 originalDownPosition;

	// Use this for initialization
	void Start () {
		originalPosition = transform.position;
		originalDownPosition = downPosition.transform.position;
		StartCoroutine("WaitUp");
	}
	
	// Update is called once per frame
	void Update () {
		if (state == State.GoingDown && transform.position.y < originalDownPosition.y)
			StartCoroutine("WaitDown");
		if (state == State.GoingUp && transform.position.y > originalPosition.y)
			StartCoroutine("WaitUp");
	}

	IEnumerator WaitUp() {
		rigidbody2D.velocity = new Vector2 (0, 0);
		state = State.WaitingUp;
		yield return new WaitForSeconds(timeWaitingUp);
		MoveDown ();
	}

	void MoveDown() {
		state = State.GoingDown;
		rigidbody2D.velocity = new Vector2 (0, -comeDownSpeed);
	}

	IEnumerator WaitDown() {
		rigidbody2D.velocity = new Vector2 (0, 0);
		state = State.WaitingUp;
		yield return new WaitForSeconds(timeWaitingDown);
		MoveUp ();
	}

	void MoveUp() {
		state = State.GoingUp;
		rigidbody2D.velocity = new Vector2 (0, goUpSpeed);
	}

	public void Stop() {
		rigidbody2D.velocity = Vector2.zero;
		StopCoroutine("WaitDown");
		StopCoroutine("WaitUp");
		enabled = false;
	}
}
