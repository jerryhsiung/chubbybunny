using UnityEngine;
using System.Collections;

public class EnemyController_Slime : EnemyController_Base {

	public AudioClip slimeDamageSound;

	public float attackRange = 3f;
	public float attackSpeed = 1f;
	public float attackDamage = 2f;

	public float walkSpeed = 2f;
	public float chaseSpeed = 4f;

	private Vector3 currDestination;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		agent.stoppingDistance = attackRange - 0.5f;

		currDestination = findRandomWanderPosition ();
		goToDestination ();
	}
	
	// Update is called once per frame
	void Update () {

		//	Just wandering around
		if (currState == EnemyState.WANDER) {
			agent.speed = walkSpeed;
			if (reachedDestination (currDestination)) {
				agent.SetDestination(this.transform.position);	//	Stop the agent
				currDestination = findRandomWanderPosition ();

				//	Go to next destination after waiting a bit
				animator.SetBool("Wait", true);
				Invoke ("goToDestination", idleTime);
			}

			//	Check if noticed player
			if(Vector3.Distance(player.transform.position, this.transform.position) < detectionRadius)
				currState = EnemyState.CHASE;
		}

		//	Chasing player
		if(currState == EnemyState.CHASE) {
			agent.speed = chaseSpeed;
			agent.SetDestination(player.transform.position);

			//	Within attacking distance
			if(Vector3.Distance(player.transform.position, this.transform.position) < attackRange) {
				currState = EnemyState.ATTACK;
				InvokeRepeating("playAttack", 0, attackSpeed);
			}

			//	Check if player out of range
			if(Vector3.Distance(player.transform.position, this.transform.position) > detectionRadius) {
				currState = EnemyState.WANDER;

				currDestination = this.transform.position;
			}
		}

		//	Attack!
		if (currState == EnemyState.ATTACK) {
			//	Check if out of attack range
			if(Vector3.Distance(player.transform.position, this.transform.position) > attackRange) {
				currState = EnemyState.CHASE;

				CancelInvoke();
			}
		}
	}

	//	Called by the animation
	void dealDamage() {
		audio.PlayOneShot(slimeDamageSound);
		damagePlayer (attackDamage, 0);
	}

	void goToDestination() {
		animator.SetBool ("Wait", false);
		agent.SetDestination (currDestination);
	}

	void playAttack() {
		animator.SetTrigger ("Attack");
	}
}
