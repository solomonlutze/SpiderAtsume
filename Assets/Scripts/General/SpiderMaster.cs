using UnityEngine;
using System.Collections;

public class SpiderMaster : UsesPlayerPrefs {

	public GameObject[] SpiderPrefabs;

	// Use this for initialization
	void Start () {
		LoadAndUpdateSpiders();
	}

	void LoadAndUpdateSpiders() {
//		TimeSpan timeSinceQuit = DataKeeper.getTimeSinceQuit();
//		print(timeSinceQuit.TotalSeconds);
//		transform.eulerAngles = new Vector3(0, 0, PlayerPrefs.GetFloat("spiderRotation", 0F));
//		print("previousRotation: " + transform.eulerAngles.z);
//		if (timeSinceQuit.TotalSeconds > 30){
//			transform.Rotate(0, 0, 30*UnityEngine.Random.Range (1, 12));
//		}
//		print("rotationNow: " + transform.eulerAngles);
//		
	}

	public override void Suspend() {
		Debug.Log ("setting spider rotation: "+transform.eulerAngles.z);
		PlayerPrefs.SetFloat("spiderRotation", transform.eulerAngles.z);
	}
	
	public override void Unsuspend() {
		LoadAndUpdateSpiders();	
	}
}
