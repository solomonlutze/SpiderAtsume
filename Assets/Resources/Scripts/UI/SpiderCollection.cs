using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SpiderCollection : MonoBehaviour {
	
	public GameObject[] collectedSpiderPrefabs;
	public GameObject[] collectionButtons;
	private GameObject[] currentSpiderCards = new GameObject[4];
	public GameObject missingSpiderPrefab;
	public GameObject spiderDescriptionPrefab;
	private int page = 1;
	public TextAsset spiderDescriptionsAsset;
	private JSONNode spiderDescriptions;

	void Start() {
		spiderDescriptions = JSON.Parse(spiderDescriptionsAsset.text);
	}

	// Use this for initialization
	void displayCollectionButtons () {
		Debug.Log ("display collection buttons...");
		for(int i = 0; i < collectionButtons.Length; i++) {
			GameObject.Destroy(currentSpiderCards[i]);
			GameObject myPrefab = missingSpiderPrefab;
			if (i + ((page-1) * collectionButtons.Length) < collectedSpiderPrefabs.Length
			    && 1 == PlayerPrefs.GetInt(collectedSpiderPrefabs[i + ((page-1) * collectionButtons.Length)].name+".hasVisited") ) {
			 	myPrefab = collectedSpiderPrefabs[i + ((page-1) * collectionButtons.Length)];
			}
			GameObject collectionButton = collectionButtons[i];
			Debug.Log ("instantiating "+myPrefab.name);
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
	}

	void openDescription(string spiderName) {
		Debug.Log ("opening description for "+spiderName);
	}
}
