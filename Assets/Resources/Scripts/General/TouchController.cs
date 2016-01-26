using UnityEngine;
using System.Collections;

//why yes, this is copied almost verbatim from the unity script reference for GetTouch()
// http://docs.unity3d.com/ScriptReference/Input.GetTouch.html

public class TouchController : MonoBehaviour {
	void Update() {
		for (var i = 0; i < Input.touchCount; ++i) {
			if (Input.GetTouch(i).phase == TouchPhase.Began) {
				RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position), Vector2.zero);
				if (hitInfo && hitInfo.transform.gameObject.GetComponent<Spider>() != null) {
					hitInfo.transform.gameObject.SendMessage("Touch");
					break;
				}
			}
		}
		if (Input.GetMouseButtonDown (0)) { //To debug our touch logic.
			Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);
			if(hitInfo && hitInfo.transform.gameObject.GetComponent<Spider>() != null)
			{
				hitInfo.transform.gameObject.SendMessage("Touch");
			}
		}
	}


}