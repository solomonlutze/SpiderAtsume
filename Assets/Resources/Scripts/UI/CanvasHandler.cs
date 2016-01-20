using UnityEngine;
using System.Collections;

public class CanvasHandler : MonoBehaviour {
	public GameObject bugPickerCanvas;
	public GameObject bugPlacerCanvas;
	public GameObject playCanvas;
	public GameObject[] spawnerButtons = null;

	// Use this for initialization
	void Start () {
		bugPickerCanvas.SetActive(false);
		bugPlacerCanvas.SetActive(false);
	}

	public void OpenMenu () {
		playCanvas.SetActive(false);
		bugPickerCanvas.SetActive(true);
	}

	public void CloseMenu () {
		playCanvas.SetActive(true);
		bugPlacerCanvas.SetActive(false);
		bugPickerCanvas.SetActive(false);
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

	// Update is called once per frame
	void Update () {
	
	}
}
