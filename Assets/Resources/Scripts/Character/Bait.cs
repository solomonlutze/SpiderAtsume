using UnityEngine;
using System.Collections;

public class Bait : MonoBehaviour {
	public int spiderMin = 0;
	public int spiderMax = 0;
	public string simpleName = "";

	// Use this for initialization
	void Start () {
		gameObject.name = simpleName;
		transform.Rotate(0, 0, 30*UnityEngine.Random.Range (1, 12));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
