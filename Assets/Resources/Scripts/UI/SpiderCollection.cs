using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class SpiderCollection : MonoBehaviour {
	
	public GameObject[] collectedSpiderPrefabs;
	public GameObject[] collectionButtons;
	private GameObject[] currentSpiderCards = new GameObject[4];
	private GameObject currentSpiderDescription;
	public GameObject missingSpiderPrefab;
	public GameObject spiderDescriptionPrefab;
	public Text spidersCollectedText;
	public Text spidersDeadText;
	public Image spidersDeadIcon;
	private int page = 1;
	public TextAsset spiderDescriptionsAsset;
	private JSONNode spiderDescriptions;
	private string baseImagesPath = "Art/Character/Spider";
	private Object[] spiderSpriteList;
	private Dictionary<string, Sprite> spiderSprites = new Dictionary<string, Sprite>();

	void Start() {
		spiderSpriteList = Resources.LoadAll("Art/Character/Spider", typeof(Sprite));
		foreach (Sprite sprite in spiderSpriteList) {
			spiderSprites.Add(sprite.name, sprite);
		}
		spiderDescriptions = JSON.Parse(spiderDescriptionsAsset.text);
	}

	// Use this for initialization
	void displayCollectionButtons () {
		int uniqueVisits = PlayerPrefs.GetInt("uniqueSpiderVisits");
		int spidersKilled = PlayerPrefs.GetInt("totalSpidersKilled");
		spidersCollectedText.text = uniqueVisits+"/24";
		if (spidersKilled > 0) {
			spidersDeadText.enabled = true;
			spidersDeadText.text = spidersKilled+"/24";
			spidersDeadIcon.enabled = true;
		} else {
			spidersDeadText.enabled = false;
			spidersDeadIcon.enabled = false;
		}
		for(int i = 0; i < collectionButtons.Length; i++) {
			GameObject.Destroy(currentSpiderCards[i]);
			GameObject myPrefab = missingSpiderPrefab;
			if (i + ((page-1) * collectionButtons.Length) < collectedSpiderPrefabs.Length
			    && 1 == PlayerPrefs.GetInt(collectedSpiderPrefabs[i + ((page-1) * collectionButtons.Length)].name+".hasVisited") ) {
			 	myPrefab = collectedSpiderPrefabs[i + ((page-1) * collectionButtons.Length)];
			}
			GameObject collectionButton = collectionButtons[i];
			GameObject spiderCard = GameObject.Instantiate(myPrefab, collectionButton.transform.position, collectionButton.transform.rotation) as GameObject;
			spiderCard.transform.SetParent(collectionButton.transform, false);
			spiderCard.transform.position = collectionButton.transform.position;
			spiderCard.GetComponent<SpiderCollectionCardButton>().myCanvas = gameObject;
			spiderCard.GetComponent<SpiderCollectionCardButton>().mySpider = myPrefab.name;
			currentSpiderCards[i] = spiderCard;
		}
	}

	public void pageForward () {
		if (page == 6) {
			page = 1;
		} else {
			page++;
		}
		displayCollectionButtons();
	}

	public void pageBackward () {
		if (page == 1) {
			page = 6;
		} else {
			page--;
		}
		displayCollectionButtons();
	}

	void resetPage() {
		page = 1;
		closeDescription();
	}

	void closeDescription() {
		GameObject.Destroy (currentSpiderDescription);
		currentSpiderDescription = null;
	}

	void openDescription(string spiderName) {
		closeDescription();
		GameObject spiderDescription = GameObject.Instantiate(spiderDescriptionPrefab, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
		spiderDescription.transform.SetParent(gameObject.transform, false);
		spiderDescription.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y+50);
		int numberOfVisits = PlayerPrefs.GetInt(spiderName+".numberOfVisits");
		spiderDescription.transform.Find("VisitsText").GetComponent<Text>().text = "Visits: "+numberOfVisits;
		if (spiderName != "MissingSpider") {
			spiderDescription.transform.Find("NameText").GetComponent<Text>().text = spiderName;
			spiderDescription.transform.Find("Spider").GetComponent<Image>().sprite = spiderSprites[spiderName];
		}
		string killedBy = PlayerPrefs.GetString(spiderName+".killedBy","");
		if (killedBy == "") {
			spiderDescription.transform.Find("DeadSpider").GetComponent<Image>().enabled=false;
			spiderDescription.transform.Find("DeadText").GetComponent<Text>().enabled=false;
			spiderDescription.transform.Find("DescriptionText").GetComponent<Text>().text = spiderDescriptions[spiderName]["description"];
		} else {
			spiderDescription.transform.Find("DeadText").GetComponent<Text>().text = "Killed by "+killedBy;
			spiderDescription.transform.Find("DescriptionText").GetComponent<Text>().text = spiderDescriptions[spiderName]["dead"];
		}
		spiderDescription.transform.Find("CancelButton").GetComponent<Button>().onClick.AddListener(closeDescription);
		spiderDescription.transform.Find("BackgroundButton").GetComponent<Button>().onClick.AddListener(closeDescription);
		currentSpiderDescription = spiderDescription;
	}
}
