using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterGenerator : MonoBehaviour {
	
	public enum MonsterTypes {
		SLIME = 0,
		FROG = 1,
		NECRO = 2,
		HEALER = 3,
		DRONE = 4
	};
	
	public bool isActive;
	
	//	Number of enemies in the sector
	public int[] maxNumEnemy;
	private int[] currNumEnemy;
	
	//	All points where monsters can spawn (sector specific)
	private Transform[] spawnPoints;
	
	public int minSpawnTime;
	public int maxSpawnTime;
	
	//	Monster prefabs
	public GameObject[] monsterPrefabs;
	
	[HideInInspector]
	public IList<GameObject> allMonsters;
	[HideInInspector]
	public SectorEngine parentSector;
	
	// Monster poster
//	public Transform frogPoster;
//	public Transform slimePoster;
//	public int frogCount;
//	public int slimeCount;
	//	public GameObject cagePuzzle;
	public GameObject tallfence1;
	public GameObject tallfence2;
	public GameObject tallfence3;
	public GameObject tallfence4;
	public GameObject tallfence5;
	
	public Vector3 bounds1;
	public Vector3 bounds2;
	public Vector3 bounds3;
	public Vector3 bounds4;
	public Vector3 bounds5;

	public GameObject gateCollider;
	GateCollider gc; 
	
	void Start () {
		//	Make sure same number of prefabs as maxNumEnemy
		if (monsterPrefabs.Length != maxNumEnemy.Length) {
			Debug.Log("Error in MonsterGenerator: size of maxNumEnemy must be the same as monsterPrefabs.");
			Application.Quit();
		}
		
		//	Set current monster counts to 0
		currNumEnemy = new int[maxNumEnemy.Length];
		for (int i = 0; i < currNumEnemy.Length; ++i)
			currNumEnemy [i] = 0;
		
		allMonsters = new List<GameObject> ();

		gc = gateCollider.GetComponent<GateCollider> ();
	}
	
	void Update () {
		if (isActive) {
			//	Take destroyed monsters out of the list
			for (int i = 0; i < allMonsters.Count; ++i) {
				GameObject go = allMonsters [i];
				if (go.GetComponent<EnemyController_Base> ().currState == EnemyController_Base.EnemyState.DEAD) {
					//	Decrement the right counter
					switch (go.tag) {
					case "Slime":
						//	Check if monster dies in Cage Puzzle
						if (gc.slimeTotal > 0) 
							gc.slimeTotal = gc.slimeTotal - 1;
						if (gc.slimeTotal < 3) 
							gc.incrementSlimeCounter();

						--currNumEnemy [(int)MonsterTypes.SLIME];
						parentSector.addKill(MonsterTypes.SLIME);
						break;
					case "Frog":
						//	Check if monster dies in Cage Puzzle
						if (gc.frogTotal > 0)
							gc.frogTotal = gc.frogTotal - 1;
						if (gc.frogTotal < 3)
							gc.incrementFrogCounter();

						--currNumEnemy [(int)MonsterTypes.FROG];
						parentSector.addKill(MonsterTypes.FROG);
						break;
					case "Necromancer":
						--currNumEnemy [(int)MonsterTypes.NECRO];
						parentSector.addKill(MonsterTypes.NECRO);
						break;
					case "Healer":
						--currNumEnemy [(int)MonsterTypes.HEALER];
						parentSector.addKill(MonsterTypes.HEALER);
						break;
					}
					
					//	Destroy and remove from list
					Destroy (go);
					allMonsters.RemoveAt (i);
					--i;
				}
			}
			
			if (spawnPoints.Length == 0)
				return;
			
			//	Create more monsters
			for (int i = 0; i < monsterPrefabs.Length; ++i) {
				while (currNumEnemy[i] < maxNumEnemy[i]) {
					//	Pick random spawn point and random time
					int index = Random.Range (0, spawnPoints.Length);
					Transform point = spawnPoints [index];
					float randTime = Random.Range (minSpawnTime, maxSpawnTime);
					
					if (i == (int)MonsterTypes.HEALER)
						StartCoroutine (instantiateMonster (monsterPrefabs [i], point, randTime, true));
					else
						StartCoroutine (instantiateMonster (monsterPrefabs [i], point, randTime, false));
					
					++currNumEnemy [i];
				}
			}
		}
	}
	
	public void setSpawnPoints(Transform[] points) {
		spawnPoints = points;
	}
	
	//	Called by sector engine
	public void deactivate() {
		isActive = false;
		StopAllCoroutines ();
		
		//	Destroy all current monsters
		for(int i = 0; i < allMonsters.Count; ++i) {
			GameObject go = allMonsters[i];
			go.GetComponent<HealthBarController>().destroyMe();
			Destroy(go);
		}
		allMonsters.Clear ();
		
		//	Reset current monster counts
		currNumEnemy = new int[maxNumEnemy.Length];
		for (int i = 0; i < currNumEnemy.Length; ++i)
			currNumEnemy [i] = 0;
	}
	
	private IEnumerator instantiateMonster(GameObject monster, Transform location, float randTime, bool healer) {
		yield return new WaitForSeconds(randTime);
		
		GameObject go = Instantiate (monster, location.position, Quaternion.identity) as GameObject;
		go.GetComponent<EnemyController_Base> ().setSpawnPoints (spawnPoints);
		
		if (healer)
			monster.GetComponent<EnemyController_Healer> ().monsterGenScript = this;
		
		allMonsters.Add (go);
	}
}
