using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof (Text))]
public class CountdovnText : MonoBehaviour {

	public delegate void CountdownFinished();
	public static event CountdownFinished OnCountdownFinished; 
	public AudioSource startAudio;
	public AudioClip [] startRocketClips;
	public AudioClip [] startUFOClips;

	Text countdown;

	void OnEnable () {
		countdown = GetComponent<Text> ();		
		if (RocketManager.curRocket % 2 == 0){
			// Rocket start clip
			startAudio.clip = startRocketClips[UnityEngine.Random.Range(0, startRocketClips.Length)];
		} else {
			// UFO start clip
			startAudio.clip = startUFOClips[UnityEngine.Random.Range(0, startUFOClips.Length)];
		}
		countdown.text = "3";				
		startAudio.Play();
		StartCoroutine("Countdown");
	}

	IEnumerator Countdown(){
		int count = 3;
		for (int i = 0; i < count; i++) {										
			countdown.text = (count - i).ToString ();
			yield return new WaitForSeconds (1);
		}
		OnCountdownFinished ();  //Sent to GameManager
	}
}

