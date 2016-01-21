using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaitPlacerButton : MonoBehaviour {

	public string baitName;
	public GameObject mySpiderSpawner;

	void Start () {
		gameObject.GetComponent<Button>().onClick.AddListener(placeBait);
	}

	void placeBait() {
		mySpiderSpawner.SendMessage("cleanUpBait");
		mySpiderSpawner.SendMessage("cleanUpSpider");
		mySpiderSpawner.SendMessage("setBaitAndTimestamp", baitName);
	}

	void setBaitType(string baitType) {
		baitName = baitType;
	}
}
