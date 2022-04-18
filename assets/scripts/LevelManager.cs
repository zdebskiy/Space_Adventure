using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	public static LevelManager instance = null;
	public static int curLevel;
	public static int nLevels;

	class LevelObject {

		public float poolSize;
		public float shiftSpeed;
		public float spawnRate;

		public LevelObject(float nPS, float nSS, float nSR){
			poolSize = nPS;
			shiftSpeed = nSS;
			spawnRate = nSR;
		}			
	}

	public GameObject levelButtons;
	public GameObject goSelectLight;


	private string openLevels = "100000000";
	private LevelObject[] levels;

	private Button[] lvlBtns;

	//private int curLevel;


	// Use this for initialization
	void Awake () {		
		InitSingleton ();

		lvlBtns = levelButtons.GetComponentsInChildren<Button> ();

//		print ("lvl: " + curLevel);
		initLevels ();
//		print ("lvl: " + curLevel);
		initLevelsButton ();
//		print ("lvl: " + curLevel);
//		levelGenerator ();
//		print ("lvl: " + curLevel);

		rocketController.OnRocketWin += OnRocketWin;
	}

	void OnEnable (){
		initLevelsButton ();
	}

	void OnDisable(){

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


	public void LevelChanger (int lvl){
		curLevel = lvl;
		//print ("Cur lvl: " + curLevel);
		goSelectLight.transform.localPosition = lvlBtns [lvl].transform.localPosition + lvlBtns [lvl].transform.parent.localPosition;
	}

	public void NextLevel (){
		LevelChanger (curLevel + 1);
	}

	public void PreviousLevel (){
		LevelChanger (curLevel - 1);
	}

	private void initLevels (){
		string lvlFlags = "100000000";
		//string lvlFlags = "111111111";
		nLevels = lvlFlags.Length;
		if (PlayerPrefs.HasKey ("OpenLevels")) {			
		//if (false) {			
			//print ("Yes open levels");
			lvlFlags = PlayerPrefs.GetString ("OpenLevels");
			//print ("lvlFlags = " + lvlFlags);
			for (int i = 0; i < lvlFlags.Length; i++) {
				if (lvlFlags [i] == '1') {
					curLevel = i;
				}
			}
		} else {
			//print ("No open levels");
			PlayerPrefs.SetString ("OpenLevels", lvlFlags); // 0-lvl open, 1-8-lvl close;
			PlayerPrefs.Save();
			curLevel = 0;
		}
		openLevels = lvlFlags;
	}

	private void initLevelsButton (){		
		//print ("init openLevels = " + openLevels);
		for (int i = 0; i < openLevels.Length; i++) {
			//print ("-> "+i);
			if (openLevels [i] == '0') {
				lvlBtns [i].interactable = false; 
			} else {
				lvlBtns [i].interactable = true; 				
			}

			if (i == curLevel){
				lvlBtns [i].Select ();
				//print("CurLvl = "+curLevel);
				goSelectLight.transform.localPosition = lvlBtns [i].transform.localPosition + lvlBtns [i].transform.parent.localPosition;
			}
				
		}
	}

	private void levelGenerator () {
		levels = new LevelObject[openLevels.Length];
		for (int i = 0; i < openLevels.Length; i++) {
			levels [i] = new LevelObject (5 * (i + 1), 2 * (i + 1), 12 * (i + 1));
		}
	}

	private void OnRocketWin() {
		//print ("LvlMgr On Rocet Win");
		if (curLevel != nLevels - 1) {
			if (openLevels [curLevel + 1] == '0') {
				openLevels = openLevels.Remove (curLevel + 1, 1).Insert (curLevel + 1, "1");
				//print ("openLevels = " + openLevels);
				PlayerPrefs.SetString ("OpenLevels", openLevels);
				PlayerPrefs.Save ();
			}
		}
	}
}
