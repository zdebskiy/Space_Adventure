using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class rocketController : MonoBehaviour {

	public delegate void RocketDelegate ();
	public static event RocketDelegate	OnRocketDamaged;
	public static event RocketDelegate	OnRocketScored;
	public static event RocketDelegate	OnRocketWin;
	public static event RocketDelegate	OnRocketFinish;
	public static event RocketDelegate	OnRocketTakeHeart;
	public static event RocketDelegate	OnRocketTakeHealPotion;
	public static event RocketDelegate	OnRocketTakeDiamond;

	public delegate void RocketDelegateStr (string s);
	public static event RocketDelegateStr OnRocketTakeGun;


	public Vector3 startPos;
	public int fullHealth; 
	public int speed;
	public int price;
	
	public Joystick joyStick;
	public Transform tShotButton;
	public Camera curCamera;

	public GameObject[] prefabGuns;
	private bool readyToShot = true;
	private float timeCoolDown;
	private float timerCoolDown;
	private Image imgShotButton;
	private Image imgShotButtonBg;
	private Button btnShotButton;
	private int nShots;
	private int iShot;

	private Animation animRocketDmg;

	[System.Serializable]
	public struct YRange {
		public float min;
		public float max;
	}

	[System.Serializable]		
	public struct XRange {
		public float min;
		public float max;
	}

	public YRange yRange;
	public XRange xRange;

	public AudioSource motorAudioSource;
	public AudioClip [] motorAudioClips;

	public AudioSource speakerAudioSource;
	public AudioClip [] speakerScoreAudioClips;
	public AudioClip [] speakerDamageAudioClips;
	public AudioClip [] speakerHeartAudioClips;
	public AudioClip [] speakerHealPotionAudioClips;
	public AudioClip [] speakerDiamondAudioClips;
	public AudioClip [] speakerNewWeaponAudioClips;
	
	public AudioSource gunAudioSource;
	public AudioClip [] gunShotAudioClips;
	public AudioClip [] gunRechargeAudioClips;
	public AudioClip [] gunNotReadyAudioClips;


	private Rigidbody2D rb;
	private CapsuleCollider2D colRocket;

	//private GameObject rocket;
	//private AudioSource scoreSound;

	// Use this for initialization
	private void Awake() {
		btnShotButton = tShotButton.GetComponent<Button>();		
	} 

	void Start () {
		print ("Start rocket: " + transform.name);		
		animRocketDmg = GetComponent <Animation> ();
		rb = GetComponent <Rigidbody2D> ();		
		colRocket = GetComponent <CapsuleCollider2D> ();
	
		motorAudioSource.volume = 0.2f;
		motorAudioSource.pitch = 0.8f;		

		gunAudioSource.volume = 0.5f;		

		nShots = speed*5;
		iShot = 0;
		timeCoolDown = (6-speed)/2.0f;
		timerCoolDown = 0;
		imgShotButton = tShotButton.GetComponent<Image>();
		imgShotButtonBg = tShotButton.GetChild(0).GetComponent<Image>();				
	}

	void OnEnable (){
		//print ("OnEnable rocket: " + transform.name);
		GameManager.OnGameStarted += OnGameStarted;
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
		GameManager.OnRocketDie += OnRocketDie;
		//print("Count: "+btnShotButton.onClick.GetPersistentEventCount());		
		btnShotButton.onClick.AddListener(shot);		
	}

	void OnDisable (){
		GameManager.OnGameStarted -= OnGameStarted;
		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
		GameManager.OnRocketDie -= OnRocketDie;
		btnShotButton.onClick.RemoveListener(shot);
	}

	void OnGameStarted(){		
		rb.velocity = Vector3.zero;
		motorAudioSource.clip = motorAudioClips[UnityEngine.Random.Range(0, motorAudioClips.Length)];
		motorAudioSource.Play();
		//rb.simulated = true;
	}

	void OnGameOverConfirmed () {		
		transform.localPosition = startPos; //transform.SetPositionAndRotation(startPos, Quaternion.identity);
		transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);		
	}

	// Update is called once per frame
	void Update () {

		if (GameManager.gameNew) {
			readyToShot=true;
			timerCoolDown = 0;
			iShot=0;
			imgShotButton.fillAmount=1.0f;
			PaintShotButton(GameManager.curGun.ToString());		
		}

		if ((GameManager.gamePause) || (GameManager.gameNew) || (GameManager.gameOver)) return;

		//float moveX = Input.GetAxis ("Horizontal");
		//float moveY = Input.GetAxis ("Vertical");

		float moveX = joyStick.Horizontal;
		float moveY = joyStick.Vertical;
		float moveZ = transform.position.z;
		
		Vector3 moveVector = Vector3.zero;

		if (moveX < 0) {
			//moveVector = new Vector3 (moveX * (speed * 0.5f) * 10 * Time.deltaTime, moveY * (speed * 0.5f) * 5 * Time.deltaTime, moveZ);			
			moveVector = new Vector3 (moveX * 10 * Time.deltaTime, moveY * 5 * Time.deltaTime, moveZ); // все ракеты на всех уровнях одной скорости, меняется только скорость прокрутки левела
		} else {
			//moveVector = new Vector3 (moveX * (speed * 0.5f) * 5 * Time.deltaTime, moveY * (speed * 0.5f) * 5 * Time.deltaTime, moveZ);			
			moveVector = new Vector3 (moveX * 5 * Time.deltaTime, moveY * 5 * Time.deltaTime, moveZ); // все ракеты на всех уровнях одной скорости, меняется только скорость прокрутки левела
		}

		

		Vector3 newPosition = rb.position;
		Vector3 futurePosition = new Vector3 (rb.position.x, rb.position.y, -90)  + moveVector;
		Vector3 borderControl = curCamera.WorldToViewportPoint(futurePosition);
		//print("POINT: [ X : " + borderControl.x + " ][ Y : " + borderControl.y + " ]");


		//print ("futurePosition: [ " + futurePosition.x + " ] [ " + futurePosition.y + " ] [ " + futurePosition.z + " ]");

		if ((borderControl.x > 0.1f) && (borderControl.x < 0.9f)) {
			newPosition.x = futurePosition.x;
		}

		if ((borderControl.y > 0.1f) && (borderControl.y < 0.9f)) {
			newPosition.y = futurePosition.y;
		}

		rb.MovePosition (newPosition);

		
		motorAudioSource.volume = 0.2f+0.8f*Math.Max(Math.Abs(moveX), Math.Abs(moveY));
		motorAudioSource.pitch = 0.7f+0.3f*Math.Max(Math.Abs(moveX), Math.Abs(moveY));

		shotCoolDown();

	}

	void OnTriggerEnter2D(Collider2D col) {		
		if (col.gameObject.tag == "ScoreZone") {
			//регистрация события поднятия Score
			speakerAudioSource.clip = speakerScoreAudioClips[UnityEngine.Random.Range(0, speakerScoreAudioClips.Length)];
			speakerAudioSource.Play();
			OnRocketScored();  //event sent to GameManager
		}
		if (col.gameObject.tag == "Heart") {
			//регистрация события поднятия Heart
			speakerAudioSource.clip = speakerHeartAudioClips[UnityEngine.Random.Range(0, speakerHeartAudioClips.Length)];
			speakerAudioSource.Play();
			OnRocketTakeHeart();  //event sent to GameManager
		}
		if (col.gameObject.tag == "HealPotion") {
			//регистрация события поднятия HealPotion
			speakerAudioSource.clip = speakerHealPotionAudioClips[UnityEngine.Random.Range(0, speakerHealPotionAudioClips.Length)];
			speakerAudioSource.Play();
			OnRocketTakeHealPotion();  //event sent to GameManager
		}		
		if (col.gameObject.tag == "DamageZone") {
			//регистрация события смерти Damage
			//print ("Rocket Damaged!");								
			OnRocketDamaged(); // //event sent to GameManager
			if (GameManager.curHealth != 0){
				speakerAudioSource.clip = speakerDamageAudioClips[UnityEngine.Random.Range(0, speakerDamageAudioClips.Length)];
				speakerAudioSource.PlayDelayed(0.25f);
			}							
			animRocketDmg.Play("rocket_take_damage");
		}
		if (col.gameObject.tag.Contains("GunZone")) {
			// Tag zone TakeGun: 'GunZone_0' 'GunZone_1' 'GunZone_2'
			string numberGun=col.gameObject.tag.Substring(col.gameObject.tag.IndexOf('_')+1); // Cut number of gun
			OnRocketTakeGun(numberGun); //event sent to GameManager number of taken gun
			PaintShotButton(numberGun);
			speakerAudioSource.clip = speakerNewWeaponAudioClips[UnityEngine.Random.Range(0, speakerNewWeaponAudioClips.Length)];
			speakerAudioSource.Play();						
		}

		if (col.gameObject.tag.Contains("Diamond")) {			
			OnRocketTakeDiamond(); //event sent to GameManager
			speakerAudioSource.clip = speakerDiamondAudioClips[UnityEngine.Random.Range(0, speakerDiamondAudioClips.Length)];
			speakerAudioSource.Play();			
		}		

		if (col.gameObject.tag == "WinZone") {
			//регистрация события смерти Damage
			//print ("Rocket Damaged!");	
			OnRocketFinish();		
			colRocket.enabled = false;
			animRocketDmg.Play("rocket_landing");
			StartCoroutine("RocketLanding");
		}
	}

	IEnumerator RocketLanding(){
		int count=2;
		for (int i = 0; i < count; i++) {				
			yield return new WaitForSeconds (1);
		}
		OnRocketWin(); // //event sent to GameManager
		colRocket.enabled = true;
	}

	private void OnRocketDie() {
		motorAudioSource.Stop();
	}

	public void audioRocketPause(){
		motorAudioSource.Pause();
	}

	public void audioRocketPlay(){
		motorAudioSource.Play();
	}	

	public void shot (){
		if ((readyToShot) && (!GameManager.gameNew)){
			// generate Bullet
			GameObject go = Instantiate (prefabGuns[GameManager.curGun]) as GameObject;
			Transform t = go.transform;			
			t.position = new Vector3 (transform.position.x+1.5f, transform.position.y, transform.position.z);					
			//gunAudioSource.clip = gunShotAudioClips[UnityEngine.Random.Range(0, gunShotAudioClips.Length)];
			gunAudioSource.PlayOneShot(gunShotAudioClips[UnityEngine.Random.Range(0, gunShotAudioClips.Length)]);
			// start CoolDown to next shot
			iShot++;
			imgShotButton.fillAmount = (1.0f/nShots * (nShots-iShot));
			if (iShot == nShots){
				readyToShot = false;
			}
		} else {
			//play sound not ready to shot
			if (!GameManager.gameNew){
				gunAudioSource.clip = gunNotReadyAudioClips[UnityEngine.Random.Range(0, gunNotReadyAudioClips.Length)];
				gunAudioSource.Play();			
			}
		}
		
		
		// draw CoolDown on button
	}

	private void shotCoolDown(){		
		if (!readyToShot) {			
			timerCoolDown += Time.deltaTime;
			imgShotButton.fillAmount = timerCoolDown / timeCoolDown;
			if (timerCoolDown > timeCoolDown) {
				readyToShot = true;
				iShot = 0;
				imgShotButton.fillAmount=1.0f;
				timerCoolDown = 0;
				gunAudioSource.clip = gunRechargeAudioClips[UnityEngine.Random.Range(0, gunRechargeAudioClips.Length)];
				gunAudioSource.Play();							
				//print (GameManager.gameNew + " : " + GameManager.gamePause + " : " + GameManager.gameOver);
			}
		}
	}

	private void PaintShotButton(string numberGun){
		//print("PaintButton --->>> "+numberGun);
		Color[] GUNS_BTN_COLOR ={new Color(0.9686275f, 0.7843137f, 0.2627451f, 1.0f), new Color(0.275f, 0.404f, 0.063f, 1.0f), new Color(0.651f, 0.40f, 0.847f, 1.0f), new Color(0.40f, 0.816f, 0.976f, 1.0f)};
		Color[] GUNS_BG_BTN_COLOR ={new Color(0.827451f, 0.5137255f, 0.09411765f, 1.0f), new Color(0.153f, 0.224f, 0.035f, 1.0f), new Color(0.349f, 0.157f, 0.506f, 1.0f), new Color(0.0f, 0.604f, 0.808f, 1.0f)};
		switch (numberGun)
		{
			case "0":
				imgShotButton.color = GUNS_BTN_COLOR[0];
				imgShotButtonBg.color = GUNS_BG_BTN_COLOR[0];
				break;
			case "1":
				imgShotButton.color = GUNS_BTN_COLOR[1];
				imgShotButtonBg.color = GUNS_BG_BTN_COLOR[1];
				break;			
			case "2":
				imgShotButton.color = GUNS_BTN_COLOR[2];
				imgShotButtonBg.color = GUNS_BG_BTN_COLOR[2];
				break;							
			case "3":
				imgShotButton.color = GUNS_BTN_COLOR[3];
				imgShotButtonBg.color = GUNS_BG_BTN_COLOR[3];			
				break;			
			default:
				imgShotButton.color = GUNS_BTN_COLOR[0];
				imgShotButtonBg.color = GUNS_BG_BTN_COLOR[0];			
				break;			
		}
	}

	
}
