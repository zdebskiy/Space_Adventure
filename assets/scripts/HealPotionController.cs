using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPotionController : MonoBehaviour
{
   public int scoreSpeed = 5;  

	private bool isScored = false;

	private GameObject goScoreBar;

	private void Start() {
		goScoreBar = GameObject.FindWithTag("HealthBar");		
	}

	void Update () {
		//transform.RotateAround(transform.position, axis, 10*Time.deltaTime);

		if (isScored) {
			//print ("ScoreBar pos: "+goScoreBar.transform.position);
			transform.Translate((goScoreBar.transform.position - transform.position) * scoreSpeed * Time.deltaTime);			
            if (transform.position.y >= (goScoreBar.transform.position.y-0.5f)) {			
                isScored = false;
                Destroy(gameObject);
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
