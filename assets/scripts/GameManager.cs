using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;


public class GameManager : MonoBehaviour {
	public delegate void GameDelegate();
	public static event GameDelegate OnGameStarted; 
	public static event GameDelegate OnGameOverConfirmed;
	public static event GameDelegate OnRocketDie;
	public static event GameDelegate OnWtchedRewardedVideo;
	public static event GameDelegate OnMultiplyScore;

	public static GameManager instance = null;

	public GameObject liveBackground;
	public GameObject blackPageDown;
	public GameObject blackPageUp;
	public GameObject rocketMenu;
	public GameObject levelMenu;
	public GameObject gamePage;	
	public GameObject gamePageJoystik;	
	public GameObject gameEnvironment;
	public GameObject gameOverPage;
	public GameObject gameWinPage;
	public GameObject gamePausePage;
	public GameObject countdownPage;
	public GameObject gameEndPage;
	public GameObject shopPage;

	public GameObject bonusLevel;
	public GameObject bonusBox;

	public GameObject[] helpHands;
	private HelpHands helpHandsName;

	enum PageState {
		None,
		Game,
		GameOver,
		Countdown,
		RocketMenu,
		LevelMenu,
		Win,
		Pause,
		End,
		Shop,
		AdMob,
		BonusLevel
	}

	enum HelpHands {
		BuyRocket0,
		BuyRocket1,
		BuyRocket2,
		BuyRocket3,
		BuyRocket4,
		BuyRocket5,
		StartGame,
		Shop
	}

	//Rocket
	public static int fullHealth;
	public static int curHealth;
	public static int curGun;

	// NumSession
	public static int savedNumSession = 1;

	//Score
	public static int score = 0;
	public static int savedScore;

	//Diamond
	public static int diamond = 0;
	public static int savedDiamond;	
	private int tryCounter = 0;

	// Bonuses variable
	public bool firstLevelInSession = true;
	public int winCounter = 0;
	public int failureCounter = 0;
	public bool isBonusLevel = false;
	public bool isBonusBox = false;
	
	// Audio Sourse
	public AudioSource soundGameOver;
	public AudioClip [] soundRocketGameOverClips;
	public AudioClip [] soundUFOGameOverClips;

	public AudioSource soundWin;
	public AudioClip [] soundRocketWinClips;
	public AudioClip [] soundUFOWinClips;

	public AudioSource soundBonusBox;
	public AudioClip [] soundBonusBoxWindowClips;

	public AudioSource soundBonusLevel;
	public AudioClip [] soundBonusLevelWindowClips;

	//Game State
	public static bool gameOver = true;
	public static bool gamePause = false;
	public static bool gameNew = false;
	public static bool gameMenu = true;

	//AdMob
    private InterstitialAd interstitialAd;
	public static bool interstitialAdLoaded = false;

	//private RewardBasedVideoAd videoAd; // old
    private RewardedAd videoAd;  // new
	public static bool videoAdLoaded = false;

    private string appID;
    private string interstitialAdID;
    private string videoAdID;
    int rewardID;

	string nameCall; // name button who call Ad Video;
	bool adWindowStatus = false;
	int iClosed = 0;

	// Firebase Google Analitics
	DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
	protected bool firebaseInitialized = false;

/*
	public int GetScore(){
		return score;
	}
*/	

	void Awake(){

		InitSingleton ();

		if (PlayerPrefs.HasKey ("SavedNumSession")) {
		//if (false){
			//print ("Has NumSession");			
			savedNumSession = PlayerPrefs.GetInt ("SavedNumSession");
			savedNumSession++;
			PlayerPrefs.SetInt ("SavedNumSession", savedNumSession);
			PlayerPrefs.Save();			
		} else {
			//print ("Dont has NumSession");
			//savedNumSession = 1;
			PlayerPrefs.SetInt ("SavedNumSession", savedNumSession);
			PlayerPrefs.Save();
		}

		if (PlayerPrefs.HasKey ("SaveScore")) {
		//if (false){
			//print ("Has SaveScore");
			savedScore = PlayerPrefs.GetInt ("SaveScore");
		} else {
			//print ("Dont has SaveScore");
			//savedScore = 1000;
			PlayerPrefs.SetInt ("SaveScore", savedScore);
			PlayerPrefs.Save();
		}

		if (PlayerPrefs.HasKey ("SaveDiamond")) {
		//if (false){
			//print ("Has SaveDiamond");
			savedDiamond = PlayerPrefs.GetInt ("SaveDiamond");
		} else {
			//print ("Dont has SaveDiamond");
			//savedDiamond = 10;
			PlayerPrefs.SetInt ("SaveDiamond", savedDiamond);
			PlayerPrefs.Save();
		}
	}

	void Start() {

		AdMobInit(false);
		AnaliticsInit();		

		VideoAdInitAndRequest();
		InterstitialAdInitAndRequest();

		//scoreGameOverText = gameOverPage.transform.Find ("ScoreText").GetComponent<Text> ();
		
		//print (savedScore.ToString ());

		winCounter = 0;
		failureCounter = 0;

		if (savedNumSession == 1) {
			helpHandsName = HelpHands.StartGame;
			helpHands[(int)helpHandsName].SetActive (true);
		}

	}

	private void Update() {
		if (adWindowStatus) {

		}
	}

