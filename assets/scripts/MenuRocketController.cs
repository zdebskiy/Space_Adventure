using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuRocketController : MonoBehaviour
{    
	public int iOnCanvas; // 'i' position Rocket Card on canvas

	public GameObject rocketImage;
	public GameObject rocketHealth;
	public GameObject rocketSpeed;
	public GameObject buyBar;
	public GameObject bgShadow;
	public GameObject btnSelect;


	private int health;
	private int speed;
	private int price;
	private bool isOpen;
	private bool isSelected;	

	private const int N_CARDS_ON_CANVAS =6;

	private void Awake() {
		
	}
	// Use this for initialization
	void Start () {		
		ShowRocketCard(RocketManager.curRocketsPage*N_CARDS_ON_CANVAS+iOnCanvas);
		ShowRocketCardSelector(RocketManager.curRocketsPage*N_CARDS_ON_CANVAS+iOnCanvas);
	}

	void OnEnable (){
		RocketManager.OnCurRocketChange += OnCurRocketChange;
		RocketManager.OnCurRocketsPageChange += OnCurRocketsPageChange;		

		ShowRocketCard(RocketManager.curRocketsPage*N_CARDS_ON_CANVAS+iOnCanvas);
		ShowRocketCardSelector(RocketManager.curRocketsPage*N_CARDS_ON_CANVAS+iOnCanvas);
	}

	private void OnDisable() {
		RocketManager.OnCurRocketChange -= OnCurRocketChange;
		RocketManager.OnCurRocketsPageChange -= OnCurRocketsPageChange;				
	}


	private void OnCurRocketChange(){
		ShowRocketCardSelector(RocketManager.curRocketsPage*N_CARDS_ON_CANVAS+iOnCanvas);		
	}

	private void OnCurRocketsPageChange(){
		ShowRocketCard(RocketManager.curRocketsPage*N_CARDS_ON_CANVAS+iOnCanvas);
		ShowRocketCardSelector(RocketManager.curRocketsPage*N_CARDS_ON_CANVAS+iOnCanvas);		
	}	

	private void ShowRocketCard (int iShowRocket){
		health = RocketManager.rockets[iShowRocket].health;
		speed = RocketManager.rockets[iShowRocket].speed;
		price =	RocketManager.rockets[iShowRocket].price;
		isOpen = RocketManager.rockets[iShowRocket].isOpen;		
		ShowRocketImage (rocketImage, iShowRocket);
		ShowStars (rocketHealth, RocketManager.rockets[iShowRocket].health/2);
		ShowStars (rocketSpeed, RocketManager.rockets[iShowRocket].speed);
	}

	private void ShowRocketImage (GameObject go, int nR){

		for (int i = 0; i < go.transform.childCount; i++) {
			
			GameObject rocket = go.transform.GetChild (i).transform.GetChild (0).gameObject;
			GameObject rocketDis = go.transform.GetChild (i).transform.GetChild (1).gameObject;
			GameObject rocketTitle = go.transform.GetChild (i).transform.GetChild (2).gameObject;

			if (i == nR) {
				rocketTitle.SetActive(true);
				if (isOpen) {
					rocket.SetActive(true);
					rocketDis.SetActive(false);
					buyBar.SetActive(false);
					btnSelect.GetComponent<Button> ().interactable = true;
				} else {
					rocket.SetActive(false);
					rocketDis.SetActive (true);
					//print("Score: "+GameManager.savedScore+"; Price: "+price);
					if (GameManager.savedScore >= price) {
						// buy button
						buyBar.transform.GetChild (1).transform.GetChild (0).gameObject.SetActive(true);
						buyBar.transform.GetChild (1).transform.GetChild (1).gameObject.SetActive(false);
					} else {
						// shop button
						buyBar.transform.GetChild (1).transform.GetChild (0).gameObject.SetActive(false);
						buyBar.transform.GetChild (1).transform.GetChild (1).gameObject.SetActive(true);
					}
					buyBar.transform.GetChild (2).transform.GetComponent<Text>().text=price.ToString();
					buyBar.SetActive(true);
					btnSelect.GetComponent<Button> ().interactable = false;
				}
			} else {
				rocket.SetActive(false);
				rocketDis.SetActive(false);
				rocketTitle.SetActive(false);
			}
		}
	}		

	private void ShowRocketCardSelector (int iShowRocket){
		if (isOpen){			
			if (RocketManager.curRocket == iShowRocket){
				isSelected=true;
				bgShadow.transform.GetComponent<SpriteRenderer>().color = new Color (0f, 1f, 0.06f, 0.8f);
			}else{
				isSelected=false;
				bgShadow.transform.GetComponent<SpriteRenderer>().color = new Color (0.4f, 0.4f, 0.4f, 0.8f);
			}		
			bgShadow.SetActive(true);			
		}else{
			bgShadow.SetActive(false);
		}
	}

	private void ShowStars(GameObject go, int val){
		SpriteRenderer[] sr = go.GetComponentsInChildren<SpriteRenderer>();
		for (int i=0; i<sr.Length; i++){
			if (i < val) {
				sr [i].color = new Color (1f, 1f, 1f, 1f);
			} else {
				sr [i].color = new Color (1f, 1f, 1f, 0.3f);
			}
				
		}
	}
}
