using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLaunchAnimation : MonoBehaviour {

	public float taskScale;

	private Rigidbody2D rb;


/*
	void OnEnable {
		
	}
*/	

	// Use this for initialization
	void Start () {
		rb = GetComponent <Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (GameManager.gamePause) {
			return;
		}

		if (transform.localScale.x < taskScale) {			
			transform.localScale += new Vector3 (0.2f * Time.deltaTime, 0.2f * Time.deltaTime, 0.2f * Time.deltaTime);
		}
	}
}
