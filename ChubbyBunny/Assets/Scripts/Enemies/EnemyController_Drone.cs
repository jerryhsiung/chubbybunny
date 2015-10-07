using UnityEngine;
using System.Collections;

public class EnemyController_Drone : EnemyController_Base {

	public GameObject bomb;
	public GameObject shotDownEffect;
	public GameObject crashedEffect;

	public AudioClip crashedSoundEffect;
	public GameObject soundBox;

	public float attackDamage;
	public float attackRadius;

	public float flySpeed;
	public float flyAcceleration;
	public float slowAcceleration;
	public float rotateSpeed;
	public float slowDistance;

	public float attackSpeed;
	private float attackTimer;

	private float currVelocity;
	private float currIdleTime;
	
	private Vector3 currDestination;
	private Quaternion dirRotation;
	private Vector3 direction;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		currVelocity = currIdleTime = attackTimer = 0;
		currDestination = findRandomWanderPosition ();
	}

	// Update is called once per frame
	void Update () {
		attackTimer += Time.deltaTime;

		//	Use WANDER for rotating to face destination
		if (currState == EnemyState.WANDER) {
			currIdleTime += Time.deltaTime;
			if(currIdleTime >= idleTime) {
				//	Get vector to destination and rotation needed to face it
				direction = (currDestination - this.transform.position).normalized;
				dirRotation = Quaternion.LookRotation (direction);

				//	Slerp for smoothness
				this.transform.rotation = Quaternion.Slerp (transform.rotation, dirRotation,
				                                           Time.deltaTime * rotateSpeed);

				//	Check if facing destination
				if (Quaternion.Angle (this.transform.rotation, dirRotation) < 2f) {
					this.transform.LookAt (currDestination);
					currIdleTime = 0;

					//	Move towards destination
					animator.SetTrigger ("Forward");
					currState = EnemyState.CHASE;
				}
			}
		}
		//	Use CHASE for movement
		else if (currState == EnemyState.CHASE) {
			//	Always look at destination
			this.transform.LookAt(currDestination);

			currVelocity += flyAcceleration * Time.deltaTime;
			currVelocity = Mathf.Min (currVelocity, flySpeed);
			this.transform.position = Vector3.MoveTowards (this.transform.position,
			                                              currDestination,
			                                              currVelocity * Time.deltaTime);

			//	Check if within slowing distance
			if (Vector3.Distance (this.transform.position, currDestination) <= slowDistance) {
				animator.SetTrigger ("Slow");
				currState = EnemyState.ATTACK;
			}
		}
		//	Use ATTACK for slowing down
		else if (currState == EnemyState.ATTACK) {
			currVelocity -= slowAcceleration * Time.deltaTime;
			currVelocity = Mathf.Max(currVelocity, 0.5f);
			this.transform.position = Vector3.MoveTowards(this.transform.position,
			                                              currDestination,
			                                              currVelocity * Time.deltaTime);

			if(reachedDestination(currDestination)) {
				animator.SetTrigger("Idle");
				currDestination = findRandomWanderPosition ();
				currState = EnemyState.WANDER;
			}
		}
	}

	//	Special killed function, called by player projectile
	public void killed() {
		if (currState != EnemyState.DYING) {
			Instantiate (shotDownEffect, this.transform.position, Quaternion.identity);
			animator.SetTrigger ("Death");
			currState = EnemyState.DYING;

			//	Fall out of the sky
			this.rigidbody.isKinematic = false;
			this.rigidbody.useGravity = true;
			this.rigidbody.AddForce (this.transform.forward * 500f);
		}
	}

	void OnCollisionEnter(Collision col) {
		if (currState == EnemyState.DYING && col.gameObject.tag != "Attack") {
			//	It crashed
			Instantiate(crashedEffect, this.transform.position, Quaternion.identity);

			//	Play sound effect
			GameObject box = Instantiate(soundBox, this.transform.position, Quaternion.identity) as GameObject;
			box.audio.clip = crashedSoundEffect;
			box.audio.Play();

			currState = EnemyState.DEAD;
		}
	}

	void OnTriggerStay(Collider col) {
		if (col.gameObject.tag == "Player" && attackTimer >= attackSpeed) {
			//	Drop a bomb
			GameObject drop = Instantiate (bomb, this.transform.position, Quaternion.identity) as GameObject;

			//	Set attributes
			BombScript script = drop.GetComponent<BombScript> ();
			script.damage = this.attackDamage;
			script.damageRadius = this.attackRadius;

			//	Give it some forward momentum if not stationary
			if(currState != EnemyState.WANDER)
				drop.rigidbody.AddForce(this.transform.forward * 200f);

			attackTimer = 0;
		}
	}
}
