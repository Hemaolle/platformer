using UnityEngine;
using System.Collections;

/// <summary>
/// Sets the target FPS. For testing with different FPSs.
/// </summary>
public class TargetFPS : MonoBehaviour {

	public int targetFPS = 60;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = targetFPS;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
