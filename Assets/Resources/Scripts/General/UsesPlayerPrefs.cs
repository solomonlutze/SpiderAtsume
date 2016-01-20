using UnityEngine;
using System.Collections;

public class UsesPlayerPrefs : MonoBehaviour {
	
	private bool hasSuspended = true;

	void Awake() {
		DoUnuspend();
	}
	
	void OnApplicationQuit() {
		DoSuspend();
	}
	
	void OnApplicationFocus(bool focus) {
		if (focus) {
			DoUnuspend();
		} else {
			DoSuspend();
		}
	}
	
	void OnApplicationPause(bool pause) {
		if (pause) {
			DoSuspend();
		} else {
			DoUnuspend();
		}
	}

	void DoSuspend() {
		hasSuspended = true;
		Suspend();
	}

	void DoUnuspend() {
		if (hasSuspended) {
			hasSuspended = false;
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
