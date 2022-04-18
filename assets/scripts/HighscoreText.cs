using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HighscoreText : MonoBehaviour {

	Text highscore;

	// Use this for initialization
	void Start () {
		highscore = GetComponent<Text> ();
		highscore.text = "Highscore: " + PlayerPrefs.GetInt ("HighScore").ToString();
	}

}