	void OnEnable () {
		CountdovnText.OnCountdownFinished += OnCountdownFinished; 
		rocketController.OnRocketDamaged += OnRocketDamaged;
		rocketController.OnRocketScored += OnRocketScored;
		rocketController.OnRocketFinish += OnRocketFinish;
		rocketController.OnRocketWin += OnRocketWin;
		rocketController.OnRocketTakeHeart += OnRocketTakeHeart;
		rocketController.OnRocketTakeHealPotion += OnRocketTakeHealPotion;
		rocketController.OnRocketTakeGun += OnRocketTakeGun;
		rocketController.OnRocketTakeDiamond += OnRocketTakeDiamond;
		GameOverPageController.OnContinueFinished += OnContinueFinished;
	}

	void OnDisable (){
		CountdovnText.OnCountdownFinished -= OnCountdownFinished; 
		rocketController.OnRocketDamaged -= OnRocketDamaged;
		rocketController.OnRocketScored -= OnRocketScored;
		rocketController.OnRocketFinish -= OnRocketFinish;
		rocketController.OnRocketWin -= OnRocketWin;
		rocketController.OnRocketTakeHeart -= OnRocketTakeHeart;
		rocketController.OnRocketTakeHealPotion -= OnRocketTakeHealPotion;
		rocketController.OnRocketTakeGun -= OnRocketTakeGun;
		rocketController.OnRocketTakeDiamond -= OnRocketTakeDiamond;
		GameOverPageController.OnContinueFinished -= OnContinueFinished;
	}

