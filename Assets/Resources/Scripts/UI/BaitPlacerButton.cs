using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaitPlacerButton : MonoBehaviour {

	public string baitName;
	public GameObject mySpiderSpawner;
	public SpiderMaster spiderMaster;

	void Start () {
		gameObject.GetComponent<Button>().onClick.AddListener(placeBait);
	}

	void placeBait() {
		spiderMaster.cleanUpBait(mySpiderSpawner.GetComponent<SpiderSpawner>());
		spiderMaster.cleanUpSpider(mySpiderSpawner.name, mySpiderSpawner.GetComponent<SpiderSpawner>());
		mySpiderSpawner.SendMessage("setBaitAndTimestamp", baitName);
	}

	void setBaitType(string baitType) {
		baitName = baitType;
	}
}
