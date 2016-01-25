using UnityEngine;
using System.Collections;

public class BaitGater : MonoBehaviour {
	
	public string[] baitArray = new string[6] {"Housefly","Mosquito","Moth","Butterfly","Dragonfly","Scarab"};
	
	void Awake () {
		populateAvailableBait();
	}
	
	public void populateAvailableBait() {
		int uniqueSpiderVisits = PlayerPrefs.GetInt("uniqueSpiderVisits");
		Debug.Log ("uniqueSpiderVisits"+uniqueSpiderVisits);
		for(int i = 0; i < baitArray.Length; i++) {
			Transform childButton = transform.FindChild(baitArray[i]+"NotAvailableButton");
			if (childButton != null) {
				if (uniqueSpiderVisits >= i * 4) {	
					childButton.gameObject.SetActive(false);
				} else {
					childButton.gameObject.SetActive(true);
				}
			}
		}
	}
}
