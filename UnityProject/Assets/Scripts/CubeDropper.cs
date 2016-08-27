using UnityEngine;
using System.Collections;

/// <summary>
/// Drops cubes between given intervals.
/// </summary>
public class CubeDropper : MonoBehaviour {

	public GameObject cube;
	public GameObject destroyCubeCollider;

	public float dropInterval = 4;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("DropCube", dropInterval, dropInterval);
	}

	void DropCube() {
		GameObject newCube = (GameObject)Instantiate (cube, transform.position, Quaternion.identity);
		newCube.GetComponent<DestroyWhenHittingTarget> ().target = destroyCubeCollider;
	}
}
