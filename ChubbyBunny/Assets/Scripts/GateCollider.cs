using System.Collections;

using UnityEngine;
using System.Collections;

public class GateCollider : MonoBehaviour {
	
	public int frogCount;
	public int slimeCount;

	public int slimeTotal;
	public int frogTotal;

	Transform frogPoster;
	Transform slimePoster;
	
	bool frogSuccess;
	bool slimeSuccess;
	public int success;
	
	public GameObject sector2;
	SectorEngine se2;
	
	void Start() {
		// Initializing enemy counts
		frogCount = 0;
		slimeCount = 0;

		slimeTotal = 0;
		frogTotal = 0;
		
		frogPoster = transform.FindChild("FrogCount");
		slimePoster = transform.FindChild ("SlimeCount");
		
		frogSuccess = false;
		slimeSuccess = false;
		success = 0;
		
		se2 = sector2.GetComponent<SectorEngine> ();
	}

	public void incrementSlimeCounter() {
		// Increment slime poster each time slime walks out
		string slimeCountStr = slimePoster.GetComponent<TextMesh>().text;
		int.TryParse(slimeCountStr, out slimeCount);
		if (slimeCount < 3 && slimeCount >= 0) {
			slimeCount = slimeCount + 1;
			slimePoster.GetComponent<TextMesh>().text = slimeCount.ToString ();
			if (slimeCount > 0) {
				slimeSuccess = false;
			}
		}
	}

	public void incrementFrogCounter() {
		// Increment frog poster each time frog walks out 
		string frogCountStr = frogPoster.GetComponent<TextMesh>().text;
		int.TryParse(frogCountStr, out frogCount);
		if (frogCount < 3 && frogCount >= 0) {
			frogCount = frogCount + 1;
			frogPoster.GetComponent<TextMesh>().text = frogCount.ToString ();
			if (frogCount > 0) {
				frogSuccess = false;
			}
		}
	}

	void OnTriggerEnter(Collider col) {
		//	Check if player enters gates
		if (col.tag == "Player") {
			Debug.Log ("player enter gate");
		}
		
		if (col.tag == "Frog") {
			// Decrement frog poster each time frog walks in
			string frogCountStr = frogPoster.GetComponent<TextMesh>().text;
			int.TryParse(frogCountStr, out frogCount);
			
			if (frogCount > 0) {
				frogCount = frogCount - 1;
				frogPoster.GetComponent<TextMesh>().text = frogCount.ToString ();
				if (frogCount == 0) {
					Debug.Log ("Frog capture complete");
					frogSuccess = true;
					if (frogSuccess && slimeSuccess) {
						frogSuccess = false;
						slimeSuccess = false;
						success = success + 1;
						if (success == 1) 
							Debug.Log ("done cage puzzle");
						se2.donePuzzle();
						frogPoster.GetComponent<TextMesh>().text = ":)";
						slimePoster.GetComponent<TextMesh>().text = ":)";
					}
				}
			}
			frogTotal = frogTotal + 1;
		}
		
		if (col.tag == "Slime") {
			// Decrement slime poster each time slime walks in
			string slimeCountStr = slimePoster.GetComponent<TextMesh>().text;
			int.TryParse(slimeCountStr, out slimeCount);
			
			if (slimeCount > 0) {
				slimeCount = slimeCount - 1;
				slimePoster.GetComponent<TextMesh>().text = slimeCount.ToString ();
				if (slimeCount == 0) {
					Debug.Log ("Slime capture complete");
					slimeSuccess = true;
					if (frogSuccess && slimeSuccess) {
						frogSuccess = false;
						slimeSuccess = false;
						success = success + 1;
						if (success == 1) {
							Debug.Log ("done cage puzzle");
							se2.donePuzzle();
							frogPoster.GetComponent<TextMesh>().text = ":)";
							slimePoster.GetComponent<TextMesh>().text = ":)";
						}
					}
				}
			}
				slimeTotal = slimeTotal + 1;
		}
	}
	
	void OnTriggerExit(Collider col) {
		//	Check if player exits gate
		if (col.tag == "Player") {
			Debug.Log ("player exit gate");	
		}
		
		if (col.tag == "Frog") {
//			// Increment frog poster each time frog walks out 
//			string frogCountStr = frogPoster.GetComponent<TextMesh>().text;
//			int.TryParse(frogCountStr, out frogCount);
//			if (frogCount < 3 && frogCount >= 0) {
//				frogCount = frogCount + 1;
//				frogPoster.GetComponent<TextMesh>().text = frogCount.ToString ();
//				if (frogCount > 0) {
//					frogSuccess = false;
//				}
//			}
			frogTotal = frogTotal + 1;
			incrementFrogCounter();
		}
		
		if (col.tag == "Slime") {
//			// Increment slime poster each time slime walks out
//			string slimeCountStr = slimePoster.GetComponent<TextMesh>().text;
//			int.TryParse(slimeCountStr, out slimeCount);
//			if (slimeCount < 3 && slimeCount >= 0) {
//				slimeCount = slimeCount + 1;
//				slimePoster.GetComponent<TextMesh>().text = slimeCount.ToString ();
//				if (slimeCount > 0) {
//					slimeSuccess = false;
//				}
//			}
			slimeTotal = slimeTotal + 1;
			incrementSlimeCounter();
		}
	}
	
}