	private void InitSingleton(){
		if (instance == null) {
			instance = this;
		} else if (instance == this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	void OnContinueFinished() {		
		gameOver = true;
		gamePause = false;
		if (score > savedScore) {
			savedScore = score;
			PlayerPrefs.SetInt ("HighScore", score);
			PlayerPrefs.Save();
		}
	}

	void OnCountdownFinished(){		
		SetPageState (PageState.Game);
		gamePageJoystik.SetActive(true);		
		OnGameStarted (); //Send to rocketController and to Parallaxer
/*
 * 		if (gamePause) {
			score = 0;
		}
*/		
		gamePause = false;
		gameNew = false;
	}

	void OnRocketDamaged (){
		//scoreGameOverText.text = "Score: " + score.ToString();
		curHealth--;
		//print ("OnRocketDie: " + score.ToString() + " > " + savedScore.ToString ());
		//Debug.Log("--->>> savedNumSession = "+savedNumSession+" | firstLevelInSession = "+firstLevelInSession);
		if ((curHealth == 0) && (savedNumSession != 1) && (!firstLevelInSession)) {
			gamePageJoystik.SetActive(false);			
			FGA_LevelFailure(RocketManager.curRocket, LevelManager.curLevel, tryCounter);
			gamePause = true;
			OnRocketDie(); // 
			SetPageState (PageState.GameOver);			
			failureCounter++;
			BonusBoxGenerator();
		}
	}

	void OnRocketScored (){
		score++;
		//scoreText.text = score.ToString (); 
	}


	void OnRocketFinish (){		
		blackPageUp.SetActive (true);
		gamePageJoystik.SetActive(false);		
	}

	void OnRocketWin () {		
		if (!isBonusLevel) {
			winCounter++;
		}
		if (firstLevelInSession){
			firstLevelInSession = false;
		}
		//print ("OnRocketWin");			
		if (LevelManager.curLevel == LevelManager.nLevels - 1) {
			SetPageState (PageState.End);
			FGA_GameEnd(RocketManager.curRocket, LevelManager.curLevel, tryCounter, score);
		} else {
			if (isBonusLevel){
				FGA_BonusLevelEnd(RocketManager.curRocket, LevelManager.curLevel, score);
			} else {
				FGA_LevelEnd(RocketManager.curRocket, LevelManager.curLevel, tryCounter, score);
			}			
			SetPageState (PageState.Win);
			BonusGenerator();
		}
		savedScore += score;
		PlayerPrefs.SetInt ("SaveScore", savedScore);
		PlayerPrefs.Save();

		savedDiamond += diamond;
		PlayerPrefs.SetInt ("SaveDiamond", savedDiamond);
		PlayerPrefs.Save();

		gameOver = true;
		//ShowInterstitialAd();		
	}

	public void GoToMultiplyScore (){
		print("MultiplyScore : "+savedScore);
		FGA_EventSelectPromotion("WinPage");
		//SetPageState (PageState.None);
		ShowVideoAd("GameWin");
	}

	public void MultiplyScore (){
		savedScore += score;
		PlayerPrefs.SetInt ("SaveScore", savedScore);
		PlayerPrefs.Save();
		score*=2;
		OnMultiplyScore (); // Send to GameWinPageController
		SetPageState (PageState.Win);		
	}


	void OnRocketTakeHeart(){
		if (curHealth < fullHealth){
			curHealth++;	
		}
	}

	void OnRocketTakeHealPotion() {
		curHealth = fullHealth;		
	}

	void OnRocketTakeDiamond() {
		diamond++;		
	}

	void OnRocketTakeGun(string numberGun) {
		switch (numberGun)
		{
			case "0":
				//print("Take Gun - 0");
				curGun=0;
				break;
			case "1":
				//print("Take Gun - 1");
				curGun=1;
				break;
			case "2":
				//print("Take Gun - 2");
				curGun=2;
				break;								
			case "3":
				//print("Take Gun - 3");
				curGun=3;
				break;				
			default:
				curGun=0;			
				break;
		}
	}

	public void RateApp (){
		Application.OpenURL("market://details?id=" + Application.identifier);
	}

	public void InformApp (){
		Application.OpenURL("https://docs.google.com/document/d/1DzJgq4dvLGkuBZY_qZAu_UR_zUXKtMdTQofonq3Dv1I/edit?usp=sharing");
	}

	void SetPageState (PageState state){
		switch (state) {
		case PageState.None:
			gameMenu = false;

			liveBackground.SetActive (false);

			blackPageDown.SetActive (false);
			blackPageUp.SetActive (false);

			rocketMenu.SetActive (false);
			levelMenu.SetActive (false);

			gamePage.SetActive (false);
			gameEnvironment.SetActive (false);

			gameWinPage.SetActive (false);

			gameOverPage.SetActive (false);

			gamePausePage.SetActive (false);

			countdownPage.SetActive (false);

			gameEndPage.SetActive (false);

			shopPage.SetActive (false);

			bonusLevel.SetActive (false);
			break;

		case PageState.RocketMenu:

			FGA_SetCurrentScreen("RocketMenuPage");

			gameMenu = true;

			liveBackground.SetActive (true);

			blackPageDown.SetActive (false);
			blackPageUp.SetActive (false);

			rocketMenu.SetActive (true);
			levelMenu.SetActive (false);

			gamePage.SetActive (false);
			gameEnvironment.SetActive (false);

			gameWinPage.SetActive (false);

			gameOverPage.SetActive (false);

			gamePausePage.SetActive (false);

			countdownPage.SetActive (false);

			gameEndPage.SetActive (false);

			shopPage.SetActive (false);

			bonusLevel.SetActive (false);

			DisableHands();
			break;

		case PageState.LevelMenu:

			FGA_SetCurrentScreen("LevelMenuPage");

			gameMenu = true;

			liveBackground.SetActive (true);

			blackPageDown.SetActive (false);
			blackPageUp.SetActive (false);

			rocketMenu.SetActive (false);
			levelMenu.SetActive (true);

			gamePage.SetActive (false);
			gameEnvironment.SetActive (false);

			gameWinPage.SetActive (false);

			gameOverPage.SetActive (false);

			gamePausePage.SetActive (false);

			countdownPage.SetActive (false);

			gameEndPage.SetActive (false);

			shopPage.SetActive (false);

			bonusLevel.SetActive (false);
			break;			

		case PageState.Game:

			FGA_SetCurrentScreen("GamePage");

			gameMenu = false;

			liveBackground.SetActive (false);

			blackPageDown.SetActive (false);
			blackPageUp.SetActive (false);

			rocketMenu.SetActive (false);
			levelMenu.SetActive (false);

			gamePage.SetActive (true);
			gameEnvironment.SetActive (true);

			gameWinPage.SetActive (false);

			gameOverPage.SetActive (false);

			gamePausePage.SetActive (false);

			countdownPage.SetActive (false);

			gameEndPage.SetActive (false);

			shopPage.SetActive (false);

			bonusLevel.SetActive (false);
			break;

		case PageState.GameOver:

			FGA_SetCurrentScreen("GameOverPage");
			gameMenu = false;

			liveBackground.SetActive (false);

			blackPageDown.SetActive (false);
			blackPageUp.SetActive (false);			

			rocketMenu.SetActive (false);
			levelMenu.SetActive (false);

			gamePage.SetActive (false);
			gameEnvironment.SetActive (true);

			gameWinPage.SetActive (false);

			gameOverPage.SetActive (true);

			countdownPage.SetActive (false);

			gameEndPage.SetActive (false);

			shopPage.SetActive (false);

			bonusLevel.SetActive (false);

			soundGameOver.Play();
			break;

		case PageState.Countdown:
			gameMenu = false;

			liveBackground.SetActive (false);

			blackPageDown.SetActive (true);
			blackPageUp.SetActive (false);

			rocketMenu.SetActive (false);
			levelMenu.SetActive (false);

			gamePage.SetActive (true);
			gameEnvironment.SetActive (true);

			gameWinPage.SetActive (false);

			gameOverPage.SetActive (false);

			gamePausePage.SetActive (false);

			countdownPage.SetActive (true);

			gameEndPage.SetActive (false);

			shopPage.SetActive (false);

			bonusLevel.SetActive (false);

			break;

		case PageState.Win:
			FGA_SetCurrentScreen("GameWinPage");
			gameMenu = false;

			liveBackground.SetActive (true);

			blackPageDown.SetActive (true);
			blackPageUp.SetActive (false);

			rocketMenu.SetActive (false);
			levelMenu.SetActive (false);

			gamePage.SetActive (false);
			gameEnvironment.SetActive (false);

			gameWinPage.SetActive (true);

			gameOverPage.SetActive (false);

			gamePausePage.SetActive (false);

			countdownPage.SetActive (false);

			gameEndPage.SetActive (false);

			shopPage.SetActive (false);

			bonusLevel.SetActive (false);

			soundWin.Play();
			break;

		case PageState.Pause:
			FGA_SetCurrentScreen("GamePausePage");
			gameMenu = false;
			
			liveBackground.SetActive (false);

			blackPageDown.SetActive (false);
			blackPageUp.SetActive (false);			

			rocketMenu.SetActive (false);
			levelMenu.SetActive (false);

			gamePage.SetActive (false);
			gameEnvironment.SetActive (true);

			gameWinPage.SetActive (false);

			gameOverPage.SetActive (false);

			gamePausePage.SetActive (true);

			countdownPage.SetActive (false);

			gameEndPage.SetActive (false);

			shopPage.SetActive (false);

			bonusLevel.SetActive (false);

			break;

		case PageState.Shop:
			FGA_SetCurrentScreen("ShopPage");
			gameMenu = false;

			liveBackground.SetActive (true);

			blackPageDown.SetActive (true);
			blackPageUp.SetActive (false);

			rocketMenu.SetActive (false);
			levelMenu.SetActive (false);

			gamePage.SetActive (false);
			gameEnvironment.SetActive (false);

			gameWinPage.SetActive (false);

			gameOverPage.SetActive (false);

			gamePausePage.SetActive (false);

			countdownPage.SetActive (false);

			gameEndPage.SetActive (false);

			shopPage.SetActive (true);

			bonusLevel.SetActive (false);

			break;		
			
		case PageState.End:
			FGA_SetCurrentScreen("GameEndPage");
			gameMenu = false;

			liveBackground.SetActive (true);

			blackPageDown.SetActive (true);
			blackPageUp.SetActive (false);

			rocketMenu.SetActive (false);
			levelMenu.SetActive (false);

			gamePage.SetActive (false);
			gameEnvironment.SetActive (false);

			gameWinPage.SetActive (false);

			gameOverPage.SetActive (false);

			gamePausePage.SetActive (false);

			countdownPage.SetActive (false);

			gameEndPage.SetActive (true);

			shopPage.SetActive (false);

			bonusLevel.SetActive (false);

			soundWin.Play();			

			break;

		case PageState.BonusLevel:

			FGA_SetCurrentScreen("GameBonusLevelPage");
			gameMenu = false;

			liveBackground.SetActive (true);

			blackPageDown.SetActive (true);
			blackPageUp.SetActive (false);

			rocketMenu.SetActive (false);
			levelMenu.SetActive (false);

			gamePage.SetActive (false);
			gameEnvironment.SetActive (false);

			gameWinPage.SetActive (false);

			gameOverPage.SetActive (false);

			gamePausePage.SetActive (false);

			countdownPage.SetActive (false);

			gameEndPage.SetActive (false);

			shopPage.SetActive (false);

			bonusLevel.SetActive (true);

			break;

		}
	}
/*
	public void ConfirmGameOver(){
		// активируется при нажатии кнопки REPLAY
		OnGameOverConfirmed(); //Send to rocketController
		//scoreText.text = "0";
		SetPageState(PageState.Game);
	}
*/	

	public void StartGame(){		
		if (isBonusLevel){
			FGA_BonusLevelStart(RocketManager.curRocket, LevelManager.curLevel);
		} else {
			FGA_LevelStart(RocketManager.curRocket, LevelManager.curLevel);
		}
				
		gameNew = true;
		gamePause = false;
		InitGame();
		SetPageState(PageState.Countdown);
		OnGameOverConfirmed(); //Send to rocketController
		gameOver = false;
	}

	public void ContinueGame () {	
		tryCounter++;			
		curHealth = fullHealth;
		SetPageState(PageState.Countdown);
	}

	public void PauseGame(){
		if (!gamePause) {
			gamePause = true;
			SetPageState (PageState.Pause);
		} else {
			if (gameNew) {
				gamePause = false;
			}
			SetPageState(PageState.Countdown);
		}
	}

	public void GoToMenu () {
		isBonusLevel=false;
		SetPageState(PageState.RocketMenu);
	}

	public void GoToLevelMenu () {
		SetPageState(PageState.LevelMenu);
	}

	public void FGA_GoToMenu () {
		FGA_FailureGoToMenu(RocketManager.curRocket, LevelManager.curLevel, tryCounter);
	}

	public void Restart () {
		FGA_FailureRestart(RocketManager.curRocket, LevelManager.curLevel, tryCounter);
		StartGame();
	}

	public void GoToShop () {
		SetPageState(PageState.Shop);
	}

	public void GoToAdMob () {
		//SetPageState(PageState.None);	
		FGA_EventSelectPromotion("GameOverPage");
		ShowVideoAd("GameOver");
	}	

	public void BuyRocket() {
		FGA_EventPurchase(RocketManager.curRocket);
		savedScore = savedScore - RocketManager.price;
		PlayerPrefs.SetInt ("SaveScore", savedScore);
		PlayerPrefs.Save();

		helpHandsName = HelpHands.StartGame;
		helpHands[(int)helpHandsName].SetActive (true);		
	}

	void InitSpeaker() {

	}

	void InitGame() {
		score = 0;
		diamond = 0;
		tryCounter = 0;
		Transform tGE = gameEnvironment.transform;

		//isBonusLevel = true; // test row

		for (int i = 0; i < tGE.childCount; i++) {
			Transform tChildGE = tGE.GetChild (i);
			switch (tChildGE.name) {

			case "Rockets":
				for (int j = 0; j < tChildGE.childCount; j++) {
					//print (tChildGE.GetChild (j).name);
					if (j == RocketManager.curRocket) {
					//if (j == 4) { // test row
						tChildGE.GetChild (j).gameObject.SetActive (true);
					} else {
						tChildGE.GetChild (j).gameObject.SetActive (false);
					}
				}
				fullHealth = RocketManager.health;
				curHealth = fullHealth;
				curGun = 0;
				//curGun = 3;		//test row
				break;

			case "Levels":								
				for (int j = 0; j < tChildGE.childCount; j++) {					
					//print (tChildGE.GetChild (j).name);
					if ((j == LevelManager.curLevel) && (!isBonusLevel)) {
					//if (j == 9){//to start BONUS lvl
					//print ("true");
						tChildGE.GetChild (j).gameObject.SetActive (true);
					} else {
						tChildGE.GetChild (j).gameObject.SetActive (false);
					}
				}				
				break;

			case "BonusLevels":
				for (int j = 0; j < tChildGE.childCount; j++) {
					//print (tChildGE.GetChild (j).name);
					//if (j == LevelManager.curLevel) {
					if ((j == LevelManager.curLevel) && (isBonusLevel)) {//to start BONUS lvl
					//print ("true");
						tChildGE.GetChild (j).gameObject.SetActive (true);
					} else {
						tChildGE.GetChild (j).gameObject.SetActive (false);
					}
				}
				//isBonusLevel = false;
				break;								
			}
		}
	    
		//init sound
		if (RocketManager.curRocket % 2 == 0){
			// ROCKET speaker
			soundGameOver.clip = soundRocketGameOverClips[UnityEngine.Random.Range(0, soundRocketGameOverClips.Length)];
			soundWin.clip = soundRocketWinClips[UnityEngine.Random.Range(0, soundRocketWinClips.Length)];
		} else {
			// UFO speaker
			soundGameOver.clip = soundUFOGameOverClips[UnityEngine.Random.Range(0, soundUFOGameOverClips.Length)];
			soundWin.clip = soundUFOWinClips[UnityEngine.Random.Range(0, soundUFOWinClips.Length)];			
		}	
	}

/*
// { AD MOB Section for old metods
	void AdMobInit(){
		print ("AdMob old = Init!!!");
        //appID = "ca-app-pub-1339618403883604~1261030067";  // my ID		         

		appID = "ca-app-pub-3940256099942544~3347511713"; // test ID
                
        interstitialAdID = "ca-app-pub-3940256099942544/1033173712"; // free ID		                    
        
        videoAdID = "ca-app-pub-3940256099942544/5224354917"; // free ID for Video Ad    			             
                        
        MobileAds.Initialize(appID);

		this.videoAd = RewardBasedVideoAd.Instance;

        // Called when an ad request has successfully loaded.
        videoAd.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        videoAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        videoAd.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        videoAd.OnAdStarted += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        videoAd.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        videoAd.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        videoAd.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

        //RequestInterstitialAd();
	}

	void VideoAdInitAndRequest (){

		print ("Old Init And Request!!!");		

        AdRequest request = AdRequestBuild();
        videoAd.LoadAd(request, videoAdID);		
	}

	AdRequest AdRequestBuild(){
        return new AdRequest.Builder().Build();
    }	

	void VideoAdDestroy (){
        // Called when an ad request has successfully loaded.
        videoAd.OnAdLoaded -= HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        videoAd.OnAdFailedToLoad -= HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        videoAd.OnAdOpening -= HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        videoAd.OnAdStarted -= HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        videoAd.OnAdRewarded -= HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        videoAd.OnAdClosed -= HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        videoAd.OnAdLeavingApplication -= HandleRewardBasedVideoLeftApplication;

		//videoAd.Destroy();
	}

	// ////////
	 public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoClosed event received");


		VideoAdInitAndRequest ();

		ContinueGame();
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
    }
	// ////////

    public void ShowVideoAd(){
		print("Show video - "+ videoAd.IsLoaded());
        if(videoAd.IsLoaded()){
            videoAd.Show();
        }
    }


// } AD MOB Section for old metods
*/

// { AD MOB Section for new metods
	void AdMobInit(bool testMod){
		//print ("AdMob = Init!!!");
        appID = "ca-app-pub-1339618403883604~1261030067";  // my ID		         
                
		if (testMod){
			//Debug.Log("Test mod!!!");
        	interstitialAdID = "ca-app-pub-3940256099942544/1033173712"; // free AD ID		 
        	videoAdID = "ca-app-pub-3940256099942544/5224354917"; // free AD reward ID
		} else {
			//Debug.Log ("Buy mod!!!");
        	interstitialAdID = "ca-app-pub-1339618403883604/2828234933"; // my AD ID		                            
        	videoAdID = "ca-app-pub-1339618403883604/4737364355"; // my AD reward ID
		}
                        
        //MobileAds.Initialize(appID);
		MobileAds.Initialize(initStatus => { 
            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
            {
                string className = keyValuePair.Key;
                AdapterStatus status = keyValuePair.Value;
                switch (status.InitializationState)
                {
                    case AdapterState.NotReady:
                        // The adapter initialization did not complete.
                        MonoBehaviour.print("Adapter: '" + className + "' not ready --> " + status.Description);
                        break;
                    case AdapterState.Ready:
                        // The adapter was successfully initialized.
                        MonoBehaviour.print("Adapter: '" + className + "' is initialized.");
                        break;
                }
            }			
		});
        //RequestInterstitialAd();
	}

	AdRequest AdRequestBuild(){
        return new AdRequest.Builder().Build();
    }	

	// { video AD 
	void VideoAdInitAndRequest (){

		//print ("Init And Request!!!");

        videoAd = new RewardedAd(videoAdID);

		// Called when an ad request has successfully loaded.
        this.videoAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.videoAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.videoAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.videoAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.videoAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.videoAd.OnAdClosed += HandleRewardedAdClosed;			

        AdRequest request = AdRequestBuild();
        videoAd.LoadAd(request);		
	}

	void VideoAdDestroy (){
		// Called when an ad request has successfully loaded.
        this.videoAd.OnAdLoaded -= HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.videoAd.OnAdFailedToLoad -= HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.videoAd.OnAdOpening -= HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.videoAd.OnAdFailedToShow -= HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.videoAd.OnUserEarnedReward -= HandleUserEarnedReward;
        // Called when the ad is closed.
        this.videoAd.OnAdClosed -= HandleRewardedAdClosed;			

		//videoAd.Destroy();
	}

	// ////////
	public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
		//print ("LOADED --- LOADED --- LOADED");
		videoAdLoaded=true;
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
		//print ("FAILED --- FAILED --- FAILED");
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
		print("Sender : " + sender);
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
		videoAdLoaded=false;
		MonoBehaviour.print("HandleRewardedAdClosed event received");

		VideoAdDestroy();

		VideoAdInitAndRequest();
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {	
		//print("Sender : "+sender);
		//print("Args.Type : "+args.Type);
		//print("Args.Amount : "+args.Amount);
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
		StartCoroutine(coroContinueGame());
    }

	IEnumerator coroContinueGame(){
		yield return new WaitForSecondsRealtime(0.1f);
		switch (nameCall){
			case "GameOver":
				ContinueGame();
				break;
			case "GameWin":
				MultiplyScore();
				break;	
			case "BonusBox":
				Debug.Log("Game Manager - Rewarded!!!");
				OnWtchedRewardedVideo();  // Send to BonusBoxController to open Box
				break;					
			default:
				break;
		}		
	}
	// ////////

    public void ShowVideoAd(string nameBtn){
		nameCall = nameBtn;
		//print("Show video - "+ videoAd.IsLoaded());
        if(videoAd.IsLoaded()){
            videoAd.Show();
		}
    }
	// } video AD 

	// { Interstitial Ad

	void InterstitialAdInitAndRequest (){

		//print ("Init And Request!!!");

        interstitialAd = new InterstitialAd(interstitialAdID);

		// Called when an ad request has successfully loaded.
        this.interstitialAd.OnAdLoaded += HandleInterstitialAdLoaded;
        // Called when an ad request failed to load.
        this.interstitialAd.OnAdFailedToLoad += HandleInterstitialAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitialAd.OnAdOpening += HandleInterstitialAdOpening;
        // Called when the ad is closed.
        this.interstitialAd.OnAdClosed += HandleInterstitialAdClosed;			

        AdRequest request = AdRequestBuild();
        interstitialAd.LoadAd(request);		
	}

	void InterstitialAdDestroy (){
		// Called when an ad request has successfully loaded.
        this.interstitialAd.OnAdLoaded -= HandleInterstitialAdLoaded;
        // Called when an ad request failed to load.
        this.interstitialAd.OnAdFailedToLoad -= HandleInterstitialAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitialAd.OnAdOpening -= HandleInterstitialAdOpening;
        // Called when the ad is closed.
        this.interstitialAd.OnAdClosed -= HandleInterstitialAdClosed;					

		interstitialAd.Destroy();
	}

	// ////////
	public void HandleInterstitialAdLoaded(object sender, EventArgs args)
    {
		interstitialAdLoaded=true;
        MonoBehaviour.print("HandleInterstitialAdLoaded event received");
    }

    public void HandleInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleInterstitialAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleInterstitialAdOpening(object sender, EventArgs args)
    {		
        MonoBehaviour.print("HandleInterstitialAdOpening event received");
		adWindowStatus = true;
		//print ("adWindowStatus - TRUE");
    }

    public void HandleInterstitialAdClosed(object sender, EventArgs args)
    {
		interstitialAdLoaded=false;
		iClosed++;
		print ("adWindowStatus - "+ adWindowStatus + "[ "+iClosed+" ]");
		MonoBehaviour.print("HandleInterstitialAdClosed event received");

		if (adWindowStatus){
			//print ("adWindowStatus IN - "+ adWindowStatus);
			InterstitialAdDestroy();
			InterstitialAdInitAndRequest();

			StartCoroutine(coroBackFromWin());
			adWindowStatus = false;
		}
		//ContinueGame();
    }

	IEnumerator coroBackFromWin() {
		yield return new WaitForSecondsRealtime(0.1f);
		switch (nameCall){
			case "Menu":
				GoToMenu();
				break;
			case "Next":
				LevelManager.curLevel++;
				StartGame();
				break;
			case "Previous":
				LevelManager.curLevel--;
				StartGame();
				break;				
			case "Play":
				LevelManager.curLevel++;
				StartGame();
				break;		
			case "PlayBonus":
				StartGame();
				break;				
			case "Back":
				FGA_EventSelectPromotion("GameEndPage");			
				break;
			case "Restart":
				StartGame();
				break;						
			default:
				GoToMenu();
				break;
		}
	}
	// ////////

    public void ShowInterstitialAd(string nameBtn){
		nameCall = nameBtn;
		//print("Show video - "+ interstitialAd.IsLoaded());
        if(interstitialAd.IsLoaded()){
            interstitialAd.Show();
        } else {			
			switch (nameCall){
				case "Menu":
					GoToMenu();
					break;
				case "Next":
					LevelManager.curLevel++;
					StartGame();
					break;
				case "Previous":
					LevelManager.curLevel--;
					StartGame();
					break;				
				case "Play":
					LevelManager.curLevel++;
					StartGame();
					break;
				case "PlayBonus":
					StartGame();
					break;
				case "Back":
					FGA_EventSelectPromotion("GameEndPage");
					break;
				case "Restart":
					StartGame();
					break;					
				default:
					GoToMenu();
					break;
			}			
		}
    }

	// } Interstitial Ad


// } AD MOB Section for new metods

	string GetRocketName(int iRocket){
		if (iRocket % 2 == 0){
			return "Rocket_"+iRocket;
		} else {
			return "UFO_"+iRocket;
		}
	}

// { Analitics
	void AnaliticsInit () {
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => 
		{
			dependencyStatus = task.Result;
			if (dependencyStatus == DependencyStatus.Available) 
			{
				//print("Init Analitics!!!");
				InitializeFirebase();
				
			} else {
				Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        	}
      	});		
	}

	void InitializeFirebase() {
    	Debug.Log("Enabling data collection.");
      	FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

      	Debug.Log("Set user properties.");
		FGA_SetCurrentScreen("RocketMenuPage");
      	// Set the user's sign up method.
      	//FirebaseAnalytics.SetUserProperty(FirebaseAnalytics.UserPropertySignUpMethod, "Google");
      	// Set the user ID.
      	// FirebaseAnalytics.SetUserId("uber_user_510");
      	// Set default session duration values.
      	// FirebaseAnalytics.SetMinimumSessionDuration(new TimeSpan(0, 0, 10));
      	// FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
		// Firebase.Analytics.FirebaseAnalytics.LogEvent("custom_progress_event", "percent", 0.4f);
      	firebaseInitialized = true;
    }

	void FGA_LevelStart(int iRocket, int iLevel){
		Parameter[] paramEvent = {
			new Parameter (FirebaseAnalytics.ParameterCharacter, GetRocketName(iRocket)),
			new Parameter (FirebaseAnalytics.ParameterLevel, iLevel),
		};
		FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, paramEvent);
		FirebaseAnalytics.LogEvent("Start_Level_"+iLevel.ToString());
		FirebaseAnalytics.LogEvent("Start_Level_"+GetRocketName(iRocket));
	}	

