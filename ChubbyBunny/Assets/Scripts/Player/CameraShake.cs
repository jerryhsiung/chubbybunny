using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	public float numShakes;				//	How many times to shake
	private float currNumShakes;

	private Vector3 originalPos;		//	Original local pos of camera
	private float shakeIntensity;		//	Max shake intensity

	private bool shakeDir;				//	True = shake up, false = shake down
	private bool startShake;

	// Use this for initialization
	void Start () {
		shakeDir = true;
		startShake = false;
		currNumShakes = 0;

		originalPos = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if (startShake) {
			//	Test if time to stop shaking
			if(currNumShakes > numShakes) {
				startShake = false;
				currNumShakes = 0;
				return;
			}

			//	Get the shake vector
			Vector3 shakePos = Vector3.up*shakeIntensity;
			if(!shakeDir)
				shakePos *= -1;

			//	Set position
			this.transform.localPosition = originalPos + shakePos;

			//	Reverse shake direction
			if(shakeDir)
				shakeDir = false;
			else
				shakeDir = true;

			++currNumShakes;
		}
		else
			this.transform.localPosition = originalPos;
	}

	public void shake(float intensity) {
		shakeIntensity = intensity;
		startShake = true;
	}
}
