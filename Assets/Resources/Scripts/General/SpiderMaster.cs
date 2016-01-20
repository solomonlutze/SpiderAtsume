using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpiderMaster : UsesPlayerPrefs {

	private GameObject[] spiderSpawners = null;
	public GameObject[] spiderPrefabs;
	public string[] baitArray = new string[6] {"Housefly","Mosquito","Moth","Butterfly","Dragonfly","Scarab"};
	private Dictionary<string, bool> availableSpiders = new Dictionary<string, bool>();
	private Dictionary<string, GameObject> spiderPrefabDictionary = new Dictionary<string, GameObject>();
	public int secondsBetweenBaitPlacingAndEating = 15;
	public int secondsBetweenEatingAndChilling = 900; // 15 minutes
	public int secondsBetweenChillingAndLeaving = 7200; // 2 hours

	// Use this for initialization
	void Awake () {
		spiderSpawners = GameObject.FindGameObjectsWithTag("SpiderSpawner");
		PopulateAvailableSpiders();
//		LoadSpiderSpawners();
	}


	void PopulateAvailableSpiders() {
		for(int i = 0; i < spiderPrefabs.Length; i++) {
			availableSpiders.Add(spiderPrefabs[i].name, true);
			spiderPrefabDictionary.Add(spiderPrefabs[i].name, spiderPrefabs[i]);
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
			Debug.Log ("loading spider spawners!!");
			for(int i = 0; i < spiderSpawners.Length; i++) {
				TimeSpan timeSinceBaitSet = DataKeeper.getTimeSinceStamp(spiderSpawners[i].gameObject.name+".baitSetAt");
				Debug.Log ("time Since Bait Set: " +timeSinceBaitSet.TotalSeconds);
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
//			spiderSpawnerScript.SendMessage("CleanUpSpider");	//hoooping we don't need this...
			Debug.Log ("existingSpider: "+existingSpider);
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
			Debug.Log ("selected spider: " + selectedSpiderPrefab.name + "is available? " +availableSpiders[selectedSpiderPrefab.name]);
			if (availableSpiders[selectedSpiderPrefab.name]){
				spiderSpawnerScript.SendMessage("cleanUpSpider");
				availableSpiders[selectedSpiderPrefab.name] = false;
				spiderSpawnerScript.spiderPrefab = selectedSpiderPrefab;
				spiderSpawnerScript.SendMessage("spawnSpider");
			}
		} else {
			Debug.LogError("tried to spawn spider at "+spiderSpawnerName+" but either spider wasn't null or bait was null");
		}
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
			int numberOfVisits = PlayerPrefs.GetInt(existingSpider+".numberOfVisits", 0);
			Debug.Log ("number of visits for"+existingSpider+": "+(numberOfVisits +1));
			PlayerPrefs.SetInt(existingSpider+".numberOfVisits", ++numberOfVisits);
		}
	}

	public override void Suspend() {  //I thiiink this is unnecessary.
//		DataKeeper.getAndUpdateStamp("LoadSpiderSpawnerTimestamp");
//		if (spiderSpawners != null) {
//			for(int i = 0; i < spiderSpawners.Length; i++) {
//				SpiderSpawner spiderSpawnerScript = spiderSpawners[i].GetComponent<SpiderSpawner>();
//				if (spiderSpawnerScript.mySpider == null) {
//					PlayerPrefs.DeleteKey(spiderSpawners[i].gameObject.name+".mySpider");
//					if (spiderSpawnerScript.myBait == null) {
//						PlayerPrefs.DeleteKey(spiderSpawners[i].gameObject.name+".myBait");
//					} else {
//						PlayerPrefs.SetString(spiderSpawners[i].gameObject.name+".myBait", spiderSpawnerScript.myBait.name);
//					}
//
//				} else {
//					PlayerPrefs.SetString(spiderSpawners[i].gameObject.name+".mySpider", spiderSpawnerScript.mySpider.name);
//					DataKeeper.getAndUpdateStamp(spiderSpawners[i].gameObject.name+"SpiderSaveTimestamp");
//				}
//			}
//		}
	}
	
	public override void Unsuspend() {
		LoadSpiderSpawners();	
	}
}

//stuff I took out:

//				string existingSpider = PlayerPrefs.GetString (spiderSpawners[i].gameObject.name+".mySpider","");
//				string existingBait = PlayerPrefs.GetString (spiderSpawners[i].gameObject.name+".myBait","");
//				SpiderSpawner spiderSpawnerScript = spiderSpawners[i].GetComponent<SpiderSpawner>();
//				if (existingSpider != ""){
//					if (DataKeeper.getAndUpdateStamp(spiderSpawners[i].gameObject.name+"SpiderSaveTimestamp").TotalSeconds > 30) { //clean out this spider
//						availableSpiders[existingSpider] = true;
//						spiderSpawnerScript.SendMessage("CleanUpSpider");
//					} else {
//						spiderSpawnerScript.SendMessage("CleanUpSpider");
//						spiderSpawnerScript.spiderPrefab = spiderPrefabDictionary[existingSpider];
//						availableSpiders[existingSpider] = false;
//						spiderSpawnerScript.SendMessage("SpawnSpider");
//					}
//				} else if (existingBait != ""){
//					spiderSpawnerScript.SendMessage("placeBait", existingBait);
//					PlayerPrefs.DeleteKey(spiderSpawners[i].gameObject.name+".myBait");
//				}
//			}
//			if (DataKeeper.getAndUpdateStamp("LoadSpiderSpawnerTimestamp").TotalSeconds > 15) {
//				for(int i = 0; i < spiderSpawners.Length; i++) {
//					SpiderSpawner spiderSpawnerScript = spiderSpawners[i].GetComponent<SpiderSpawner>();
//					if (spiderSpawnerScript.mySpider == null && spiderSpawnerScript.myBait != null) {
//						Bait baitScript = spiderSpawnerScript.myBait.GetComponent<Bait>();
//						int min = baitScript.spiderMin;
//						int max = baitScript.spiderMax;
//						GameObject selectedSpiderPrefab = spiderPrefabs[Random.Range(min, max)];
//						Debug.Log ("selected spider: " + selectedSpiderPrefab.name + "is available? " +availableSpiders[selectedSpiderPrefab.name]);
//						if (availableSpiders[selectedSpiderPrefab.name]){
//							spiderSpawnerScript.SendMessage("CleanUpSpider");
//							availableSpiders[selectedSpiderPrefab.name] = false;
//							spiderSpawnerScript.spiderPrefab = selectedSpiderPrefab;
//							spiderSpawnerScript.SendMessage("SpawnSpider");
//						}
//					}
//				}
//			}
