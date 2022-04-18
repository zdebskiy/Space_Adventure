using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class coutController : MonoBehaviour {

	Text cout;	
	Vector3 pos;

	// Use this for initialization
	void Start () {
		cout = GetComponent<Text> ();
		cout.text = "Width: "+Screen.width+"\nHeight: "+Screen.height;
		cout.transform.position = new Vector3(1, -1, 0);
	}
	
	// Update is called once per frame
	void Update () {		
		pos = cout.transform.position;
		cout.text = "Width: "+Screen.width+"\nHeight: "+Screen.height+"pos: "+pos.x;
	}
}
