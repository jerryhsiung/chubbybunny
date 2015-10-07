using UnityEngine;
using System.Collections;

public class Puzzle1 : MonoBehaviour {

	private static int count;
	public GameObject[] greenButtons;
	public GameObject twoPrefab, zeroPrefab, onePrefab, fivePrefab;
	public GameObject waffle;
	public GameObject[] poems;

	public GameObject player;
	public GameObject se1;
	public GameObject se2;
	SectorEngine engine1;
	SectorEngine engine2;

	static bool alreadyCalled = false;

	void Start ()
	{
		engine1 = se1.GetComponent<SectorEngine> ();
		engine2 = se2.GetComponent<SectorEngine> ();
		count = 0;
		greenButtons = GameObject.FindGameObjectsWithTag("GreenButton");
		for (int i = 0; i < greenButtons.Length; i++) {
			greenButtons[i].transform.renderer.material.color = Color.green;
		}
	}
	
	public void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Attack") {

			if (this.gameObject.tag == "GreenButton") {
				if (this.gameObject.renderer.material.color == Color.green) {
					transform.renderer.material.color = Color.blue;
					count = count + 1;
			
					Debug.Log ("green button hit");
					Debug.Log (count);
				}
				if (count == 4) {
					GameObject twoInstance, zeroInstance, oneInstance, fiveInstance;

					twoInstance = Instantiate (twoPrefab) as GameObject;
					zeroInstance = Instantiate (zeroPrefab) as GameObject;
					oneInstance = Instantiate (onePrefab) as GameObject;
					fiveInstance = Instantiate (fivePrefab) as GameObject;

					if (!alreadyCalled) {
						alreadyCalled = true;
						engine2.donePuzzle();
					}
				}
			}
			if (this.gameObject.tag == "RedButton") {
				count = 0;

//				Debug.Log ("red button hit");
//				Debug.Log (count);

				for (int i = 0; i < greenButtons.Length; i++) {
					greenButtons [i].transform.renderer.material.color = Color.green;
				}
			}

			//sector 2 puzzle 1 for poem puzzle
			if (this.gameObject.name == "PoemB") {
				Debug.Log("poem puzzle");
				Quaternion rotation = new Quaternion(1,1,1,1);

				GameObject missingShape = Instantiate (waffle, player.transform.position, Quaternion.identity) as GameObject;
				//delete the posters
				for(int i=0; i<poems.Length; i++){
					Destroy(poems[i]);
				}
				engine1.donePuzzle();
			}
		} 
		else {
			
		}
	}

}
