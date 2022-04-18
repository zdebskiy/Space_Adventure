using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RocketPrice : MonoBehaviour {
	
	void Start () {
		transform.GetComponent<Text> ().text = RocketManager.price.ToString();
	}

	public void ChangePrice () {
		transform.GetComponent<Text> ().text = RocketManager.price.ToString();
	}
}
