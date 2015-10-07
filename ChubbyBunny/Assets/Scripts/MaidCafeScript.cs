using UnityEngine;
using System.Collections;

public class MaidCafeScript : MonoBehaviour {

	public GameObject gameEngine;
	public Transform telePoint;

	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") {
			GameObject player = col.gameObject;

			//	Teleport to rooftop
			player.transform.position = telePoint.position;

			//	Face bunny
			player.transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));

			//	Start da party ayyyy
			gameEngine.GetComponent<GameEngine>().partyDontStartTilIWalkIn();
		}
	}
}
