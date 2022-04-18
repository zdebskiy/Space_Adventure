using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInterfaceController : MonoBehaviour {

	public GameObject healthBar;
	public GameObject scoreBar;
	public GameObject diamondBar;
	public GameObject dropStar;

	private Animation animDropStar;

	private Transform stars;

	void Awake () {
		stars = healthBar.transform.GetChild(3);
	}

	// Use this for initialization
	void Start () {
		animDropStar = dropStar.GetComponent<Animation>(); 
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnEnable (){
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
		rocketController.OnRocketDamaged += OnRocketDamaged;
		rocketController.OnRocketScored += OnRocketScored;
		rocketController.OnRocketTakeHeart += OnRocketTakeHeart;
		rocketController.OnRocketTakeHealPotion += OnRocketTakeHealPotion;
		rocketController.OnRocketTakeDiamond += OnRocketTakeDiamond;

		Configure ();
		scoreRender ();
		DiamondRender ();
		healthRender ();
	}

	void OnDisable (){
		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
		rocketController.OnRocketDamaged -= OnRocketDamaged;
		rocketController.OnRocketScored -= OnRocketScored;
		rocketController.OnRocketTakeHeart -= OnRocketTakeHeart;
		rocketController.OnRocketTakeHealPotion -= OnRocketTakeHealPotion;
		rocketController.OnRocketTakeDiamond -= OnRocketTakeDiamond;
	}

	void OnRocketDamaged (){
		healthRender ();
		animDropStar.Play("star_drop");
	}

	void OnRocketScored (){
		scoreRender ();
	}

	void OnRocketTakeDiamond (){
		DiamondRender();
	}

	void OnRocketTakeHeart(){
		healthRender ();
	}

	void OnRocketTakeHealPotion(){
		healthRender ();
	}

	void OnGameOverConfirmed(){
		//SetAllHeart ();
	}
		
	void Configure() {
		for (int i = 0; i < stars.transform.childCount; i++) {
			stars.transform.GetChild(i).GetComponent<Image>().color = new Color (1f, 1f, 1f, 1f);
			//print ("fullHealth: " + GameManager.fullHealth);
			if (i < GameManager.fullHealth/2) {
				stars.transform.GetChild (i).gameObject.SetActive (true);
			} else {
				stars.transform.GetChild (i).gameObject.SetActive (false);
			}
		}

//		scoreBar.gameObject.GetComponent<Text> ().text = GameManager.score;
	}

	void healthRender (){
		//print (GameManager.curHealth);
		//print("Stars: "+stars.transform.childCount);
		for (int i = 0; i < stars.transform.childCount; i++) {
			if (i*2 >= GameManager.curHealth) {
				DisposeHeart (i, 0.0f);
			} else {
				if ((i*2+1) == GameManager.curHealth) {
					DisposeHeart (i, 0.5f);
				} else {
					ActivateHeart (i);
				}
			}
		}
	}

	void DisposeHeart(int i, float fillAmount){
		//print(stars.transform.GetChild(i).transform.name);
		Image sr = stars.transform.GetChild(i).GetComponent<Image>();
		sr.fillAmount = fillAmount;
	}

	void ActivateHeart(int i){
		Image sr = stars.transform.GetChild(i).GetComponent<Image>();
		sr.fillAmount = 1.0f;
	}


	void scoreRender () {
		scoreBar.GetComponent<Text>().text = GameManager.score.ToString();
	}

	void DiamondRender () {
		diamondBar.GetComponent<Text>().text = GameManager.diamond.ToString();
	}

/*
	void SetAllHeart(){
		foreach (Transform child in stars) {
			SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
			sr.color = new Color (1f, 1f, 1f, 1f);			
		}
	}		
*/

}
