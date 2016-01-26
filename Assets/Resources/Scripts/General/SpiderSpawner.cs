using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiderSpawner : MonoBehaviour {
	public GameObject spiderPrefab = null;
	public GameObject mySpider = null;
	public GameObject myBait = null;
	private string baseBaitPath = "Prefabs/Bait/";
	public int minSpider = -1; // Unity C# doesn't seem to support tuples; this will work, I guess
	public int maxSpider = -1;


	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Renderer>().enabled = false;
	}
	
	void setBaitAndTimestamp(string baitName){
		DataKeeper.getAndUpdateStamp(gameObject.name+".baitSetAt");
		placeBait(baitName);
	}
	
	void placeBait(string baitName){
		if (myBait == null) {
			PlayerPrefs.SetString(gameObject.name+".myBait", baitName);
			myBait = Instantiate(Resources.Load(baseBaitPath+baitName), transform.position, transform.rotation) as GameObject;
		}
	}

	void spawnSpider() {
		if (mySpider == null && spiderPrefab != null) {
			mySpider = GameObject.Instantiate(spiderPrefab, transform.position, transform.rotation) as GameObject;
			mySpider.transform.Rotate(0, 0, 30*UnityEngine.Random.Range (1, 12));
			spiderPrefab.GetComponent<Spider>().spawnerId = gameObject.name;
			spiderPrefab.GetComponent<Spider>().name = spiderPrefab.GetComponent<Spider>().simpleName;
			PlayerPrefs.SetString(gameObject.name+".mySpider", spiderPrefab.name);
			if (PlayerPrefs.GetInt(spiderPrefab.name+".hasVisited") == 0) {
				int uniqueVisits = PlayerPrefs.GetInt("uniqueSpiderVisits");
				PlayerPrefs.SetInt("uniqueSpiderVisits", uniqueVisits+1);
			}
			PlayerPrefs.SetInt(spiderPrefab.name+".hasVisited", 1);
			int spiderVisits = PlayerPrefs.GetInt(spiderPrefab.name+".numberOfVisits", 0);
			spiderVisits = spiderVisits > 5 ? 5 : spiderVisits;
			mySpider.transform.localScale = new Vector3(
				mySpider.transform.localScale.x + mySpider.transform.localScale.x*0.1F*spiderVisits, 
				mySpider.transform.localScale.y + mySpider.transform.localScale.y*0.1F*spiderVisits); 
			if (myBait) {
				Vector3 tempScale = myBait.transform.localScale; //this shouldn't be necessary, but life is never neat.
				myBait.transform.position = mySpider.transform.position + (mySpider.transform.up * mySpider.transform.localScale.x);
				myBait.transform.localScale = tempScale;
			}
		}
	}

	public void cleanUpBait() {
		if (myBait != null) {
			GameObject.Destroy(myBait);
			myBait = null;
		}
		PlayerPrefs.DeleteKey(gameObject.name+".myBait");	
	}
	
	void cleanUpSpider() {
		if (mySpider != null) {
			spiderPrefab = null;
			GameObject.Destroy(mySpider);
			mySpider = null;
		}
		PlayerPrefs.DeleteKey(gameObject.name+".mySpider");
	}
}
