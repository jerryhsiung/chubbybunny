using UnityEngine;
using System.Collections;

public class AlwaysFacePlayer : MonoBehaviour {

	private Transform player;

	void Start() {
		player = GameObject.Find ("Player").transform.FindChild ("Main Camera");

	}

	// Update is called once per frame
	void Update () {
		transform.LookAt (player.position);
	}
}
