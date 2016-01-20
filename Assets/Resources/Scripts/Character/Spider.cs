using UnityEngine;
using System.Collections;
using System;

public class Spider : MonoBehaviour {
	public String spawnerId;
	public String simpleName;

	void Start () {
		gameObject.name = simpleName;
	}
	
	void Touch() {
		transform.Rotate(0, 0, 30*UnityEngine.Random.Range (1, 12));
	}
	
}
