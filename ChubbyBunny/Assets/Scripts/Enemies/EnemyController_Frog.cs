using UnityEngine;
using System.Collections;

public class EnemyController_Frog : EnemyController_Base {

	public AudioClip shockwaveDamageSound;
	public float shockwaveRadius;
	public float shockwaveShake;
	public float jumpSpeed;
	public float shockwaveDamage;
	public float stunTime;
	
	public float walkSpeed;
	public float jumpDistance;
	
	private Vector3 currDestination;

	private CameraShake shakeScript;
	
	// Use this for initialization
	public override void Start () {
		base.Start ();

		shakeScript = Camera.main.GetComponent<CameraShake> ();

		agent.stoppingDistance = 0.5f;
		agent.speed = walkSpeed;
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
				animator.SetBool("Idle", true);
				Invoke ("goToDestination", idleTime);
			}
			else if(animator.GetBool("Idle") == true)
				animator.SetBool("Idle", false);
			
			//	Check if noticed player
			if(Vector3.Distance(player.transform.position, this.transform.position) < detectionRadius) {
				currState = EnemyState.CHASE;

				InvokeRepeating ("jump", 0.5f, jumpSpeed);
				agent.SetDestination(player.transform.position);
				agent.speed = jumpDistance;
				agent.updatePosition = false;
			}
		}
		
		//	Chasing player
		if(currState == EnemyState.CHASE) {
			//	Keep updating destination to player position
			agent.SetDestination(player.transform.position);

			//	Check if player out of range
			if(Vector3.Distance(player.transform.position, this.transform.position) > detectionRadius) {
				currState = EnemyState.WANDER;
				CancelInvoke();

				animator.SetBool("Jump", false);
				currDestination = this.transform.position;
				agent.updatePosition = true;
			}
		}
	}

	void dealShockwaveDamage() {
		if (Vector3.Distance (player.transform.position, this.transform.position) < shockwaveRadius) {
			audio.PlayOneShot(shockwaveDamageSound, 2.0F);

			shakeScript.shake (shockwaveShake);

			//	Also stuns the player briefly
			damagePlayer(shockwaveDamage, stunTime);
		}
	}
	
	void goToDestination() {
		animator.SetBool ("Idle", false);
		agent.SetDestination (currDestination);
	}
	
	void jump() {
		agent.updatePosition = true;
		animator.SetBool("Idle", true);
		animator.SetBool ("Jump", true);
	}
	void stopJump() {
		agent.updatePosition = false;
		animator.SetBool ("Jump", false);
		animator.SetBool ("Idle", true);
	}
}
