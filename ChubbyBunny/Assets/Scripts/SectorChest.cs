using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SectorChest : MonoBehaviour {

	public AudioClip beepSound;

	public GameObject canvas;
	public GameObject parentEngine;
	public GameObject playerController;
	public GameObject keyPad;
	public GameObject keyPadFrame;
	public GameObject screenDark;

	public string combination;
	private string combo;

	public float timeStopIncorrectComboEffect;
	public float timeToggleIncorrectComboEffect;
	private float currToggleTime;
	private float toggleTimeSinceStart;
	private bool color;
	private bool incorrect;

	public float clickDistToClose;

	private RectTransform panel;

	void Start() {
		color = incorrect = false;
		combo = "";
		currToggleTime = toggleTimeSinceStart = 0;

		panel = keyPadFrame.GetComponent<RectTransform> ();
	}

	void OnCollisionEnter(Collision col) {
		//	Open chest keypad if struck by player attack
		if (col.gameObject.tag == "Attack") {
			GameEngine.pauseGame = true;

			screenDark.SetActive(true);
			keyPadFrame.SetActive(true);

			Time.timeScale = 0;
			keyPad.SetActive (true);
		}
	}

	void Update() {
		//	Check for exit touches
		#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
		if (Input.GetMouseButtonDown (0) && GameEngine.pauseGame) {
			Vector2 clickPos = new Vector2(Input.mousePosition.x,
			                               Input.mousePosition.y);

			if(clickPos.x < Screen.width*clickDistToClose || clickPos.x > (Screen.width*(1 - clickDistToClose))) {
				//	Clicked outside the keypad frame
				screenDark.SetActive(false);
				keyPadFrame.SetActive(false);
				
				Time.timeScale = 1;
				keyPad.SetActive(false);
				GameEngine.pauseGame = false;
			}
		}
		#endif
		#if UNITY_IOS
		if(Input.touchCount > 0) {
			Touch touch = Input.GetTouch(0);
			if(touch.phase == TouchPhase.Ended) {
				Vector2 touchPos = new Vector2(touch.position.x, touch.position.y);

				if(touchPos.x < Screen.width*clickDistToClose || touchPos.x > (Screen.width*(1 - clickDistToClose))) {
					//	Clicked outside the keypad frame
					screenDark.SetActive(false);
					keyPadFrame.SetActive(false);
					
					Time.timeScale = 1;
					keyPad.SetActive(false);
					GameEngine.pauseGame = false;
				}
			}
		}
		#endif
		
		if (incorrect) {
			currToggleTime += Time.unscaledDeltaTime;
			toggleTimeSinceStart += Time.unscaledDeltaTime;

			//	Flicker keypad colors if wrong combo
			if(toggleTimeSinceStart > timeStopIncorrectComboEffect) {
				incorrect = false;
				currToggleTime = toggleTimeSinceStart = 0;
				changeKeypadColor(Color.white);
			}
			else if(currToggleTime >= timeToggleIncorrectComboEffect) {
				currToggleTime = 0;
				if(color) {
					changeKeypadColor(Color.red);
					color = false;
				}
				else {
					changeKeypadColor(Color.white);
					color = true;
				}
			}
		}
	}

	public void keyPressed(string key) {
		audio.PlayOneShot (beepSound);
		if (key == "*") {
			if(combo == combination) {
				//	Got it!
				PlayerController control = playerController.GetComponent<PlayerController>();

				//	Double max health
				control.maxHealth *= 2f;
				control.health = control.maxHealth;

				//	Make chest disappear
				Destroy(this.gameObject);

				keyPad.SetActive(false);
				screenDark.SetActive(false);
				keyPadFrame.SetActive(false);

				parentEngine.GetComponent<SectorEngine>().treasureFound = true;

			}
			else {
				combo = "";
				changeKeypadColor(Color.red);
				incorrect = true;
			}
		}
		else {
			combo += key;
		}
	}

	private void changeKeypadColor(Color col) {
		//	Change color of all buttons
		for(int i = 0; i < keyPad.transform.childCount; ++i) {
			GameObject button = keyPad.transform.GetChild(i).gameObject;
			button.GetComponent<Image>().color = col;
		}
	}
}