	void FGA_BonusLevelStart(int iRocket, int iLevel){
		FirebaseAnalytics.LogEvent("Start_Bonus_Level_"+iLevel.ToString());
		FirebaseAnalytics.LogEvent("Start_Bonus_Level_"+GetRocketName(iRocket));
	}		

	void FGA_LevelFailure(int iRocket, int iLevel, int nTry){
		Parameter[] paramEvent = {
			new Parameter (FirebaseAnalytics.ParameterCharacter, GetRocketName(iRocket)),
			new Parameter (FirebaseAnalytics.ParameterLevel, iLevel),
			new Parameter ("TryCount", nTry),
		};
		FirebaseAnalytics.LogEvent("EventLevelFailure", paramEvent);
		FirebaseAnalytics.LogEvent("Failure_Level_"+iLevel.ToString());
		FirebaseAnalytics.LogEvent("Failure_Level_"+GetRocketName(iRocket));
	}

	void FGA_FailureRestart(int iRocket, int iLevel, int nTry){
		Parameter[] paramEvent = {
			new Parameter (FirebaseAnalytics.ParameterCharacter, GetRocketName(iRocket)),
			new Parameter (FirebaseAnalytics.ParameterLevel, iLevel),
			new Parameter ("TryCount", nTry),
		};
		FirebaseAnalytics.LogEvent("FailureRestart", paramEvent);
		FirebaseAnalytics.LogEvent("Failure_Level_"+iLevel.ToString()+"_Select_Restart(Try: "+nTry.ToString()+")");
	}	

