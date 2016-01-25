using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpiderMaster : UsesPlayerPrefs {

	private GameObject[] spiderSpawners = null;
	public GameObject[] spiderPrefabs;
	public GameObject canvasHandler;
	public string[] baitArray = new string[6] {"Housefly","Mosquito","Moth","Butterfly","Dragonfly","Scarab"};
	private Dictionary<string, bool> availableSpiders = new Dictionary<string, bool>();
	private Dictionary<string, GameObject> spiderPrefabDictionary = new Dictionary<string, GameObject>();
	public int secondsBetweenBaitPlacingAndEating = 15;
	public int secondsBetweenEatingAndChilling = 900; // 15 minutes
	public int secondsBetweenChillingAndLeaving = 7200; // 2 hours

	// Use this for initialization
	void Awake () {
		spiderSpawners = GameObject.FindGameObjectsWithTag("SpiderSpawner");
		populateAvailableSpiders();
	}
	

	void populateAvailableSpiders() {
		for(int i = 0; i < spiderPrefabs.Length; i++) {
			availableSpiders.Add(spiderPrefabs[i].name, true);
			if (PlayerPrefs.GetString(spiderPrefabs[i].name+".killedBy", "") != "") {
				availableSpiders[spiderPrefabs[i].name] = false;
			}
			spiderPrefabDictionary.Add(spiderPrefabs[i].name, spiderPrefabs[i]);
		}
		for(int i = 0; i < spiderSpawners.Length; i++) { //This loop prevents spawning a spider randomly who already was loaded at another spawner.
			string existingSpider = PlayerPrefs.GetString (spiderSpawners[i].name+".mySpider","");
			if (existingSpider != "") {
				availableSpiders[existingSpider] = false;
			}
		}
		canvasHandler.GetComponent<CanvasHandler>().showWinner("Bitsy");
		if (PlayerPrefs.GetInt("totalSpidersKilled") == spiderPrefabs.Length - 1) {
			string winnerSpider = "MissingSpider";
			foreach (string potentialWinner in availableSpiders.Keys) {
				if (availableSpiders[potentialWinner]) {
					winnerSpider = potentialWinner;
					break;
				}
			}
			canvasHandler.GetComponent<CanvasHandler>().showWinner(winnerSpider);
		}

	}

	//if baitSetAt < 60 seconds, spawn the bait
	//else if baitSetAt < 15 minutes, spawn the bait plus the spider
		//if there's already a spider saved, spawn that one
		//if not, pick one and save it
	//else if baitSetAt < 2 hours, spawn just the spider
		//if there is bait but no spider, spawn the bait, then the spider, then clean up the bait
		//if there is spider, spawn it and clean up the bait
	//else clean up
		//delete baitSetAt
		//clear mybait + playerpref
		//clear myspider + playerpref

	void LoadSpiderSpawners() {
		if (spiderSpawners != null) {
			for(int i = 0; i < spiderSpawners.Length; i++) {
				TimeSpan timeSinceBaitSet = DataKeeper.getTimeSinceStamp(spiderSpawners[i].gameObject.name+".baitSetAt");
				if (timeSinceBaitSet.TotalSeconds > 0) {  // Should only ever be 0 if the stamp isn't set
					SpiderSpawner spiderSpawnerScript = spiderSpawners[i].GetComponent<SpiderSpawner>();
					string spiderSpawnerName = spiderSpawners[i].gameObject.name;
					if (timeSinceBaitSet.TotalSeconds < secondsBetweenBaitPlacingAndEating) {  // bait sitting out
						spawnBait(spiderSpawnerName, spiderSpawnerScript);
					} else if (timeSinceBaitSet.TotalSeconds < secondsBetweenEatingAndChilling) {	// spider nomming bait
						spawnBait(spiderSpawnerName, spiderSpawnerScript);
						spawnSpider(spiderSpawnerName, spiderSpawnerScript);
					} else if (timeSinceBaitSet.TotalSeconds < secondsBetweenChillingAndLeaving) { // spider chilling
						spawnBait(spiderSpawnerName, spiderSpawnerScript);
						spawnSpider(spiderSpawnerName, spiderSpawnerScript);
						cleanUpBait(spiderSpawnerScript);
					} else {  // clean up and note how many times it has visited
						spawnBait(spiderSpawnerName, spiderSpawnerScript);
						spawnSpider(spiderSpawnerName, spiderSpawnerScript);
						updateSpiderVisits(spiderSpawnerName);
						cleanUpBait(spiderSpawnerScript);
						cleanUpSpider(spiderSpawnerName, spiderSpawnerScript);
						PlayerPrefs.DeleteKey(spiderSpawnerName+".baitSetAt");
					}
				}
			}
		}
	}

	public void spawnBait(string spiderSpawnerName, SpiderSpawner spiderSpawnerScript) {
		string existingBait = PlayerPrefs.GetString (spiderSpawnerName+".myBait","");
		if (existingBait != "") { //there should only ever *not* be bait if a spider's already chlling
			spiderSpawnerScript.SendMessage("placeBait", existingBait);
//			PlayerPrefs.DeleteKey(spiderSpawners[i].gameObject.name+".myBait"); //was this line necessary?
		} 
	}

	public void spawnSpider(string spiderSpawnerName, SpiderSpawner spiderSpawnerScript) {
		string existingSpider = PlayerPrefs.GetString (spiderSpawnerName+".mySpider","");
		if (existingSpider != "") {
			spiderSpawnerScript.spiderPrefab = spiderPrefabDictionary[existingSpider];
			availableSpiders[existingSpider] = false;
			spiderSpawnerScript.SendMessage("spawnSpider");
		} else {
			spawnNewSpiderFromBait(spiderSpawnerName, spiderSpawnerScript);
		}
	}

	public void spawnNewSpiderFromBait(string spiderSpawnerName, SpiderSpawner spiderSpawnerScript) {
		if (spiderSpawnerScript.mySpider == null && spiderSpawnerScript.myBait != null) {
			Bait baitScript = spiderSpawnerScript.myBait.GetComponent<Bait>();
			int min = baitScript.spiderMin;
			int max = baitScript.spiderMax;
			GameObject selectedSpiderPrefab = spiderPrefabs[UnityEngine.Random.Range(min, max)];
			if (availableSpiders[selectedSpiderPrefab.name]){
				int numberOfVisits = PlayerPrefs.GetInt(selectedSpiderPrefab.name+".numberOfVisits");
				PlayerPrefs.SetInt (selectedSpiderPrefab.name+".numberOfVisits", numberOfVisits+1);
				int numberOfCompletedVisits = PlayerPrefs.GetInt(selectedSpiderPrefab.name+".numberOfCompletedVisits");
				if (numberOfCompletedVisits >= 3) {
					maybeKillSpider(selectedSpiderPrefab.name);
				}
				spiderSpawnerScript.SendMessage("cleanUpSpider");
				availableSpiders[selectedSpiderPrefab.name] = false;
				spiderSpawnerScript.spiderPrefab = selectedSpiderPrefab;
				spiderSpawnerScript.SendMessage("spawnSpider");
			}
		} else {
			Debug.Log("tried to spawn spider at "+spiderSpawnerName+" but either spider wasn't null or bait was null");
		}
	}

	public void maybeKillSpider(string killerSpider) {
		for(int i = 0; i < spiderSpawners.Length; i++) {
			if (spiderSpawners[i].GetComponent<SpiderSpawner>().mySpider != null) {
				killSpider(spiderSpawners[i].GetComponent<SpiderSpawner>(), killerSpider);
				break;
			}
		}
	}

	public void killSpider(SpiderSpawner spiderSpawner, string killerSpiderName) {
		int spidersKilled = PlayerPrefs.GetInt("totalSpidersKilled");
		PlayerPrefs.SetInt("totalSpidersKilled", spidersKilled+1);
		PlayerPrefs.SetString(spiderSpawner.mySpider.name+".killedBy", killerSpiderName);
		string killedSpiderName = spiderSpawner.mySpider.GetComponent<Spider>().simpleName;
		cleanUpBait(spiderSpawner);
		cleanUpSpider(spiderSpawner.gameObject.name, spiderSpawner);
		canvasHandler.GetComponent<CanvasHandler>().displaySpiderPresent(killedSpiderName, killerSpiderName);
	}
	public void cleanUpBait(SpiderSpawner spiderSpawnerScript) {
		spiderSpawnerScript.SendMessage("cleanUpBait");
	}

	public void cleanUpSpider(string spiderSpawnerName, SpiderSpawner spiderSpawnerScript) {
		string existingSpider = PlayerPrefs.GetString(spiderSpawnerName+".mySpider","");
		availableSpiders[existingSpider] = true;
		spiderSpawnerScript.SendMessage("cleanUpSpider");
	}
	
	public void updateSpiderVisits(string spiderSpawnerName) {
		string existingSpider = PlayerPrefs.GetString(spiderSpawnerName+".mySpider","");
		if (existingSpider != "") {
			int numberOfCompletedVisits = PlayerPrefs.GetInt(existingSpider+".numberOfCompletedVisits", 0);
			Debug.Log ("number of visits for"+existingSpider+": "+(numberOfCompletedVisits +1));
			PlayerPrefs.SetInt(existingSpider+".numberOfCompletedVisits", numberOfCompletedVisits+1);
		}
	}

	public void resetEverything() {
		for(int i = 0; i < spiderSpawners.Length; i++) {
			cleanUpBait(spiderSpawners[i].GetComponent<SpiderSpawner>());
			cleanUpSpider(spiderSpawners[i].gameObject.name, spiderSpawners[i].GetComponent<SpiderSpawner>());
		}
		LoadSpiderSpawners();
	}

	public override void Unsuspend() {
		LoadSpiderSpawners();	
	}
}

