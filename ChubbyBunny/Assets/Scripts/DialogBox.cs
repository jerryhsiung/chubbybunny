using UnityEngine;
using System.Collections;

public class DialogBox : MonoBehaviour {

	//Getting information from other Engines
	public GameObject S1Wall;
	public GameObject S2Wall;
	SectorCollider s1Collider;
	SectorCollider s2Collider;
	public GameObject engine1;
	public GameObject engine2;
	SectorEngine se1;
	SectorEngine se2;
	public GameObject gameEngine;
	GameEngine ge;

	//private variables
	bool GuiOn = true;
	GUIStyle currentStyle = null;
	GUIStyle buttonStyle = null;
	int page = 0;
	bool restart = false;
	Texture image;
	Vector2 scrollPosition;
	bool yesimage = false;

	// Use this for initialization
	void Start () {
		s1Collider = S1Wall.GetComponent<SectorCollider> ();
		s2Collider = S2Wall.GetComponent<SectorCollider> ();
		se1 = engine1.GetComponent<SectorEngine> ();
		se2 = engine2.GetComponent<SectorEngine> ();
		ge = gameEngine.GetComponent<GameEngine> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount == 0)
			return;
		Touch touch = Input.touches [0];
		if (touch.phase == TouchPhase.Moved) {
			float dt = Time.deltaTime/touch.deltaTime;
			if(dt==0 || float.IsNaN(dt) || float.IsInfinity(dt)){
				dt = 3.0f;
			}
			Vector2 glassDelta = touch.deltaPosition * dt;
			scrollPosition.y += glassDelta.y;
		}
	}
	
	public void OnGUI(){
		InitStyles ();
		if (GUI.Button (new Rect (Screen.width * .92f, Screen.height * .87f, Screen.width * .06f, Screen.height * .1f), "?", buttonStyle)) {
			if (s1Collider.onHighway && s2Collider.onHighway) {
				page = 0;
			}
			GuiOn = true;
		} 
		else {
			//get page # from quadrant # or when finish both sectors
			if (se1.showGUI && se2.showGUI) {
				page = 3;
				GuiOn = true;
			} 
			else if (s1Collider.sector1) {
				page = 1;
				GuiOn = true;
			} 
			else if (s2Collider.sector2) {
				page = 2;
				GuiOn = true;
			} 
			else if (ge.gameOver) {
				restart = true;
			}
		}

		if (se2.treasureFound) {
			page = 5;
			GuiOn = true;
		}

		if (se1.finished && !se2.finished) {
			//show left side of the riddle
			image = (Texture)Resources.Load("leftscroll");
			yesimage = true;
		}
		if (se2.finished && !se1.finished) {
			//show right side of the riddle
			image = (Texture)Resources.Load("rightscroll");
			yesimage = true;
		}
		if (se1.finished && se2.finished) {
			image = (Texture)Resources.Load ("fullscroll");
			yesimage = true;
		}

		if (GuiOn) {
			Time.timeScale = 0;
			GameEngine.pauseGame = true;
			scrollPosition = GUI.BeginScrollView(new Rect(Screen.width * .05f, Screen.width *.05f, Screen.width * .9f, Screen.height * .9f),scrollPosition, new Rect(Screen.width * .05f, Screen.width * .05f, Screen.width * .9f, Screen.height * 1.5f));

			GUI.Box (new Rect (Screen.width * .05f, Screen.width * .05f, Screen.width * .9f, Screen.height*1.5f), story(page), currentStyle);

			if(yesimage){
				if(page == 0){
					GUI.DrawTexture(new Rect(Screen.width * .35f, Screen.height*.8f, image.width, image.height), image);
				}
				else if(page == 3){
					GUI.DrawTexture(new Rect(Screen.width * .35f, Screen.height*.4f, image.width, image.height), image);
				}
				else{
					GUI.DrawTexture(new Rect(Screen.width * .35f, Screen.width * .6f, image.width, image.height), image);
				}

			}
			GUI.EndScrollView();
			if (GUI.Button (new Rect (Screen.width * .8f, Screen.height * .905f, Screen.width * .15f, Screen.height * .075f), "OK", buttonStyle)) {
				if (se1.showGUI && se2.showGUI) {
					se1.showGUI = false;
					se2.showGUI = false;
				}
				if (s1Collider.sector1) {
					s1Collider.sector1 = false;
				} else if (s2Collider.sector2) {
					s2Collider.sector2 = false;
				}
				se2.treasureFound = false;
				GuiOn = false;
				Time.timeScale = 1;
				GameEngine.pauseGame = false;
			}
		}
		if (restart) {
			Time.timeScale = 0;
			GameEngine.pauseGame = true;
			Texture2D image = (Texture2D)Resources.Load ("losescene");
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), image);
			if (GUI.Button (new Rect (Screen.width * .35f, Screen.height * .5f, Screen.width * .3f, Screen.height * .15f), "Play Again", buttonStyle)) {
				restart = false;
				Application.LoadLevel(Application.loadedLevel);
				Time.timeScale = 1;
				GameEngine.pauseGame = false;
			}
		}
	}

	void InitStyles(){
		if (currentStyle == null) {
			currentStyle = new GUIStyle(GUI.skin.box);
			currentStyle.normal.background = MakeTex(2, 2, new Color( 0f, 0f, 0f, 0.5f ));
			currentStyle.fontSize = 40;
		}
		if(buttonStyle == null){
			buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.normal.textColor = Color.white;
			buttonStyle.fontSize = 40;
		}
	}

	Texture2D MakeTex(int width, int height, Color col){
		Color[] pix = new Color[width * height];
		for (int i=0; i<pix.Length; ++i) {
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

	string story(int q){
		string s = null;
		switch (q) {
		case 0:
			s = "Welcome to Tokyo city!\r\nWe know a rabbit leads you here, and in order to\r\nfind the rabbit, you must finish our puzzle challenges\r\nwhile defeating numerous enemies in the city";
			s += "\r\nThe city is divided into 2 sectors. Each sector has a\r\ndifferent game mode, 3-4 puzzles to solve, and 3 types\r\nof enemies to kill.";
			s += " Finishing a quadrant would give you\r\na piece of clue to find the rabbit that's hidden somewhere\r\nin the city.";
			s += "\r\nWarning: There will be drones in the sky dropping bombs";
//			s += "\r\nHave fun finding the rabbit, and remember: Just because you\r\ncan float like a ghost, doesn't mean you cannot die! MUAHAHAHA";
			s += "\r\n\r\nMovement controls:";
			s += "\r\nTo look around, drag on the right half of your screen";
			s += ".\r\nTo move around, drag on the left half of your screen";
			s += ".\r\nTap on the target to shoot.";
			s += "\r\nHold on the target to pick up items.";
			break;

		case 1:
			s = "To finish this quadrant, you must kill\r\n4 slimes\r\n3 tanky frogs\r\n2 necromancers\r\nwithout taking any damages";
			s += ".\r\nAchieving this goal would give you double damage for a minute.";
			s += "\r\nOn top of that, finishing 3 puzzle challenges would give you a\r\npiece of clue to find the rabbit.";
			break;
		case 2:
			s = "To finish this quadrant, you must kill\r\n5 slimes\r\n5 tanky frogs\r\n2 medics\r\nin 1 minute.";
			s += ".\r\nAchieving this goal would give you invincibility for a minute.";
			s += "\r\nOn top of that, finishing 4 puzzle challenges would give you a\r\npiece of clue to find the rabbit.";
			break;
		case 3:
			s = "Congratulations! You have finished both sectors!";
			s += "\r\nYou may have noticed the image is now complete.";
			s += "\r\n Now it's time to find the bunny.";
			break;
//		case 4:
//			s = "To finish this quadrant, you must kill\r\n# slimes\r\n# necromancers\r\n# medics\r\nAFTER solving all 4 puzzles.";
//			s += ".\r\nAchieving this goal would give you double movement speed for 2 minutes.";
//			s += "\r\nOn top of that, finishing 4 puzzle challenges would give you a piece of\r\nclue to find the rabbit.";
//			break;
		case 5:
			s = "You've solved this puzzle and a magical invisible fairy";
			s += "\r\nwithin the treasure chest has doubled your maximum";
			s += "\r\nhealth and fully healed you. Rejoice!";
			break;
		}

		return s;
	}
}