	void FGA_FailureGoToMenu(int iRocket, int iLevel, int nTry){
		Parameter[] paramEvent = {
			new Parameter (FirebaseAnalytics.ParameterCharacter, GetRocketName(iRocket)),
			new Parameter (FirebaseAnalytics.ParameterLevel, iLevel),
			new Parameter ("TryCount", nTry),
		};
		FirebaseAnalytics.LogEvent("FailureGoToMenu", paramEvent);
		FirebaseAnalytics.LogEvent("Failure_Level_"+iLevel.ToString()+"_Select_GoToMenu(Try: "+nTry.ToString()+")");
	}	

	void FGA_LevelEnd(int iRocket, int iLevel, int nTry, int nScore){
		Parameter[] paramEvent = {
			new Parameter (FirebaseAnalytics.ParameterCharacter, GetRocketName(iRocket)),
			new Parameter (FirebaseAnalytics.ParameterLevel, iLevel),
			new Parameter ("TryCount", nTry),
			new Parameter (FirebaseAnalytics.ParameterScore, nScore),
		};
		FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, paramEvent);

		FirebaseAnalytics.LogEvent("End_Level_"+iLevel.ToString());
		FirebaseAnalytics.LogEvent("End_Level_"+GetRocketName(iRocket));
	}
	void FGA_BonusLevelEnd(int iRocket, int iLevel, int nScore){
		FirebaseAnalytics.LogEvent("End_Bonus_Level_"+iLevel.ToString());
		FirebaseAnalytics.LogEvent("End_Bonus_Level_"+GetRocketName(iRocket));
	}	

	void FGA_GameEnd(int iRocket, int iLevel, int nTry, int nScore){
		Parameter[] paramEvent = {
			new Parameter (FirebaseAnalytics.ParameterCharacter, GetRocketName(iRocket)),
			new Parameter (FirebaseAnalytics.ParameterLevel, iLevel),
			new Parameter ("TryCount", nTry),
			new Parameter (FirebaseAnalytics.ParameterScore, nScore),
		};
		FirebaseAnalytics.LogEvent("GameEnd", paramEvent);
	}

	public void FGA_BuyDiamonds (int quantity) {
		//Debug.Log("Buy Diamond: "+quantity.ToString());
		Parameter[] paramEvent = {
			new Parameter (FirebaseAnalytics.ParameterVirtualCurrencyName, "Diamonds"),
			new Parameter (FirebaseAnalytics.ParameterValue, quantity),
		};
		FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventEarnVirtualCurrency, paramEvent);
		FirebaseAnalytics.LogEvent("Buy_Diamonds_"+quantity.ToString());
	}

	void FGA_EventPurchase (int iRocket){
		Parameter[] paramEvent = {
			new Parameter (FirebaseAnalytics.ParameterCharacter, GetRocketName(iRocket)),
		};
		FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPurchase, paramEvent);
		FirebaseAnalytics.LogEvent("Buy_SpaceShip_"+GetRocketName(iRocket));
	}


	public void FGA_EventSelectPromotion (string nameScreen){
		Parameter[] paramEvent = {
			new Parameter ("FromScreen", nameScreen),
		};
		FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectPromotion, paramEvent);
		FirebaseAnalytics.LogEvent("Select_Rewarded_Ad_From_"+nameScreen);
	}	

	void FGA_SetCurrentScreen (string nameScreen) {
		FirebaseAnalytics.SetCurrentScreen(nameScreen, null);
	}

	public void FGA_EventShare (string nameScreen){
		Parameter[] paramEvent = {
			new Parameter ("FromScreen", nameScreen),
		};
		FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventShare, paramEvent);
		FirebaseAnalytics.LogEvent("Like_App_From_"+nameScreen);
	}	

	void FGA_BonusBoxWindow (){
		FirebaseAnalytics.LogEvent("BonusBoxWindow");
	}			
	
	void FGA_BonusBoxClose (){
		FirebaseAnalytics.LogEvent("BonusBoxClose");
	}			

	void FGA_BonusBoxOpenDiamond (){
		FirebaseAnalytics.LogEvent("BonusBoxOpenDiamond");
	}

	void FGA_BonusBoxOpenRewardedAd (){
		FirebaseAnalytics.LogEvent("BonusBoxOpenRewardedAd");
	}

	void FGA_BonusBoxCloseAfterOpen () {
		FirebaseAnalytics.LogEvent("BonusBoxCloseAfterOpen");
	}

