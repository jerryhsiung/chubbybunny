using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public AudioClip energyBoltSound;
	public GameObject energyBolt;
	public float boltSpeed;

	//Get sector info
	public GameObject sector1;
	public GameObject sector2;
	SectorEngine engine1;
	SectorEngine engine2;

	//	Health bar
	public RectTransform healthBar;
	private Image healthBarImg;
	private float healthBarMaxWidth;
	
	public float health;
	public float healthRegenerationRate;	//	1 health per x sec
	private float currRegenTime;
	[HideInInspector]
	public float maxHealth;
	public float attackDmg;

	[HideInInspector]
	public Transform spawnPos;
	
	//	For blinking when damaged
	public float timeInvincible;
	public float timeDoubleDamage;
	public float blinkRate;
	public bool invincible;
	public bool doubleDamage;
	private bool blink;

	//	Min distance for picking up objects
	public float pickupDistance;
	private Transform pickupPoint;
	[HideInInspector]
	public GameObject pickupObj;

	[HideInInspector]
	public bool dead;
	
	[HideInInspector]
	public Transform body;

	//player invincibility effect
	public GameObject invincibleEffect;
	[HideInInspector]
	public bool donewithInvincible = false;
	[HideInInspector]
	bool noresetInvincible = false;

	//player double damage effect
	public GameObject doubleDamageEffect;
	public bool donewithDoubleDamage = false;

	void Awake() {
		maxHealth = health;
	}

	void Start () {
		particleSystem.Pause ();
		engine1 = sector1.GetComponent<SectorEngine> ();
		engine2 = sector2.GetComponent<SectorEngine> ();
		currRegenTime = 0;

		healthBarMaxWidth = healthBar.rect.width;
		healthBarImg = healthBar.GetComponent<Image>();
		
		spawnPos = this.transform.FindChild ("Shoot Point");
		pickupPoint = this.transform.FindChild ("Pickup Point");
		doubleDamage = invincible = dead = false;
		
		body = this.transform.FindChild ("Query-Chan").FindChild ("BodyParts");

	}

	void Update () {
		//	Hack so OnTriggerStay works for drones
		this.transform.position += Vector3.zero;

		if (dead)
			return;

		if (pickupObj != null) {
			pickupObj.rigidbody.velocity = Vector3.zero;
			pickupObj.rigidbody.angularVelocity = Vector3.zero;
			pickupObj.transform.position = pickupPoint.position;
		}

		//	Regenerate health
		currRegenTime += Time.deltaTime;
		if(currRegenTime >= healthRegenerationRate) {
			++health;
			if(health > maxHealth) {
				currRegenTime = 0;
				health = maxHealth;
			}

			currRegenTime -= healthRegenerationRate;
		}

		//	Update health bar
		float fraction = health/maxHealth;

		//	Change color based on fraction
		if(fraction >= 0.7f) {
		 	healthBarImg.color = Color.green;
		}
		else if(fraction >= 0.3f) {
			healthBarImg.color = Color.yellow;
		}
		else {
			healthBarImg.color = Color.red;
		}

		healthBar.sizeDelta = new Vector2(fraction*healthBarMaxWidth, healthBar.rect.height);

		#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
		//	Temp code for shooting
		if (Input.GetMouseButtonDown (0) && !GameEngine.pauseGame) {
			//	Convert touch position to a ray
			Vector3 touchPos = Input.mousePosition;
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(touchPos.x, touchPos.y, 0));
			Vector3 forceDir;
			//	Raycast and check if hit anything
			RaycastHit hitInfo;
			if(Physics.Raycast(ray.origin, ray.direction, out hitInfo)) {
				forceDir = hitInfo.point - spawnPos.position;

				//	First check if tapped on pickup
				if(pickupObj != null) {
					if(hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Pickups")) {
						print("Dropping object");
						dropObject();
						return;
					}
				}
			}
			else {
				//	Get a world point 1000 units away instead
				Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, 1000f));
				forceDir = point - spawnPos.position;
			}
			
			//	Instantiate energy bolt
			forceDir.Normalize();
			fireEnergyBolt(forceDir);
		}

		//	Temp code for picking things up
		if (Input.GetMouseButtonDown (1)) {
			//	Convert touch position to a ray
			Vector3 touchPos = Input.mousePosition;
			pickupObject(new Vector3(touchPos.x, touchPos.y, 0));
		}
		#endif

		//player is invincible for a minute
		if (!engine2.rewardTime_done) {
			invincibleEffect.transform.position = this.transform.position;
		} 
		else if(!noresetInvincible){
			invincible = false;
			noresetInvincible = true;
			Destroy(invincibleEffect);
		}

		//player has double damage for a minute
		if (!engine1.rewardTime_done) {
			doubleDamageEffect.transform.position = this.transform.position;
		} 
		else {
			doubleDamage = false;
			Destroy(doubleDamageEffect);
		}

	}

	//called when player complete sector 2
	public void invincibility(){
		invincible = true;
		donewithInvincible = true;
		invincibleEffect = Instantiate (invincibleEffect, this.transform.position, Quaternion.identity) as GameObject;
	}

	//caled when player completes sector 1
	public void doubleDamagePower(){
		doubleDamage = true;
		donewithDoubleDamage = true;
		doubleDamageEffect = Instantiate (doubleDamageEffect, this.transform.position, Quaternion.identity) as GameObject;
	}

	//	Triggered by the touch assistants
	public void fireEnergyBolt(Vector3 dir) {
		audio.PlayOneShot(energyBoltSound);
		GameObject bolt = Instantiate(energyBolt, spawnPos.position,
		                              Quaternion.LookRotation(-dir)) as GameObject;
		bolt.GetComponent<ProjectileController_Player>().parent = this.gameObject;
		bolt.GetComponent<ProjectileController_Player>().setAttackDmg(attackDmg);
		Debug.Log (attackDmg);

		if (doubleDamage) {
//			doubleDamage = true;

			bolt.GetComponent<ProjectileController_Player>().parent = this.gameObject;
			bolt.GetComponent<ProjectileController_Player>().setAttackDmg(20);

//			Invoke ("resetDoubleDamage", timeDoubleDamage);
		}

		//	Disable rotation
		bolt.rigidbody.freezeRotation = true;

		//	Kick it
		bolt.rigidbody.AddForce(dir * boltSpeed);
	}
	
	//	I got hit :(
	public void takeDamage(float dmg, float stunTime) {
		if (!invincible && !dead) {
			health -= dmg;
			//	Check if killed
			if(health <= 0) {
				//StartCoroutine(setMove (false, 0));
				dead = true;
				return;
			}

			//	Drop anything being carried
			if(pickupObj != null)
				dropObject();

			//	Report to game engine
			GameObject engine = GameObject.FindGameObjectWithTag("GameEngine");
			engine.GetComponent<GameEngine>().playerHit();

			Debug.Log("invincible true "+timeInvincible);
			invincible = true;

			
			InvokeRepeating ("blinkPlayer", 0, blinkRate);
			Invoke ("resetInvincibility", timeInvincible);
			
			if (stunTime > 0) {
				StartCoroutine(setMove (false, 0));
				StartCoroutine(setMove(true, stunTime));
				
				//animator.SetTrigger ("TakeDamage");
			}
		}
	}

	public void pickupObject(Vector3 screenPos) {
		Ray ray = Camera.main.ScreenPointToRay (screenPos);
		RaycastHit hitInfo;
		if (Physics.Raycast (ray.origin, ray.direction, out hitInfo)) {
			GameObject pickup = hitInfo.transform.gameObject;
			//	Make sure is pickups layer
			if(pickup.layer == LayerMask.NameToLayer("Pickups")
			   && Vector3.Distance(this.transform.position, pickup.transform.position) <= pickupDistance) {
				pickupObj = pickup;
			}
		}
	}
	public void dropObject() {
		//	So pickup falls gently
		pickupObj = null;
	}

	/*	HELPER FUNCTIONS	*/
	
	//	For invincibility blinking when damaged
	private void blinkPlayer() {
		if (blink) {
			SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
			for(int i = 0; i < renderers.Length; ++i)
				renderers[i].enabled = false;
			blink = false;
		}
		else {
			SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
			for(int i = 0; i < renderers.Length; ++i)
				renderers[i].enabled = true;
			blink = true;
		}
	}
	
	//	Reseting invincibility after damaged
	private void resetInvincibility() {
		CancelInvoke ();
		invincible = false;
		SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
		for(int i = 0; i < renderers.Length; ++i)
			renderers[i].enabled = true;
	}

	// resetting double damage after enemy attacked
//	private void resetDoubleDamage() {
//		CancelInvoke ();
//		doubleDamage = false;
//		SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
//		for(int i = 0; i < renderers.Length; ++i)
//			renderers[i].enabled = true;
//	}
	
	//	Setting if player can move or not
	public IEnumerator setMove(bool move, float time) {
		yield return new WaitForSeconds (time);
		this.GetComponent<PlayerMover> ().canMove = move;
	}
}
