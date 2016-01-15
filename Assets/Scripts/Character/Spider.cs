using UnityEngine;
using System.Collections;
using System;

public class Spider : UsesPlayerPrefs {

	// Use this for initialization
	void Start () {
		LoadAndUpdateStatus();
	}

	void LoadAndUpdateStatus() {
		TimeSpan timeSinceQuit = DataKeeper.getTimeSinceQuit();
		print(timeSinceQuit.TotalSeconds);
		transform.eulerAngles = new Vector3(0, 0, PlayerPrefs.GetFloat("spiderRotation", 0F));
		print("previousRotation: " + transform.eulerAngles.z);
		if (timeSinceQuit.TotalSeconds > 30){
			transform.Rotate(0, 0, 30*UnityEngine.Random.Range (1, 12));
		}
		print("rotationNow: " + transform.eulerAngles);

	}

	// Update is called once per frame
	void Update () {
	
	}

	void Touch() {
		transform.Rotate(0, 0, 90);
	}

	public override void Suspend() {
		Debug.Log ("setting spider rotation: "+transform.eulerAngles.z);
		PlayerPrefs.SetFloat("spiderRotation", transform.eulerAngles.z);
	}
	
	public override void Unsuspend() {
		LoadAndUpdateStatus();	
	}
}