// }

	void BonusGenerator () {
		//print ("Cur SESSION: "+savedNumSession);
		//print ("Cur LEVEL: "+LevelManager.curLevel);
		//print ("Cur BONUS: "+isBonusLevel);
		if (savedNumSession == 1) {
			switch (LevelManager.curLevel)
			{
				case 0:
					if (isBonusLevel){
						StartCoroutine(coroBonus("BonusBox"));						
					} else {
						StartCoroutine(coroBonus("BonusLevel"));						
					}
					break;
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
					if (!isBonusLevel){
						if ((winCounter > 0) && ((winCounter % 2) == 0)) {
							StartCoroutine(coroBonus("BonusLevel"));
						}
					}
					break;
				default:
					break;
			}
		} else {
			switch (LevelManager.curLevel){
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
					if (!isBonusLevel) {
						if (firstLevelInSession || ((winCounter > 0) && ((winCounter % 2) == 0))) {
							StartCoroutine(coroBonus("BonusLevel"));
						} else {
							if (UnityEngine.Random.Range(0, 9) >= 8) {					
								StartCoroutine(coroBonus("BonusBox"));
							}
						}
					}
					break;
				default:
					break;				
			}
		}
		if (isBonusLevel){
			isBonusLevel = false;
		}
	}

	void BonusLevelGenerator() {	
		//print("Bonus Generator!!!");
		if ((((savedNumSession == 1) && LevelManager.curLevel == 0)) || 
			(firstLevelInSession) || 
			((winCounter > 0) && ((winCounter % 2) == 0))) {
		//if (true){
			StartCoroutine(coroBonus("BonusLevel"));
		}
	}

	void BonusBoxGenerator() {	
		//print("Bonus Generator!!!");
		if ((failureCounter >0) && ((failureCounter % 2) == 0)) {
			StartCoroutine(coroBonus("BonusBox"));	
		}
	}
		
	IEnumerator coroBonus(string nameCall){
		yield return new WaitForSecondsRealtime(1.5f);
		switch (nameCall){
			case "BonusLevel":
				isBonusLevel=true;

				// sound effect
				soundBonusLevel.clip = soundBonusLevelWindowClips[UnityEngine.Random.Range(0, soundBonusLevelWindowClips.Length)];
				soundBonusLevel.Play();

				SetPageState (PageState.BonusLevel);
				break;
			case "BonusBox":
				isBonusBox=true;
				FGA_BonusBoxWindow ();
				bonusBox.SetActive (true);
				break;	
			default:
				break;
		}		
	}

	public void BonusBoxClose(){
		FGA_BonusBoxClose ();
		bonusBox.SetActive (false);	
	}

	public void BonusBoxOpenDiamond(){
		FGA_BonusBoxOpenDiamond ();
	}

	public void BonusBoxOpenRewardedAd(){
		FGA_BonusBoxOpenRewardedAd ();
	}	

	public void BonusBoxCloseAfterOpen(){
		FGA_BonusBoxCloseAfterOpen ();
		if (savedNumSession == 1) {
			switch (LevelManager.curLevel){
				case 0:
					bonusBox.SetActive (false);
					SetPageState(PageState.RocketMenu);
					helpHandsName = HelpHands.BuyRocket2;
					helpHands[(int)helpHandsName].SetActive (true);
					LevelManager.curLevel++;
					break;
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
					bonusBox.SetActive (false);				
					break;
				default:
					bonusBox.SetActive (false);
					break;
			}
		} else {
			bonusBox.SetActive (false);
		}
	}

	private void DisableHands(){
		for (int i = 0; i < helpHands.Length; i++) {
			helpHands[i].SetActive(false);
		}
	}
}
