using UnityEngine;
using System.Collections;

public class EnemyController_Necromancer: EnemyController_Base {
	
	public float attackRange;
	public float attackDmg;
	public float attackSpeed;
	
	public GameObject fireballFab;
	private GameObject fireball;
	private ParticleSystem fireballSystem;
	public GameObject fireballFXFab;

	//	For spawning the fireball
	public float fireballSpeed;
	public float fullFireballScale;
	public float fireballScaleRate;
	private Transform spawnLoc;
	private bool chargingFire;

	public float walkSpeed;
	public float runSpeed;

	private Vector3 currDestination;

	// Use this for initialization
	public override void Start () {
		base.Start ();

		spawnLoc = this.transform.FindChild ("AttackSpawnPoint");
		chargingFire = false;

		currDestination = findRandomWanderPosition ();
		goToDestination ();
	}

	// Update is called once per frame
	void Update () {
		//	For charging the fireball
		if (chargingFire && fireball != null) {
			//	In case necro moves while charging
			fireball.transform.position = spawnLoc.position;

			//	Scale using lerp with deltaTime
			if(fireballSystem.startSize < (fullFireballScale - 0.01f)) {
				fireballSystem.startSize = Mathf.Lerp(fireballSystem.startSize, fullFireballScale, Time.deltaTime*fireballScaleRate);
			}
			else
				chargingFire = false;
		}

		//	Just wandering around
		if (currState == EnemyState.WANDER) {
			//	Check if reached destination
			if (reachedDestination (currDestination)) {
				this.agent.SetDestination(this.transform.position);	//	Stop the this.agent
				currDestination = findRandomWanderPosition ();
				
				//	Go to next destination after waiting a bit
				animator.SetBool("Idle", true);
				Invoke ("goToDestination", idleTime);
			}
			
			//	Check if noticed player
			if(Vector3.Distance(player.transform.position, this.transform.position) <= detectionRadius) {
				currState = EnemyState.CHASE;
				animator.SetBool("Chase", true);	//	To chase state
				this.agent.speed = runSpeed;
			}
		}
		
		//	Chasing player
		if(currState == EnemyState.CHASE) {
			this.agent.SetDestination(player.transform.position);
			
			//	Within attacking distance
			if(Vector3.Distance(player.transform.position, this.transform.position) <= attackRange) {
				currState = EnemyState.ATTACK;
				animator.SetBool("Attack", true);	//	To start attack state
				this.agent.enabled = false;			//	Root the necro
			}
			
			//	Check if player out of detection range
			if(Vector3.Distance(player.transform.position, this.transform.position) > detectionRadius) {
				currState = EnemyState.WANDER;
				animator.SetBool("Chase", false);	//	To wander state
				currDestination = this.transform.position;
			}
		}
		
		//	Attack!
		if (currState == EnemyState.ATTACK) {
			this.transform.LookAt(player.transform.position);

			//	Check if out of attack range
			AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
			if(state.nameHash == Animator.StringToHash("Base.End Attack") &&
			   Vector3.Distance(player.transform.position, this.transform.position) > attackRange)
			{
				currState = EnemyState.CHASE;
				this.agent.enabled = true;
				this.agent.speed = runSpeed;
				animator.SetBool("Attack", false);
			}	
		}
	}

	//	Since there are 2 death animations for this
	public override void takeDamage(float dmg) {
		health -= dmg;
		
		//	Run death anim if killed
		if (health <= 0 && currState != EnemyState.DYING && currState != EnemyState.DEAD) {
			currState = EnemyState.DYING;
			this.agent.enabled = false;

			//	In case he was charging when killed
			if (chargingFire)
				Destroy (fireball);

			int roll = Random.Range (0, 2);
			if (roll == 0)
				animator.SetTrigger ("Death1");
			else
				animator.SetTrigger ("Death2");
		}
		else if (health > maxHealth) {
			//	Healed by healer
			health = maxHealth;
		}
	}

	//	Called by animator
	void spawnFireball() {
		fireball = Instantiate (fireballFab, spawnLoc.position, Quaternion.identity) as GameObject;

		Physics.IgnoreCollision (this.collider, fireball.collider);
		fireball.collider.enabled = false;
		fireball.GetComponent<ProjectileController_Necromancer> ().setAttackDmg (attackDmg);
		fireball.GetComponent<ProjectileController_Necromancer> ().parent = this.gameObject;

		fireballSystem = fireball.GetComponent<ParticleSystem> ();
		fireballSystem.startSize = 0.01f;

		chargingFire = true;
	}

	//	Called by animator
	void shootFireBall() {
		if (fireball != null) {
			fireball.collider.enabled = true;
			fireball.rigidbody.AddForce (this.transform.forward * fireballSpeed);
		}
	}

	//	Called by animator
	IEnumerator waitUntilNextAttack() {
		yield return new WaitForSeconds (attackSpeed);
		animator.SetTrigger ("Attack Repeat");
	}

	//	Called by animator - keeps necro from moving during attack anim
	void rootEnemy() {
		this.agent.speed = 0;
	}

	void goToDestination() {
		if (this.agent.isActiveAndEnabled) {
			animator.SetBool ("Idle", false);
			this.agent.speed = walkSpeed;
			this.agent.SetDestination (currDestination);
		}
	}
}
