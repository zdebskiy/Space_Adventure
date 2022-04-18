using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondController : MonoBehaviour {

	public int scoreSpeed = 10;

	private bool isScored = false;
	private GameObject goScoreBar;

	private void Start() {
		goScoreBar = GameObject.FindWithTag("ScoreBar");		
	}

	void Update () {

		if (isScored) {
			//print ("ScoreBar pos: "+goScoreBar.transform.position);
			transform.Translate((goScoreBar.transform.position - transform.position) * scoreSpeed * Time.deltaTime);			
		}

		if (transform.position.y >= (goScoreBar.transform.position.y-0.5f)) {
			if (transform.parent.name == "bonuses"){
				Destroy(gameObject);
			} else {
				isScored = false;
				gameObject.transform.position = Vector3.one * 1000;				
			}
		}

	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			isScored = true;
			transform.position = new Vector3 (transform.position.x,transform.position.y,90);
			//gameObject.transform.position = Vector3.one * 1000;
		}
	}
			
}
