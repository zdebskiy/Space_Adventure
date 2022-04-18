using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RocketManager : MonoBehaviour {

	public delegate void RocketManagerDelegate();
	public static event RocketManagerDelegate OnCurRocketChange;
	public static event RocketManagerDelegate OnCurRocketsPageChange;

	public static RocketManager instance = null;
	public static int curRocket;
	public static int speed;
	public static int health;
	public static int price;	
	public static bool isOpen;
	public static RocketObject[] rockets;	
	public static int curRocketsPage;

	public class RocketObject {

		public int health;
		public int speed;
		public int price;
		public bool isOpen;


		public RocketObject(int nh, int ns, int np, bool nOpen){
			health = nh;
			speed = ns;
			price = np;
			isOpen = nOpen;
		}			
	}

	public GameObject rocketsObj;
	//public GameObject rocketImage;
	//public GameObject rocketHealth;
	//public GameObject rocketSpeed;
	//public GameObject buyBar;


	private string openRockets;
	private int nRockets;
	//private int curRocket;

	private const int N_CARDS_ON_CANVAS = 6;

	private void Awake() {
		InitSingleton ();				
		InitRockets ();		
	}

	// Use this for initialization
	void Start () {		
		curRocket = 0;	
		curRocketsPage = 0;		
		SetCurRocket(curRocket);
	}

	void OnEnable (){
		//ShowRocket (curRocket);
	}

	// Update is called once per frame
	void Update () {
		
	}	

	private void InitSingleton(){
		if (instance == null) {
			instance = this;
		} else if (instance == this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	public void NextRocketsPage(){
		curRocketsPage++;
		// следующая ракета
		//curRocket = CircleChange (curRocket, '+');
		//ShowRocket (curRocket);
		//print (curRocket);
		OnCurRocketsPageChange(); //	Send to MenuRocketComtroller;
	}

	public void PreviousRocketsPage(){
		curRocketsPage--;
		// предидущая ракета
		//curRocket = CircleChange (curRocket, '-');
		//ShowRocket (curRocket);
		//print (curRocket);
		OnCurRocketsPageChange(); //	Send to MenuRocketComtroller;
	}

	public void SetCurRocket(int iOnCanvas){
		//print ("RocketManager = curRocket("+iOnCanvas+")");
		setCurRocket (curRocketsPage * N_CARDS_ON_CANVAS + iOnCanvas);
		//print("SetCurRocket = "+curRocket);
		OnCurRocketChange();
	}

	public void InitRockets (){
		// инициализация свойств ракет
		openRockets = "110000";
		if (PlayerPrefs.HasKey ("OpenRockets")) {			
		//if (false) {
			//print ("Yes OpenRockets");
			openRockets = PlayerPrefs.GetString ("OpenRockets");
			//print ("OpenRockets = " + openRockets);
		} else {
			//print ("No OpenRockets");
			PlayerPrefs.SetString ("OpenRockets", openRockets); // 0-lvl open, 1-8-lvl close;
			PlayerPrefs.Save();
		}

		nRockets = rocketsObj.transform.childCount;
		rocketController[] rCRokets = rocketsObj.GetComponentsInChildren<rocketController> (true);
		rockets = new RocketObject[nRockets];
		bool flagOpen=false;
		for (int i = 0; i < rockets.Length; i++) {
			if (openRockets[i] == '1') {flagOpen=true;} else {flagOpen=false;};
			rockets [i] = new RocketObject (rCRokets[i].fullHealth, rCRokets[i].speed, rCRokets[i].price, flagOpen);
			//print ("Rockets[ "+i+" ]= "+openRockets[i]);
		}
		//OnCurRocketChange(rockets, 0); //	Send to MenuRocketComtroller;
	}

	public void BuyRocket (int iOnCanvas) {
		int iRocket = curRocketsPage * N_CARDS_ON_CANVAS + iOnCanvas;
		//print ("Before: " + openRockets);
		openRockets = openRockets.Remove (iRocket, 1).Insert (iRocket, "1");
		//print ("After: " + openRockets);
		PlayerPrefs.SetString ("OpenRockets", openRockets);
		PlayerPrefs.Save();
		rockets[iRocket].isOpen = true;
		setCurRocket (iRocket);
		//print ("BuyRocket = "+curRocket);
		OnCurRocketsPageChange();
	}

	private void setCurRocket (int i) {
		curRocket = i;
		speed = rockets [i].speed;
		health = rockets [i].health;
		price = rockets [i].price;
		isOpen = rockets [i].isOpen;
	}

/*
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
*/

/*
	private void ShowRocketImage (GameObject go, int nR){

		if (openRockets[nR] == '1') {openRocket = true;} else {openRocket = false;}

		for (int i = 0; i < go.transform.childCount; i++) {
			
			GameObject rocket = go.transform.GetChild (i).transform.GetChild (0).gameObject;
			GameObject rocketDis = go.transform.GetChild (i).transform.GetChild (1).gameObject;
			GameObject rocketTitle = go.transform.GetChild (i).transform.GetChild (2).gameObject;

			if (i == nR) {
				rocketTitle.SetActive(true);
				if (openRocket) {
					rocket.SetActive(true);
					rocketDis.SetActive(false);					
					buyBar.SetActive(false);
				} else {
					rocket.SetActive(false);
					rocketDis.SetActive (true);
					if (GameManager.savedScore >= price) {
						// buy button
						buyBar.transform.GetChild (1).transform.GetChild (0).gameObject.SetActive(true);
						buyBar.transform.GetChild (1).transform.GetChild (1).gameObject.SetActive(false);
					} else {
						// shop button
						buyBar.transform.GetChild (1).transform.GetChild (0).gameObject.SetActive(false);
						buyBar.transform.GetChild (1).transform.GetChild (1).gameObject.SetActive(true);
					}
					buyBar.SetActive(true);

				}
			} else {
				rocket.SetActive(false);
				rocketDis.SetActive(false);
				rocketTitle.SetActive(false);
			}
		}

		SpriteRenderer[] sr = go.GetComponentsInChildren<SpriteRenderer>(true);
		//print (sr.Length);
		for (int i=0; i<sr.Length; i++){			
			if (i == nR) {
				sr [i].gameObject.SetActive (true);
			} else {
				sr [i].gameObject.SetActive (false);
			}
		}
	
	}
/*
	private int CircleChange(int cur, char operation) {
		switch (operation) {
		case '+':
			if (cur == nRockets - 1) {
				cur = 0;
			} else {
				cur++;
			}
			break;
		case '-':
			if (cur == 0) {
				cur = nRockets - 1;
			} else {
				cur--;
			}
			break;
		default:
			break;			
		}
		return cur;
	}
*/
}
