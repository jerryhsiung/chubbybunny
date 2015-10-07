using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneGenerator : MonoBehaviour {

	public Transform[] spawnPoints;
	public GameObject droneFab;

	public int maxDrones;

	public float minSpawnTime;
	public float maxSpawnTime;

	private IList<GameObject> allDrones;
	private int currNumDrones;

	private bool active;

	void Start () {
		currNumDrones = 0;
		allDrones = new List<GameObject> ();
		active = true;
	}

	void Update () {
		if (active) {
			//	Check for dead drones
			for (int i = 0; i < allDrones.Count; ++i) {
				GameObject go = allDrones [i];
				if (go.GetComponent<EnemyController_Drone> ().currState == EnemyController_Base.EnemyState.DEAD) {
					--currNumDrones;
					DestroyObject (go);
					allDrones.RemoveAt (i);
					--i;
				}
			}

			//	Add new drones
			while (currNumDrones < maxDrones) {
				//	Generate random location
				int i = Random.Range (0, spawnPoints.Length);
				Vector3 loc = spawnPoints [i].position;
				//	Generate random spawn time
				float time = Random.Range (minSpawnTime, maxSpawnTime);
				StartCoroutine (instantiateDrone (loc, time));

				++currNumDrones;
			}
		}
	}

	//	Called when game is over
	public void killAllDrones() {
		for (int i = 0; i < allDrones.Count; ++i)
			Destroy(allDrones[i].gameObject);
		allDrones.Clear ();

		active = false;
		StopAllCoroutines ();
	}

	private IEnumerator instantiateDrone(Vector3 location, float randTime) {
		yield return new WaitForSeconds(randTime);

		GameObject drone = Instantiate (droneFab, location, Quaternion.identity) as GameObject;
		drone.GetComponent<EnemyController_Base> ().setSpawnPoints (spawnPoints);

		allDrones.Add (drone);
	}
}
