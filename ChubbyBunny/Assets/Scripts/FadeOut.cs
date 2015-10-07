using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOut : MonoBehaviour {

	public bool activate;

	public float fadeTime;
	private float currFadeTime;

	private Color color;

	// Use this for initialization
	void Start () {
		currFadeTime = 0;
		color = this.GetComponent<Image> ().color;
		activate = false;
	}
	
	void Update() {
		if (activate) {
			currFadeTime += Time.deltaTime;
			color.a = Mathf.Lerp (0, 1f, currFadeTime / fadeTime);
			this.GetComponent<Image> ().color = color;
		}
	}
}
