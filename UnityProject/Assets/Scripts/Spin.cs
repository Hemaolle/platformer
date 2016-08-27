using UnityEngine;
using System.Collections;

/// <summary>
/// Spins an object around the z axis.
/// </summary>
public class Spin : MonoBehaviour {

	public float angularVelocity = 1;

	private float currentAngle;

	// Use this for initialization
	void Start () {
		currentAngle = transform.localRotation.eulerAngles.z;
	}
	
	// Update is called once per frame
	void Update () {
		currentAngle += Time.deltaTime * angularVelocity;
		transform.localRotation = Quaternion.Euler (transform.localRotation.x, transform.localRotation.y, currentAngle);
	}
}
