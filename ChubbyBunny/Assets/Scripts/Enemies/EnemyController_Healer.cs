using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController_Healer : EnemyController_Base {

	public GameObject healEffect;
	public GameObject deathEffect;

	public float walkSpeed;

	public float healPower;
	public float healRate;
	public float healRange;			//	Range before can't heal anymore
	public float startHealRange;	//	Range before starts healing

	private GameObject objectHealing;	//	GameObject being healed
	private EnemyController_Base objectHealingScript;
	private Transform steed;			//	THY ROYAL STEED

	[HideInInspector]	
	public MonsterGenerator monsterGenScript;

	private Vector3 currDestination;

	public override void Start () {
		base.Start ();

		steed = this.transform.FindChild ("Steed");

		currDestination = findRandomWanderPosition ();
		goToDestination ();
	}

	void Update () {
		switch (currState) {
		case EnemyState.WANDER:

			//	Go somewhere else if reached destination
			if (reachedDestination (currDestination)) {
				currDestination = findRandomWanderPosition ();
				goToDestination();
			}

			//	Check if any hurt monsters in detection range
			if(monsterGenScript == null)
				return;
			IList<GameObject> currMonsters = monsterGenScript.allMonsters;
			if(currMonsters == null)
				return;

			for(int i = 0; i < currMonsters.Count; ++i) {
				GameObject monster = currMonsters[i] as GameObject;
				float distance = Vector3.Distance(this.transform.position, monster.transform.position);
				EnemyController_Base control = monster.GetComponent<EnemyController_Base>();

				if(distance <= detectionRadius && control.health < control.maxHealth) {
					//	Found one, go to chase state
					transitionChase(monster);
					animator.SetTrigger("follow");
				}
			}

			break;

		case EnemyState.CHASE:	//	Following enemy

			//	Juuuust in case
			if(objectHealing == null) {
				animator.SetTrigger("wander");
				transitionWander();
				return;
			}

			agent.SetDestination(objectHealing.transform.position);

			//	Check if at startHealRange
			if(Vector3.Distance(this.transform.position, objectHealing.transform.position) < startHealRange) {
				animator.SetTrigger("stop");
				InvokeRepeating ("heal", 0, healRate);

				//	Stop movement
				agent.Stop();
				agent.updatePosition = false;
				steed.gameObject.GetComponent<Animator>().SetBool("stop", true);

				currState = EnemyState.ATTACK;
			}

			break;

		case EnemyState.ATTACK:	//	Healing enemy

			//	Turn to face enemy
			Vector3 pointVector = objectHealing.transform.position - this.transform.position;
			if(pointVector != Vector3.zero) {
				Quaternion targetRotation = Quaternion.LookRotation (pointVector);
				float turn = Mathf.Min (4 * Time.deltaTime, 1);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, turn);
			}

			//	Check if patient is fully healed
			if(objectHealingScript.health >= objectHealingScript.maxHealth) {
				animator.SetTrigger("wander");
				transitionWander();
			}

			break;
		}

		if (currState != EnemyState.DEAD && currState != EnemyState.DYING) {
			//	In case the patient dies T_T
			if (objectHealing != null && objectHealingScript.currState == EnemyState.DEAD) {
				//	Transition to wander state
				animator.SetTrigger ("wander");
				transitionWander ();
			}
		
			//	Check if I'm hurt
			if (health < maxHealth && objectHealing != this.gameObject) {
				agent.Stop();
				animator.SetTrigger ("stopSelf");
				transitionChase (this.gameObject);
				CancelInvoke ();
			}
		}
	}

	/*	State transitions	*/
	void transitionWander() {
		//	Cancel any invokes to heal
		CancelInvoke();
		objectHealing = null;

		//	Get steed walking and go to random destination
		steed.gameObject.GetComponent<Animator>().SetBool("stop", false);
		currDestination = findRandomWanderPosition ();
		goToDestination();

		currState = EnemyState.WANDER;
	}
	void transitionChase(GameObject chase) {
		objectHealing = chase;
		objectHealingScript = chase.GetComponent<EnemyController_Base>();
		currState = EnemyState.CHASE;
	}
	void transitionDead() {
		currState = EnemyState.DEAD;
	}
	/************************/

	//	Called by InvokeRepeating
	void heal() {
		animator.SetTrigger("heal");
	}

	//	Override to fix death animation
	public override void takeDamage(float dmg) {
		health -= dmg;
		
		//	Run death anim if killed
		if (health <= 0 && currState != EnemyState.DYING && currState != EnemyState.DEAD) {
			currState = EnemyState.DYING;
			agent.enabled = false;
			animator.SetTrigger ("Death");

			//	Death effect
			float yOffset = this.gameObject.collider.bounds.size.y * 0.5f;
			Instantiate(deathEffect, this.transform.position + new Vector3(0, yOffset, 0),
			            Quaternion.Euler(new Vector3(270f, 0, 0)));
			Instantiate(deathEffect, this.transform.position + new Vector3(0, yOffset, 0),
			            Quaternion.Euler(new Vector3(270f, 0, 0)));

			Invoke("transitionDead", 0.5f);
		}
		else if (health > maxHealth) {
			//	Healed by healer
			health = maxHealth;
		}
	}

	//	Called by animator
	void healTarget() {
		//	Spawn heal effect
		if (objectHealing != null) {
			float yOffset = objectHealing.collider.bounds.size.y * 0.2f;
			float xScale = objectHealing.collider.bounds.size.x;

			GameObject heal = Instantiate (healEffect, this.transform.position,
			                               Quaternion.Euler (new Vector3 (90, 0, 0))) as GameObject;

			HealEffectController healControl = heal.GetComponent<HealEffectController>();
			healControl.parent = objectHealing;
			healControl.yOffset = yOffset;

			//	Adjust effect size based on object size
			float scaleFactor = xScale*0.05f;
			heal.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

			//	Heal
			objectHealingScript.takeDamage (-healPower);
		}
	}

	void goToDestination() {
		if (agent.isActiveAndEnabled) {
			agent.speed = walkSpeed;
			agent.SetDestination (currDestination);
		}
	}
}
