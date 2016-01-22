using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasHandler : MonoBehaviour {
	public GameObject playCanvas;
	public GameObject bugPickerCanvas;
	public GameObject bugPlacerCanvas;
	public GameObject collectionCanvas;
	public GameObject announceDeathPrefab;
	private Text myDeathAnnouncement;
	public GameObject[] spawnerButtons = null;

	// We could also just say that only one canvas can be active at a time,
	// have events activate a particular canvas, and then deactivate all the other ones.
	// It's not worth a refactor at the moment though.

	void Start () {
		bugPickerCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
		collectionCanvas.SetActive(false);
	}

	public void OpenMenu () {
		playCanvas.SetActive(false);
		bugPickerCanvas.SetActive(true);
	}

	public void OpenCollection() {
		playCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
		bugPickerCanvas.SetActive(false);
		collectionCanvas.SetActive(true);
		collectionCanvas.SendMessage("displayCollectionButtons");

	}
	public void CloseMenu () {
		collectionCanvas.SendMessage("resetPage");
		playCanvas.SetActive(true);
		bugPlacerCanvas.SetActive(false);
		bugPickerCanvas.SetActive(false);
		collectionCanvas.SetActive(false);
	}

	public void PickBug(string pickedBug) {
		bugPickerCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(true);

		foreach (GameObject button in spawnerButtons) {
			button.SendMessage("setBaitType", pickedBug);
		}
	}

	public void placeBait() {
		bugPickerCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
		playCanvas.SetActive(true);
	}

	public void announceDeath(string killedSpider, string killerSpider) {
		myDeathAnnouncement = GameObject.Instantiate(announceDeathPrefab).GetComponent<Text>();
		myDeathAnnouncement.text = killedSpider + " killed by " + killerSpider;
		myDeathAnnouncement.transform.SetParent(playCanvas.transform, false);
	}

}
