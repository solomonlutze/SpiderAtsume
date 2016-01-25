using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpiderCollectionCardButton : MonoBehaviour {

	public string mySpider;
	public GameObject myCanvas;
	
	void Start () {
		gameObject.GetComponentInChildren<Button>().onClick.AddListener(openDescription);
	}
	
	void openDescription() {
		myCanvas.SendMessage("openDescription", mySpider);
	}
}