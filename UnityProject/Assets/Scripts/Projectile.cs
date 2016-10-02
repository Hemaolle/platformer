using UnityEngine;
using System.Collections;

/// <summary>
/// Damages characters of the opposite alignment on collision.
/// </summary>
public class Projectile : MonoBehaviour {

	public float damage = 1;
	public Character.Alignment alignment = Character.Alignment.Friendly;
    public bool destroyWhenHitsGround = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		Character target = collider.gameObject.GetComponent<Character>();
		if (target != null && target.alignment != alignment) 
		{
			target.TakeDamage(damage);
			Destroy(gameObject);
		}
        if(destroyWhenHitsGround && collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
			Destroy(gameObject);
	}
}
