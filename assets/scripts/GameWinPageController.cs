using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameWinPageController : MonoBehaviour {

	public Transform startBtn;
	public Transform previousBtn;
	public Transform nextBtn;
	public Transform scoreText;
	public GameObject planetIcons;
	public GameObject multiplyButton;
	public GameObject menuButton;

	void OnEnable () {

		GameManager.OnMultiplyScore += OnMultiplyScore;

		InitPlanetIcons ();
		InitButtons();

		ShowScore(0);

		//print (LevelManager.curLevel.ToString () + " -- " + LevelManager.nLevels.ToString ());
	}

	void ShowScore (int startValue){
		//scoreText.GetComponent<Text>().text = GameManager.score.ToString();
		if (GameManager.score > 0) {
			StartCoroutine(scoreContinue(scoreText.GetComponent<Text>(), GameManager.score, startValue));
		} else {
			scoreText.GetComponent<Text>().text = "0";
		}
	}

	IEnumerator scoreContinue(Text textArea, int value, int startValue) {
			float timeStep = 0.05f;
			if ((value * timeStep) > 0.5f) {
				timeStep = 0.5f / value;
			}
			for (int i = startValue; i <= value; i++)
			{
				textArea.text = i.ToString();
				yield return new WaitForSecondsRealtime(timeStep);
			}

		}	

	private void OnDisable() {
		GameManager.OnMultiplyScore -= OnMultiplyScore;
	}	

	private void InitButtons() {
		startBtn.GetComponent<Button> ().interactable = false;
		previousBtn.GetComponent<Button> ().interactable = false;
		nextBtn.GetComponent<Button> ().interactable = false;
		multiplyButton.GetComponent<Button> ().interactable = false;
		menuButton.GetComponent<Button> ().interactable = false;
		StartCoroutine(enableButtonsContinue());
	}

	IEnumerator enableButtonsContinue(){
			yield return new WaitForSecondsRealtime(2.5f);

			startBtn.GetComponent<Button> ().interactable = true;

			if (LevelManager.curLevel == 0) {
				//previousBtn.gameObject.SetActive(false);
				previousBtn.GetComponent<Button> ().interactable = false;
			} else {
				//previousBtn.gameObject.SetActive(true);
				previousBtn.GetComponent<Button> ().interactable = true;
			}			

			if (LevelManager.curLevel == LevelManager.nLevels-1) {
				//nextBtn.gameObject.SetActive (false);
				nextBtn.GetComponent<Button> ().interactable = false;
			} else {
				//nextBtn.gameObject.SetActive (true);
				nextBtn.GetComponent<Button> ().interactable = true;
			}

			if (GameManager.videoAdLoaded){		
				multiplyButton.GetComponent<Button> ().interactable = true;	
			} else {
				multiplyButton.GetComponent<Button> ().interactable = false;
			}

			menuButton.GetComponent<Button> ().interactable = true;

		}		

	void InitPlanetIcons() {
		for (int j = 0; j < planetIcons.transform.childCount; j++) {
			//print (planetIcons.transform.GetChild (j).name);
			if (j == LevelManager.curLevel + 1) {
				planetIcons.transform.GetChild (j).gameObject.SetActive (true);
			} else {
				planetIcons.transform.GetChild (j).gameObject.SetActive (false);
			}
		}		
	}

	void OnMultiplyScore () {
		ShowScore (GameManager.score/2);
		multiplyButton.GetComponent<Button> ().interactable = false;		

	}
	

}
