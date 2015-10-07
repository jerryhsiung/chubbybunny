using UnityEngine;
using System.Collections;

public class HealEffectController : MonoBehaviour {

	[HideInInspector]
	public GameObject parent;
	[HideInInspector]
	public float yOffset;

	void Update () {
		if (parent == null) {
			Destroy (this.gameObject);
			return;
		}

		//	Stick with the parent forever <3
		this.transform.position = 
			parent.transform.position + new Vector3 (0, yOffset, 0);
	}
}
