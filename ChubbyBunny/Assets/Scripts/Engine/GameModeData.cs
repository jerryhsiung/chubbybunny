using UnityEngine;
using System.Collections;

public abstract class GameModeData {

	public SectorEngine parentEngine;

	//	If the challenge has been beaten
	public bool success;


	/*******	DERIVING CLASSES	*******/

	public class KillTimeModeData : GameModeData {
		//	Start the kill timer
		public bool startTimer;
		//	The kill goals
		public int[] killGoals;
		//	Current kills
		public int[] currKills;
		// Check if each enemy challenge is done
		public bool[] enemyDone;

		public float killTime;
		protected float currTime;

		//give player invincibility when success
		public bool call_invincible = false;
		
		public KillTimeModeData() {
			success = false;
			startTimer = false;
			currTime = 0;
			killGoals = new int[4];
			currKills = new int[4];
			enemyDone = new bool[4];
			for(int i = 0; i<4; i++){
				enemyDone[i] = false;
			}
		}

		//	Set the kill goals
		public void setKills(int[] goals) {
			killGoals = goals;
		}

		//	Add a kill of monster type
		public void addKill(int index) {
			if(!enemyDone[index]){
				++currKills[index];
			}
			//	Check for success
			bool win = true;
			for(int i = 0; i < 4; ++i) {
				if(currKills[i] < killGoals[i])
					win = false;
				else {
					//	Change text to green while we're at it
					parentEngine.setTextColor(i, Color.green);
					enemyDone[i] = true;
				}
			}
			if (win) {
				success = true;
				call_invincible = true;
				parentEngine.setTimerTextGreen ();
			}
		}

		//	Get number of kills for monster
		public int getKillsFor(int index) {
			return currKills [index];
		}
		//	Get kill goal for monster
		public int getKillGoalFor(int index) {
			return killGoals [index];
		}
		//	Set the time limit
		public void setTime(float limit) {
			killTime = limit;
		}
		//	Add time to the clock
		public void addTime(float delta) {
			currTime += delta;
			
			//	Check if passed time limit
			if (currTime > killTime) {
				currTime = 0;
				startTimer = false;
				
				//	Reset current kills and text colors
				for(int i = 0; i < currKills.Length; ++i) {
					currKills[i] = 0;
					enemyDone[i] = false;
					parentEngine.setTextColor(i, Color.red);
				}
				
			}
		}
		public float getCurrTime() {
			return currTime;
		}
	}

	public class KillNoDamageModeData : GameModeData {
		//	The kill goals
		public int[] killGoals;
		//	Current kills
		public int[] currKills;
		// Check if each enemy challenge is done
		public bool[] enemyDone;

		//give player double damage when success
		public bool call_doubleDamage = false;

		public KillNoDamageModeData() {
			success = false;
			killGoals = new int[4];
			currKills = new int[4];
			enemyDone = new bool[4];
			for(int i = 0; i<4; i++){
				enemyDone[i] = false;
			}
		}

		//	Add a kill of monster type
		public void addKill(int index) {
			if (!enemyDone [index]) {
				++currKills[index];
			}
			//	Check for success
			bool win = true;
			for(int i = 0; i < 4; ++i) {
				if(currKills[i] < killGoals[i]){
					win = false;
				}
				else {
					//	Change text to green while we're at it
					Debug.Log("gonna change color");
					parentEngine.setTextColor(i, Color.green);
					enemyDone[i] = true;
					Debug.Log(i+" done");
					Debug.Log(currKills[i]+"/"+killGoals[i]);
				}
			}
			if (win) {
				success = true;
				call_doubleDamage = true;
			}
		}

		//	If player got hit
		public void resetChallenge() {
			if (!success) {
				for (int i = 0; i < 4; ++i) {
					if(killGoals[i] != 0) {
						currKills [i] = 0;
						enemyDone[i] = false;
						parentEngine.setTextColor (i, Color.red);
					}
				}
			}
		}

		//	Set the kill goals
		public void setKills(int[] goals) {
			killGoals = goals;
		}
		//	Get number of kills for monster
		public int getKillsFor(int index) {
			return currKills [index];
		}
		//	Get kill goal for monster
		public int getKillGoalFor(int index) {
			return killGoals [index];
		}
	}
}
