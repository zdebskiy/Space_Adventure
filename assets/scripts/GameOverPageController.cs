using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPageController : MonoBehaviour {

	public delegate void ContinueFinished();
	public static event ContinueFinished OnContinueFinished; 

	public GameObject restartButton;
	public GameObject continueButton;
	public GameObject menuButton;
	public Transform scoreText;

	//public Text countdownMenuButton;
	//public Text countdownRestartButton;

	void Start (){
		//countdownRestartButton = restartButton.GetComponentInChildren<Text> ();
		//countdownMenuButton = menuButton.GetComponentInChildren<Text> ();
	}

	void OnEnable () {

		scoreText.GetComponent<Text>().text = GameManager.score.ToString();
		InitButtons();
	}

	private void InitButtons() {
		restartButton.GetComponent<Button> ().interactable = false;
		continueButton.GetComponent<Button> ().interactable = false;
		menuButton.GetComponent<Button> ().interactable = false;

		StartCoroutine(enableButtonsContinue());
	}

	IEnumerator enableButtonsContinue(){
			yield return new WaitForSecondsRealtime(2.5f);
			restartButton.GetComponent<Button> ().interactable = true;
			if (GameManager.videoAdLoaded){		
				continueButton.GetComponent<Button> ().interactable = true;	
				//restartButton.GetComponent<Button> ().interactable = false;	
				//menuButton.GetComponent<Button> ().interactable = false;
				//StartCoroutine("Countdown");			
			} else {
				continueButton.GetComponent<Button> ().interactable = false;
				//restartButton.GetComponent<Button> ().interactable = true;	
				//menuButton.GetComponent<Button> ().interactable = true;			
				//countdownRestartButton.text="";
				//countdownMenuButton.text="";			
			}
			menuButton.GetComponent<Button> ().interactable = true;
		}			

/*
	IEnumerator Countdown(){
		int count = 3;
		for (int i = 0; i < count; i++) {
			countdownRestartButton.text=(count-i).ToString();
			countdownMenuButton.text=(count-i).ToString();
			yield return new WaitForSeconds (1);
		}
		countdownRestartButton.text="";
		countdownMenuButton.text="";
		restartButton.GetComponent<Button> ().interactable = true;	
		menuButton.GetComponent<Button> ().interactable = true;
		//OnContinueFinished ();  //Sent to GameManager
	}	
*/
}
