using UnityEngine;
using System.Collections;

public class SpiderCollection : MonoBehaviour {
	
	public GameObject[] collectedSpiderPrefabs;
	public GameObject[] collectionButtons;
	private GameObject[] currentSpiderCards = new GameObject[4];
	public GameObject missingSpiderPrefab;
	private int page = 1;

	// Use this for initialization
	void displayCollectionButtons () {
		Debug.Log ("display collection buttons...");
		for(int i = 0; i < collectionButtons.Length; i++) {
			GameObject.Destroy(currentSpiderCards[i]);
			Debug.Log ("instantiating "+collectedSpiderPrefabs[i].name);
			GameObject myPrefab = collectedSpiderPrefabs[i + ((page-1) * collectionButtons.Length)];
			GameObject collectionButton = collectionButtons[i];
			Debug.Log ("instantiating "+myPrefab.name);
			GameObject spiderCard = GameObject.Instantiate(myPrefab, collectionButton.transform.position, collectionButton.transform.rotation) as GameObject;
			spiderCard.transform.SetParent(collectionButton.transform, false);
			spiderCard.transform.position = collectionButton.transform.position;
			currentSpiderCards[i] = spiderCard;
		}
	}

	public void pageForward () {
		if (page <= 6) {
			page++;
			displayCollectionButtons();
		}
	}

	public void pageBackward () {
		if (page >= 1) {
			page--;
			displayCollectionButtons();
		}
	}

	void resetPage() {
		page = 1;
	}
}
