using UnityEngine;
using System.Collections;

public class TimeOut : MonoBehaviour {

	public float timeOut;
	private float currTime;

	// Use this for initialization
	void Start () {
		currTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		currTime += Time.deltaTime;
		if (currTime >= timeOut)
			Destroy (this.gameObject);
	}
}
