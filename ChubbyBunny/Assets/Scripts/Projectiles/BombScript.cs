using UnityEngine;
using System.Collections;

public class BombScript : MonoBehaviour {

	public GameObject explosion;
	public GameObject soundBox;
	public AudioClip splodeSound;

	public float damageRadius;
	public float damage;

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Drone")
			return;

		//	Blow up!
		Instantiate (explosion, this.transform.position, Quaternion.identity);
		GameObject box = Instantiate (soundBox, this.transform.position, Quaternion.identity) as GameObject;
		box.audio.clip = splodeSound;
		box.audio.volume = 0.6f;
		box.audio.Play ();

		//	Damage everything in a radius
		Collider[] hits = Physics.OverlapSphere (this.transform.position, damageRadius);
		for (int i = 0; i < hits.Length; ++i) {
			//	Skip drone if somehow this happens
			if(hits[i].gameObject.tag == "Drone")
				continue;

			//	Creatures and the player
			if(hits[i].gameObject.layer == LayerMask.NameToLayer("Creatures"))
				hits[i].gameObject.GetComponent<EnemyController_Base>().takeDamage(damage);
			else if(hits[i].gameObject.tag == "Player") {
				hits[i].gameObject.GetComponent<PlayerController>().takeDamage(damage, 0);

				//	Make camera shake
				Camera.main.GetComponent<CameraShake>().shake (0.1f);
			}
		}

		Destroy (this.gameObject);
	}
}
