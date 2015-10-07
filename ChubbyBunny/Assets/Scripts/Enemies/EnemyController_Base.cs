using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
public class EnemyController_Base : MonoBehaviour {

	public enum EnemyState {
		WANDER,		//	Just wandering
		CHASE,		//	Chasing player
		ATTACK,		//	Attacking player
		DYING,		//	I r dying (Let's me play death animation before destroying object)
		DEAD		//	I r ded
	};

	public float maxHealth;
	public float health;

	public float detectionRadius;	//	From how far away enemy will detect player
	public float destinationOffset;	//	Cushion for determining if reached destination
	public float idleTime;			//	Time just standing around after reaching destination

	//	Use for wandering
	private Transform[] spawnPoints;
	private int numSpawnPoints;

	[HideInInspector]
	public GameObject player;
	[HideInInspector]
	public NavMeshAgent agent;
	[HideInInspector]
	public EnemyState currState;
	[HideInInspector]
	public Animator animator;

	public virtual void Start() {
		currState = EnemyState.WANDER;
		agent = this.GetComponent<NavMeshAgent> ();
		animator = this.GetComponent<Animator> ();

		//	Get player object
		player = GameObject.FindGameObjectWithTag ("Player");
		if (player == null)
			Debug.LogError ("Error: Player GameObject not found.");
	}

	//	Find a random position on the map to wander to
	public Vector3 findRandomWanderPosition() {
		int point = Random.Range (0, numSpawnPoints);
		return spawnPoints [point].position;
	}

	//	Check if pretty much at destination point
	public bool reachedDestination(Vector3 destination) {
		bool x = false, y = false, z = false;
		if (Mathf.Abs (this.transform.position.x - destination.x) < destinationOffset)
			x = true;
		if (Mathf.Abs (this.transform.position.y - destination.y) < destinationOffset)
			y = true;
		if (Mathf.Abs (this.transform.position.z - destination.z) < destinationOffset)
			z = true;

		if (x && y && z)
			return true;
		else
			return false;
	}

	public virtual void takeDamage(float dmg) {
		health -= dmg;

		//	Run death anim if killed
		if (health <= 0 && currState != EnemyState.DYING && currState != EnemyState.DEAD) {
			currState = EnemyState.DYING;
			agent.enabled = false;
			animator.SetTrigger ("Death");
		}
		else if (health > maxHealth) {
			//	Healed by healer
			health = maxHealth;
		}
	}

	public void damagePlayer(float dmg, float stunTime) {
		player.GetComponent<PlayerController> ().takeDamage (dmg, stunTime);
	}

	//	Called by monster gen
	public void setSpawnPoints(Transform[] points) {
		spawnPoints = points;
		numSpawnPoints = spawnPoints.Length;
	}

	//	Called by death anim
	private void destroyMe() {
		currState = EnemyState.DEAD;
	}
}
