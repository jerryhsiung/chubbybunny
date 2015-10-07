using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameEngine : MonoBehaviour {
	[HideInInspector]
	public static bool pauseGame;

	[HideInInspector]
	public enum MonsterChallenges {
		KILL_TIME,		//	Kill x monsters in t time
		KILL_NO_DAMAGE,	//	Kill x monsters without taking damage
		PASSED			//	Passed the challenge
	};

	public Material nightSkybox;
	public AudioClip partySong;

	public GameObject player;
	private PlayerController playerController;

	//	GUI stuff
	public Text centerText;
	public Text livesText;
	public RectTransform fadeOut;

	public Transform respawnPoint;
	public int numLives;

	//	All sectors and the current active sector (the sector player is in)
	public GameObject droneGenerator;
	public SectorEngine[] sectorEngines;
	private SectorEngine activeSector;

	public Transform maidCafeTele;
	[HideInInspector]
	public bool startTheParty;
	[HideInInspector]
	public bool gameOver;

	private bool playerDied;

	void Start() {
		pauseGame = false;
		playerController = player.GetComponent<PlayerController>();
		livesText.color = Color.green;

		centerText.text = "";
		startTheParty = playerDied = gameOver = false;

		RenderSettings.ambientLight = new Color (80f / 255, 80f / 255, 80f / 255);
	}
	
	void Update () {
		//	Update lives text
		livesText.text = numLives.ToString();

		if (playerDied) {
			//	Click or tap to respawn
			if(Input.GetMouseButtonDown(0) || Input.touchCount > 0) {
				//	Deactivate sector engines
				for(int i = 0; i < sectorEngines.Length; ++i)
					sectorEngines[i].deactivateSector();

				centerText.text = "";

				//	Reset player
				playerController.health = playerController.maxHealth;
				playerController.dead = false;
				playerController.setMove(true, 0);
				//	Respawn
				player.transform.position = respawnPoint.position;
				Time.timeScale = 1f;
				playerDied = false;
			}
		}
		//	Check if player was killed
		else if(playerController.dead == true) {
			--numLives;
			if(numLives <= 0) {
				//	Game over
				livesText.color = Color.red;
				gameOver = true;
				Time.timeScale = 0;
			}

			else {
				//	Respawn
				centerText.color = Color.red;
				centerText.text = "YOU DIED\nTap to respawn";
				playerDied = true;
				playerController.dead = false;
				Time.timeScale = 0;
			}
		}
	}

	//	Called by sector engines when they're de/activated
	public void setActiveSector(SectorEngine sector) {
		activeSector = sector;
	}
	public void deactivateSector(SectorEngine sector) {
		if (activeSector == sector)
			activeSector = null;
	}

	//	Called by player controller whenever player is hurt
	public void playerHit() {
		if(activeSector)
			activeSector.playerHit ();
	}

	//	The win party scene
	public void partyDontStartTilIWalkIn() {
		//	Stop all engines and monster gens
		for (int i = 0; i < sectorEngines.Length; ++i) {
			sectorEngines[i].deactivateSector();
		}
		droneGenerator.GetComponent<DroneGenerator> ().killAllDrones ();

		//	Stop bg music and play party song
		Camera.main.audio.Stop ();
		audio.PlayOneShot (partySong);

		//	Set skybox to night
		Camera.main.GetComponent<Skybox> ().material = nightSkybox;

		//	Set the party lighting
 		Light light = GameObject.Find ("Directional light").GetComponent<Light> ();
		light.intensity = 0.02f;
		light.color = new Color (1f, 92f/255, 92f/255);
		RenderSettings.ambientLight = new Color (0, 0, 0);

		//	Enable lighting when the bass drops
		Invoke ("turnOnLights", 2.5f);

		//	Fade to end screen
		Invoke ("activateFadeOut", 20f);
		Invoke ("reloadGame", 28f);
	}

	//	Called by sector engines when they complete
	public void sectorComplete() {
		if (sectorEngines [0].finished && sectorEngines [1].finished) {
			maidCafeTele.gameObject.SetActive(true);
		}
	}

	void turnOnLights() {
		GameObject partyKit = GameObject.Find ("Party Kit");

		Light[] lights = partyKit.GetComponentsInChildren<Light>();
		for (int i = 0; i < lights.Length; ++i)
			lights [i].enabled = true;
	}

	void reloadGame() {
		Application.LoadLevel ("MenuScene");
	}

	void activateFadeOut() {
		fadeOut.GetComponent<FadeOut> ().activate = true;
	}
}
