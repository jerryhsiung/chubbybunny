using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SectorEngine : MonoBehaviour {

	public GameObject gameEngine;

	//	The monster challenge and data for it
	public GameEngine.MonsterChallenges sectorChallenge;
	public GameModeData gameData;

	//	Player and monster gen
	public GameObject player;
	private MonsterGenerator monsterGenerator;
	PlayerController playercontroller;

	public Transform[] spawnPoints;

	//	If this is active sector
	public bool isActive;

	//puzzle GUI
	int puzzle_count;
	[HideInInspector]
	public bool treasureFound;

	//	GUI texts
	public Text[] enemyTexts;
	public Text timerText;
	public Text puzzleText;
//	public Text rewardText;
	public Text invincibleReward;
	public Text doubleDReward;

	//If this sector's challenges are finished
//	[HideInInspector]
	public bool finished;
//	[HideInInspector]
	public bool showGUI;

	bool killtime_success = false;
	bool nodamage_success = false;

	//timer for rewards
	float countdown = 60;
	public bool rewardTime_done = false;
	public bool rewardNoDamage_done = false;

	void Start () {
		playercontroller = player.GetComponent<PlayerController> ();
		monsterGenerator = this.GetComponent<MonsterGenerator> ();
		monsterGenerator.setSpawnPoints (spawnPoints);
		monsterGenerator.parentSector = this;
		treasureFound = false;

		//	Construct and set goals
		switch (sectorChallenge) {
		case GameEngine.MonsterChallenges.KILL_TIME:
			gameData = new GameModeData.KillTimeModeData();

			/*******	SET THE GOALS HERE	*******/
			((GameModeData.KillTimeModeData)gameData).setKills(new int[] { 5, 5, 0, 2, 0});
			((GameModeData.KillTimeModeData)gameData).setTime(60f);
			
			break;

		case GameEngine.MonsterChallenges.KILL_NO_DAMAGE:
			gameData = new GameModeData.KillNoDamageModeData();

			/*******	SET THE GOALS HERE	*******/
			((GameModeData.KillNoDamageModeData)gameData).setKills(new int[] {4, 3, 2, 0, 0});

			break;
		}

		//	Set texts to blank
		timerText.text = "";
		if (this.gameObject.name == "Sector1Engine") {
			puzzleText.text = "X";
			puzzle_count = 3;
		} 
		else {
			puzzleText.text = "X";
			puzzle_count = 4;
		}

		for (int i = 0; i < enemyTexts.Length; ++i)
			enemyTexts [i].text = "X";

		gameData.parentEngine = this;
	}
	
	void Update () {
		if (isActive) {
			//	Update GUI texts
			switch (sectorChallenge) {
			case GameEngine.MonsterChallenges.KILL_TIME:

				GameModeData.KillTimeModeData killTimeData = (GameModeData.KillTimeModeData)gameData;
			
				//	Set the kill texts (goal kills - curr kills)
				for(int i = 0; i < 4; ++i) {	
					enemyTexts[i].text = (killTimeData.getKillGoalFor (i) - killTimeData.getKillsFor (i)).ToString();
				}

				//	Add time if the timer is going
				if(killTimeData.startTimer && !killTimeData.success)
					killTimeData.addTime(Time.deltaTime);

				//	Update the timer text
				timerText.text = killTimeData.getCurrTime().ToString("F2") + "/"
					+ killTimeData.killTime.ToString("F2");

				//finish kill challenge - receive one minute invincibility
				if(killTimeData.call_invincible && !playercontroller.donewithInvincible){
					playercontroller.invincibility();
					killTimeData.call_invincible = false;
					invincibleReward.text = countdown.ToString("F2");
				}

				if(killTimeData.success){
					killtime_success = true;
				}

				//finish both kill and puzzle challenge
				if(killTimeData.success && puzzle_count ==0){
					finished = true;
					showGUI = true;
					playercontroller.particleSystem.Play ();
					gameEngine.GetComponent<GameEngine>().sectorComplete();
				}
				
				break;

			case GameEngine.MonsterChallenges.KILL_NO_DAMAGE:

				GameModeData.KillNoDamageModeData noDamage = (GameModeData.KillNoDamageModeData)gameData;

				//	Set the kill texts (curr kills / goal kills)
				for(int i = 0; i < 4; ++i) {
					enemyTexts[i].text = (noDamage.getKillGoalFor (i) - noDamage.getKillsFor (i)).ToString();
				}

				//finish no damage challenge - receive one minute of double damage
				if(noDamage.call_doubleDamage && !playercontroller.donewithDoubleDamage){
					playercontroller.doubleDamagePower();
					noDamage.call_doubleDamage = false;
					doubleDReward.text = countdown.ToString("F2");
				}

				if(noDamage.success){
					nodamage_success = true;
				}

				//finish both kill and puzzle challenge
				if(noDamage.success && puzzle_count == 0){
					finished = true;
					showGUI = true;
					playercontroller.particleSystem.Play ();
					gameEngine.GetComponent<GameEngine>().sectorComplete();
				}

				break;
			}
			//Update GUI texts for puzzle
			puzzleText.text = puzzle_count.ToString();
		}
		if(killtime_success && !rewardTime_done){
			playercontroller.invincible = true;
			countdown -=Time.deltaTime;
			invincibleReward.text = countdown.ToString("F2");
			if(countdown<=0){
				rewardTime_done = true;
				invincibleReward.text = "";
			}
		}
		if(nodamage_success && !rewardTime_done){
			playercontroller.doubleDamage = true;
			countdown -=Time.deltaTime;
			doubleDReward.text = countdown.ToString("F2");
			if(countdown<=0){
				rewardTime_done = true;
				doubleDReward.text = "";
			}
		}
	}

	//Called after finishing a puzzle
	public void donePuzzle(){
		if (puzzle_count > 0) {
			puzzle_count--;
		}
	}

	//	Called by the game data classes
	public void setTextColor(int index, Color color) {
		Text text = enemyTexts [index];
		text.color = color;
	}
	//	Called by the timer game data class
	public void setTimerTextGreen() {
		timerText.color = Color.green;
	}

	//	Called by monster gen to record new kill
	public void addKill(MonsterGenerator.MonsterTypes type) {
		if(sectorChallenge == GameEngine.MonsterChallenges.KILL_TIME) {
			//	Add kill and start timer if needed
			GameModeData.KillTimeModeData killTimeData = (GameModeData.KillTimeModeData)gameData;
			killTimeData.addKill((int)type);
			if(!killTimeData.startTimer)
				killTimeData.startTimer = true;
		}
		else if(sectorChallenge == GameEngine.MonsterChallenges.KILL_NO_DAMAGE) {
			//	Just add kill
			GameModeData.KillNoDamageModeData noDamage = (GameModeData.KillNoDamageModeData)gameData;
			noDamage.addKill((int)type);
		}
	}

	//	Called by game engine if player hit
	public void playerHit() {
		if (sectorChallenge == GameEngine.MonsterChallenges.KILL_NO_DAMAGE) {
			GameModeData.KillNoDamageModeData noDamage = (GameModeData.KillNoDamageModeData)gameData;
			Debug.Log ("reset no damage challenge");
			noDamage.resetChallenge();
		}
	}

	//	Called by the sector colliders to de/activate this engine
	public void activateSector() {
		isActive = true;
		monsterGenerator.isActive = true;
		gameEngine.GetComponent<GameEngine> ().setActiveSector (this);

		//	Make completed tasks green again
		if (sectorChallenge == GameEngine.MonsterChallenges.KILL_TIME) {
			GameModeData.KillTimeModeData killTimeData = (GameModeData.KillTimeModeData)gameData;
			for(int i = 0; i < killTimeData.currKills.Length; ++i) {
				if(killTimeData.currKills[i] >= killTimeData.killGoals[i]) {
					enemyTexts[i].color = Color.green;
				}
			}

			if(killTimeData.success) {
				timerText.color = Color.green;
			}
		}
		else {
			//	Same here
			GameModeData.KillNoDamageModeData noDamage = (GameModeData.KillNoDamageModeData)gameData;
			for(int i = 0; i < noDamage.currKills.Length; ++i) {
				if(noDamage.currKills[i] >= noDamage.killGoals[i]) {
					enemyTexts[i].color = Color.green;
				}
			}
		}
	}
	public void deactivateSector() {
		isActive = false;
		gameEngine.GetComponent<GameEngine> ().deactivateSector (this);

		//	Reset all texts
		for (int i = 0; i < enemyTexts.Length; ++i) {
			enemyTexts [i].color = Color.red;
			enemyTexts[i].text = "X";
		}
		timerText.color = Color.red;
		timerText.text = "";
		puzzleText.text = "X";

		monsterGenerator.deactivate ();
	}
}
