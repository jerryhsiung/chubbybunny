using UnityEngine;
using System.Collections;

public class TriggerEnter : MonoBehaviour {
	
	[HideInInspector]
	public bool caked = false;

	public GameObject[] teleporters;
	public GameObject se1;
	SectorEngine engine1;

	// Use this for initialization
	void Start () {
		engine1 = se1.GetComponent<SectorEngine> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "Player") {
			caked = true;
			Destroy (this.gameObject);
			for(int i=0; i<teleporters.Length; i++){
				Destroy(teleporters[i].gameObject);
			}
			engine1.donePuzzle();
		}

	}
}
