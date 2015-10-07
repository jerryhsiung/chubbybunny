using UnityEngine;
using System.Collections;

public class GetDonutOnTop : MonoBehaviour {

	int count;
	public int goal;
	public int index;
//	public GameObject[] donuts;
	public GameObject[] dishes;
	public GameObject sector1;
	SectorEngine se1;
	static bool[] done;

	// Use this for initialization
	void Start () {
		se1 = sector1.GetComponent<SectorEngine> ();
		done = new bool[4];
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(count == goal && !(index==0 && count==0) ){
			done[index] = true;
		}

		bool success = true;
		for (int i=0; i<4; i++) {
			if(!done[i]){
				success = false;
				break;
			}
		}
		if (success) {
			//delete the dishes
			Debug.Log("done donut puzzle");
			se1.donePuzzle();
			for (int i=0; i<4; i++) {
				Destroy(dishes[i]);
			}
//			for(int i=0; i<10; i++){
//				Destroy(donuts[i]);
//			}
		}

	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.name == "Donuts") {
			count++;
		}
	}
	void OnTriggerExit(Collider col){
		if(col.gameObject.name == "Donuts"){
			count--;
			if(done[index]){
				done[index] = false;
			}
		}
	}
}
