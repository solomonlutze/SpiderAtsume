using UnityEngine;
using System.Collections;

public class UsesPlayerPrefs : MonoBehaviour {
	
	
	void Awake() {
		Debug.Log("wake up!");
		Unsuspend();
	}
	
	void OnApplicationQuit() {
		Debug.Log("onApplicationQuit");
		Suspend();
	}
	
	void OnApplicationFocus(bool focus) {
		if (focus) {
			Unsuspend();
		} else {
			Suspend();
		}
	}
	
	void OnApplicationPause(bool pause) {
		if (pause) {
			Suspend();
		} else {
			Unsuspend();
		}
	}

	public virtual void Suspend() {
		//extend me
	}
	
	public virtual void Unsuspend() {
		//extend me
	}
}
