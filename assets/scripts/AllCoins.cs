using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllCoins : MonoBehaviour
{
    // Start is called before the first frame update
	void Start () {
		ShowAllCoins ();		
	}
		
	void OnEnable () {
		ShowAllCoins ();
	}

	public void ShowAllCoins () {
		transform.GetComponent<Text> ().text = GameManager.savedDiamond.ToString();
	}
}