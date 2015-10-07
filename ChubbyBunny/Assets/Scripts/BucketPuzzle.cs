using UnityEngine;
using System.Collections;

public class BucketPuzzle : MonoBehaviour {

	public GameObject player;
	public GameObject parentSector;

	void OnTriggerEnter(Collider col) {
		if (col.tag == "SoccerBall" && player.GetComponent<PlayerController>().pickupObj != col.gameObject) {
			//	Display the arrows
			GameObject arrows = GameObject.Find ("P1S2Arrows");

			for(int i = 0; i < arrows.transform.childCount; ++i) {
				arrows.transform.GetChild(i).gameObject.SetActive(true);
			}

			parentSector.GetComponent<SectorEngine>().donePuzzle();
		}
	}
}
