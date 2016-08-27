using UnityEngine;
using System.Collections;

/// <summary>
/// Destros the gameobject in given amount of time.
/// </summary>
public class DestroyInSeconds : MonoBehaviour {

	public float destroyInSeconds;

	private float creationTime;

	// Use this for initialization
	void Start () {
		creationTime = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - creationTime > destroyInSeconds)
			Destroy (gameObject);
	}
}
