using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour {

	class PoolObject {
		public Transform transform;
		public bool inUse;

		public PoolObject(Transform t) { transform = t; }
		public void Use () { inUse = true; }
		public void Dispose () { inUse = false; }
	}

	[System.Serializable]
	public struct YSpawnRange {
		public float min;
		public float max;
	}		

	public GameObject[] Prefab;
	public float scalePrefab = 1.0f;
	public int poolSize;
	public float shiftSpeed;
	public Vector3 shiftVector = Vector3.left;
	public float spawnRate;

	public YSpawnRange ySpawnRange; 
	public Vector3 defaultSpawnPos;
	public bool spawnImmediate;
	public Vector3 immediateSpawnPos;
	public Vector2 targetAspectRatio;
	public bool isBackground;

	float spawnTimer;
	float targetAspect;
	float yVectorAsteroid;
	PoolObject[] poolObjects = null;

	GameManager game;

	void Awake() {
		Configure ();
	}

	void Start(){
		game = GameManager.instance;
		GameManager.OnGameOverConfirmed += this.OnGameOverConfirmed;
		//rocketController.OnRocketWin += OnRocketWin;
	}

	void OnEnale (){
//		GameManager.OnGameStarted += OnGameStarted;
//		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;		
//		rocketController.OnRocketScored += OnRocketScored;
//		rocketController.OnRocketWin += OnRocketWin;
	}

	void OnDisable (){
//		GameManager.OnGameStarted -= OnGameStarted;
//		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;		
//		rocketController.OnRocketScored -= OnRocketScored;
//		rocketController.OnRocketWin -= OnRocketWin;
	}

	void OnGameOverConfirmed(){		
		//print ("par - GO Confirm");
		
		for (int i = 0; i < poolObjects.Length; i++) {
			poolObjects [i].Dispose ();
			poolObjects [i].transform.position = Vector3.one * 1000;
		}		
		Configure ();
	}		

	void Update () {
		

		if (isBackground) {
			if (!GameManager.gameMenu) {
				if (GameManager.gameNew) {				
					if (GameManager.gamePause) {
						//print ("isBak, newGame, Pause");
						return;
					}
				} else {
					if ((GameManager.gamePause) || (GameManager.gameOver)) {					
						//print ("isBak, Pause || gameOver");
						return;
					}
				}
			}
		} else {
			
			if ((GameManager.gameNew) || (GameManager.gamePause) || (GameManager.gameOver)) {
				//print ("not_isBak, Pause || gameOver");
				return;	
			}

		}			

		Shift ();

		spawnTimer += Time.deltaTime;
		if (spawnTimer > spawnRate) {
			Spawn ();
			spawnTimer = 0;
			//print (GameManager.gameNew + " : " + GameManager.gamePause + " : " + GameManager.gameOver);
		}
	}

	void Configure() {
		spawnTimer = 0;
		targetAspect = targetAspectRatio.x / targetAspectRatio.y;

		if (poolObjects == null) {
			//print ("Config: null");
			poolObjects = new PoolObject[poolSize];
			for (int i = 0; i < poolObjects.Length; i++) {
				GameObject go = Instantiate (Prefab[Random.Range(0, Prefab.Length)]) as GameObject;
				Transform t = go.transform;
				t.SetParent (transform);
				t.position = Vector3.one * 1000;
				t.localScale = t.localScale * scalePrefab;
				poolObjects [i] = new PoolObject (t);
			}
		} else {
			//print ("Config: Not null");
			for (int i = 0; i < poolObjects.Length; i++) {
				poolObjects [i].transform.position = Vector3.one * 1000;
			}
		}

		if (spawnImmediate) {
			SpawnImmediate();
		}						
	}

	void Spawn (){
		Transform t = GetPoolObject ();
		if (t == null) return;
		Vector3 pos = Vector3.zero;		
		pos.x = defaultSpawnPos.x;
		pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
		t.position = pos;
	}

	void SpawnImmediate (){
		Transform t = GetPoolObject ();
		if (t == null) return;
		Vector3 pos = Vector3.zero;
		pos.x = immediateSpawnPos.x;
		pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
		t.position = pos;
		Spawn ();
	}

	void Shift (){
		for (int i = 0; i < poolObjects.Length; i++) {
			//print("Pos[ "+i+" ] = "+poolObjects [i].transform.position);
			if (poolObjects [i].inUse) {
				if ((poolObjects [i].transform.position.z <= 89.9f) || (poolObjects [i].transform.position.z >= 90.1f)) {
					//print ("Pool pos: Shifted!");
					if (poolObjects [i].transform.parent.parent.name != "LiveBackground"){
						poolObjects [i].transform.localPosition += shiftVector * (shiftSpeed - (shiftSpeed * RocketManager.speed/10))  * Time.deltaTime;		// прокрутка зависит от скорости ракеты
						//poolObjects [i].transform.localPosition += shiftVector * shiftSpeed * Time.deltaTime; // прокурутка не зависит от скорости ракеты (упрощение происходт за счет управляемости ракеты)
					} else {
						poolObjects [i].transform.localPosition += shiftVector * shiftSpeed * Time.deltaTime;
					}
				}
				CheckDisposeObject (poolObjects [i]);
			}			
		}
			
	}

	void CheckDisposeObject(PoolObject poolObject) {
		if ((poolObject.transform.position.x < -defaultSpawnPos.x) || (poolObject.transform.position.x > 800)) {
			poolObject.Dispose ();
			poolObject.transform.position = Vector3.one * 1000;
		}
	}

	Transform GetPoolObject () {
		for (int i = 0; i < poolObjects.Length; i++) {
			if (!poolObjects [i].inUse) {
				poolObjects [i].Use ();
				return poolObjects [i].transform;
			}
				
		}
		return null;
	}


	void OnRocketWin () {
		//print("RocketWin Destroy!!!");
		for (int i = 0; i < poolObjects.Length; i++) {
			poolObjects [i].Dispose ();
			Destroy(poolObjects [i].transform.gameObject, 0.5f);
		}
		poolObjects = null;
	}		

}
