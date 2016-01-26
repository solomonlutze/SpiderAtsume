using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;

public class CanvasHandler : MonoBehaviour {
	public GameObject playCanvas;
	public GameObject bugPickerCanvas;
	public GameObject bugPlacerCanvas;
	public GameObject collectionCanvas;
	public GameObject cutsceneCanvas;
	public GameObject creditsCanvas;
	public GameObject titleCanvas;
	public GameObject spiderMaster;
	public GameObject presentPrefab;
	public GameObject announceDeathPrefab;
	public GameObject winnerPrefab;
	public Toggle audioToggle;
	public GameObject audioSource;
	private string spiderToKill;
	private string spiderWhoKilled;
	private GameObject spiderPresent;
	private GameObject deathAnnouncement;
	private GameObject spiderWinner;
	public GameObject[] spawnerButtons = null;
	private string baseImagesPath = "Art/Character/Spider";
	private Object[] spiderSpriteList;
	private Dictionary<string, Sprite> spiderSprites = new Dictionary<string, Sprite>();

	// We could also just say that only one canvas can be active at a time,
	// have events activate a particular canvas, and then deactivate all the other ones.
	// It's not worth a refactor at the moment though.
	// Why redundantly set canvases inactive? Because I don't trust them.

	void Awake () {
		if (PlayerPrefs.GetInt("disableAudio") == 1) {
			audioToggle.isOn = false;
			audioSource.GetComponent<AudioSource>().Stop();
		} else {
			audioSource.GetComponent<AudioSource>().Play();
		}
		bugPickerCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
		collectionCanvas.SetActive(false);
		creditsCanvas.SetActive(false);
		spiderSpriteList = Resources.LoadAll("Art/Character/Spider", typeof(Sprite)); //duplicate logic appears in SpiderCollection. 
		foreach (Sprite sprite in spiderSpriteList) {								  //We could make a static script to do this.
			spiderSprites.Add(sprite.name, sprite);			
		}
		StartCoroutine("displayTitleCanvas");
	}

	public IEnumerator displayTitleCanvas() {
		playCanvas.SetActive(false);
		titleCanvas.SetActive(true);
		yield return new WaitForSeconds (2);
		CloseMenu ();
	}

	public void OpenMenu () {
		playCanvas.SetActive(false);
		cutsceneCanvas.SetActive(false);
		bugPickerCanvas.SetActive(true);
		bugPickerCanvas.SendMessage ("populateAvailableBait");
	}

	public void OpenCollection() {
		playCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
		bugPickerCanvas.SetActive(false);
		collectionCanvas.SetActive(true);
		collectionCanvas.SendMessage("displayCollectionButtons");

	}
	public void CloseMenu () {
		titleCanvas.SetActive(false);
		if (collectionCanvas.activeSelf) {
			collectionCanvas.SendMessage("resetPage");
		}
		playCanvas.SetActive(true);
		bugPlacerCanvas.SetActive(false);
		bugPickerCanvas.SetActive(false);
		collectionCanvas.SetActive(false);
		cutsceneCanvas.SetActive(false);
		creditsCanvas.SetActive(false);
	}

	public void PickBug(string pickedBug) {
		bugPickerCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(true);
		cutsceneCanvas.SetActive(false);
		foreach (GameObject button in spawnerButtons) {
			button.SendMessage("setBaitType", pickedBug);
		}
	}

	public void placeBait() {
		bugPickerCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
		cutsceneCanvas.SetActive(false);
		playCanvas.SetActive(true);
	}

	public void displaySpiderPresent(string killedSpider, string killerSpider) {
		playCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
		bugPickerCanvas.SetActive(false);
		collectionCanvas.SetActive(false);
		cutsceneCanvas.SetActive(true);
		spiderToKill = killedSpider;
		spiderWhoKilled = killerSpider;
		spiderPresent = GameObject.Instantiate(presentPrefab);
		spiderPresent.transform.Find("DescriptionText").GetComponent<Text>().text = killerSpider + " has brought you a present!";
		spiderPresent.transform.GetComponentInChildren<Button>().onClick.AddListener(displaySpiderDeath);
		spiderPresent.transform.Find("Spider").GetComponent<Image>().sprite = spiderSprites[killerSpider];
		spiderPresent.transform.SetParent(cutsceneCanvas.transform, false);
	}
	
	public void displaySpiderDeath() {
		playCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
		bugPickerCanvas.SetActive(false);
		collectionCanvas.SetActive(false);
		cutsceneCanvas.SetActive(true);
		GameObject.Destroy(spiderPresent);
		deathAnnouncement = GameObject.Instantiate(announceDeathPrefab);
		deathAnnouncement.transform.Find("DescriptionText").GetComponent<Text>().text = spiderWhoKilled + " brought you the desiccated carcass of "+spiderToKill + "!";
		deathAnnouncement.transform.SetParent(cutsceneCanvas.transform, false);
		deathAnnouncement.transform.Find("Spider").GetComponent<Image>().sprite = spiderSprites[spiderToKill];
		deathAnnouncement.transform.GetComponentInChildren<Button>().onClick.AddListener(CloseMenu);
	}

	public void showWinner(string winnerSpider) {
		playCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
		bugPickerCanvas.SetActive(false);
		collectionCanvas.SetActive(false);
		cutsceneCanvas.SetActive(true);
		spiderWinner = GameObject.Instantiate(winnerPrefab);
		spiderWinner.transform.Find("DescriptionText").GetComponent<Text>().text = winnerSpider + " is the last living spider pal. "+winnerSpider+" is the winner! \n Would you like to start again?";
		spiderWinner.transform.Find("RestartButton").GetComponent<Button>().onClick.AddListener(resetEverything);
		spiderWinner.transform.Find("ContinueButton").GetComponent<Button>().onClick.AddListener(CloseMenu);
		spiderWinner.transform.Find("Spider").GetComponent<Image>().sprite = spiderSprites[winnerSpider];
		spiderWinner.transform.SetParent(cutsceneCanvas.transform, false);
	}

	public void resetEverything() {
		int existingAudioSetting = PlayerPrefs.GetInt("disableAudio");
		PlayerPrefs.DeleteAll();
		PlayerPrefs.SetInt("disableAudio", existingAudioSetting);
		CloseMenu();
		spiderMaster.SendMessage("resetEverything");
	}

	public void showCredits() {
		playCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
		bugPickerCanvas.SetActive(false);
		collectionCanvas.SetActive(false);
		creditsCanvas.SetActive(true);
	}

	public void changeAudio() {
		if (audioToggle.isOn) {
			audioSource.GetComponent<AudioSource>().Play();
			PlayerPrefs.SetInt("disableAudio", 0);
		} else {
			audioSource.GetComponent<AudioSource>().Stop();
			PlayerPrefs.SetInt("disableAudio", 1);
		}
	}

}
