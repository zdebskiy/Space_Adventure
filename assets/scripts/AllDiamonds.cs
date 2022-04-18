using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllDiamonds : MonoBehaviour {

	void Start () {
		ShowAllDiamonds ();		
	}
		
	void OnEnable () {
		ShowAllDiamonds ();
	}

	public void ShowAllDiamonds () {
		transform.GetComponent<Text> ().text = GameManager.savedScore.ToString();
	}
}
