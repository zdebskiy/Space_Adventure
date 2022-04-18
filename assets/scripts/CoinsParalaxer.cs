using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsParalaxer : MonoBehaviour
{
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


	//public YSpawnRange ySpawnRange; 
    private float defaultSpawnXPosition = 12f;	
    private int curNumberSpawnPosition = 0;
	private int numberSpawnProgram = 0;

	private Dictionary<int, float[][]> defaultSpawnYPositions = new Dictionary<int, float[][]>();
	private Dictionary<int, float[]> defaultSpawnRates = new Dictionary<int, float[]>();

	//public bool spawnImmediate;
	//public Vector3 immediateSpawnPos;
	//public Vector2 targetAspectRatio;
	public bool isBackground;

	float spawnTimer;
	//float targetAspect;
	//float yVectorAsteroid;
	PoolObject[] poolObjects = null;

	GameManager game;

	void Awake() {
		Configure ();
	}

	void Start(){
		game = GameManager.instance;
		GameManager.OnGameOverConfirmed += this.OnGameOverConfirmed;
		ConstSpawnPositionCoins();
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
		if (spawnTimer > defaultSpawnRates[numberSpawnProgram][curNumberSpawnPosition] * 9 / shiftSpeed) {
			Spawn ();
			spawnTimer = 0;
			//print (GameManager.gameNew + " : " + GameManager.gamePause + " : " + GameManager.gameOver);
		}
	}

	void Configure() {
		spawnTimer = 0;        
		curNumberSpawnPosition = 0;		

		//targetAspect = targetAspectRatio.x / targetAspectRatio.y;

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
/*
		if (spawnImmediate) {
			SpawnImmediate();
		}						
*/
	}

	void Spawn (){
		//print ("Spawn Prog = "+numberSpawnProgram+"; Spawn Pos = "+curNumberSpawnPosition);
		//print ("defaultSpawnYPositions[Spawn Prog][Spawn Pos].Length = "+defaultSpawnYPositions[numberSpawnProgram][curNumberSpawnPosition].Length);
		for (int i = 0; i < defaultSpawnYPositions[numberSpawnProgram][curNumberSpawnPosition].Length; i++)	{		
			Transform t = GetPoolObject ();
			if (t == null) return;
			Vector3 pos = Vector3.zero;		
			//pos.x = defaultSpawnPos.x;
			pos.x = defaultSpawnXPosition;
			pos.y = defaultSpawnYPositions[numberSpawnProgram][curNumberSpawnPosition][i];			
			//pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
			t.position = pos;
		}
        curNumberSpawnPosition = CircleChangeI(defaultSpawnYPositions[numberSpawnProgram].Length, curNumberSpawnPosition, '+');
        //print("Number = "+curNumberSpawnPosition);
	}

/*
	void SpawnImmediate (){
		Transform t = GetPoolObject ();
		if (t == null) return;
		Vector3 pos = Vector3.zero;
		pos.x = immediateSpawnPos.x;
		pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
		t.position = pos;
		Spawn ();
	}
*/
	void Shift (){
		for (int i = 0; i < poolObjects.Length; i++) {
			//print("Pos[ "+i+" ] = "+poolObjects [i].transform.position);
			if (poolObjects [i].inUse) {
				if ((poolObjects [i].transform.position.z <= 89.9f) || (poolObjects [i].transform.position.z >= 90.1f)) {
					//print ("Pool pos: Shifted!");
					//poolObjects [i].transform.localPosition += shiftVector * (shiftSpeed - (shiftSpeed * RocketManager.speed/10))  * Time.deltaTime;		
					poolObjects [i].transform.localPosition += shiftVector * shiftSpeed * Time.deltaTime;		
				}
				CheckDisposeObject (poolObjects [i]);
			}			
		}
			
	}

	void CheckDisposeObject(PoolObject poolObject) {
		if ((poolObject.transform.position.x < -defaultSpawnXPosition) || (poolObject.transform.position.x > 800)) {
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

    int CircleChangeI(int n, int i, char action){
        //print("i = "+i+"; n = "+n);
        switch (action)
        {
            case '+':
                if (i == (n-1)){                                        
                    return 0;
                } else {
                    return ++i;
                }
                break;

            case '-':
                if (i == 0){
                    return (n-1);
                } else {
                    return --i;
                }
                break;                                
            default: 
                return i;
                break;
        }
    }

	private void ConstSpawnPositionCoins(){
		// -3f							-	-	-
		//  0f				-	-	-
		//  3f	-	-	-
		defaultSpawnYPositions.Add(0, new float[][] {new float[] {-3f}, new float[] {-3f}, new float[] {-3f}, new float[] {0f}, new float[] {0f}, new float[] {0f}, new float[] {3f}, new float[] {3f}, new float[] {3f}});
		defaultSpawnRates.Add(0, new float[] {0.25f, 0.25f, 0.25f, 0.5f, 0.25f, 0.25f, 0.5f, 0.25f, 0.25f});

		//	3f					-
		//	2f				-		-
		//	1f			-				-
		//	0f		-						-						
		//	-1f									-				-
		//	-2f										-		-
		//	-3f											-
		defaultSpawnYPositions.Add(1, new float[][] {new float[] {0f}, new float[] {1f}, new float[] {2f}, new float[] {3f}, new float[] {2f}, new float[] {1f}, new float[] {0f}, new float[] {-1f}, new float[] {-2f}, new float[] {-3f}, new float[] {-2f}, new float[] {-1f}});
		defaultSpawnRates.Add(1, new float[] {0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f});

		//	3f						-			
		//	1.5f				-	
		//	0f				-	
		//	-1.5f		-	
		//	-3f		-							
		defaultSpawnYPositions.Add(2, new float[][] {new float[] {-3f}, new float[] {-1.5f}, new float[] {0f}, new float[] {1.5f}, new float[] {3f}} );
		defaultSpawnRates.Add(2, new float[] {0.25f, 0.25f, 0.25f, 0.25f, 0.25f});

		//	3f		-	-				-			-			-		-			-			-	-
		//	1.8f	-		-		-		-		-			-		-			-		-			-
		//	0.6f	-	-			-		-		-	-		-		-			-			-
		//	-0.6f	-		-		-		-		-		-	-		-			-				-	
		//	-1.8f	-		-		-		-		-			-		-			-		-			-
		//	-3f		-	-				-			-			-			-	-				-	-
		//			0	1	2	3	4	5	6	7	8	9	10	11	12	13	14	15	16	17	18	19	20	21		
		defaultSpawnYPositions.Add(3, new float[][] {	
														new float[] {-3f, -1.8f, -0.6f, 0.6f, 1.8f, 3f}, new float[] {-3f, 0.6f, 3f}, new float[] {-1.8f, -0.6f, 1.8f}, new float[] {}, 
														new float[] {-1.8f, -0.6f, 0.6f, 1.8f}, new float[] {-3f, 3f}, new float[] {-1.8f, -0.6f, 0.6f, 1.8f}, new float[] {},
														new float[] {-3f, -1.8f, -0.6f, 0.6f, 1.8f, 3f}, new float[] {0.6f}, new float[] {-0.6f}, new float[] {-3f, -1.8f, -0.6f, 0.6f, 1.8f, 3f}, new float[] {},
														new float[] {-1.8f, -0.6f, 0.6f, 1.8f, 3f}, new float[] {-3f}, new float[] {-3f}, new float[] {-1.8f, -0.6f, 0.6f, 1.8f, 3f}, new float[] {},
														new float[] {-1.8f, 1.8f}, new float[] {-3f, 0.6f, 3f}, new float[] {-3f, -0.6f, 3f}, new float[] {-1.8f, 1.8f}, new float[] {}
													});
		defaultSpawnRates.Add(3, new float[] {
												0.15f, 0.15f, 0.15f, 0.25f, 
												0.15f, 0.15f, 0.15f, 0.25f,
												0.15f, 0.15f, 0.15f, 0.15f, 0.25f,
												0.15f, 0.15f, 0.15f, 0.15f, 0.25f,
												0.15f, 0.15f, 0.15f, 0.15f, 1.0f
											});

		//	3f						-							-	-	-			-	-	-	-	-		-				-
		//	1.5f				-		-					-				-		-				-			-		-
		//	0f				-				-				-				-		-				-				-	
		//	-1.5f		-						-			-				-		-				-			-		-
		//	-3f		-		-		-		-		-			-	-	-			-	-	-	-	-		-				-
		//			0	1	2	3	4	5	6	7	8	9	10	11	12	13	14	15	16	17	18	19	20	21	22	23	24	25	26		
		
		defaultSpawnYPositions.Add(4, new float[][] {	
														new float[] {-3f}, new float[] {-1.5f}, new float[] {-3f, 0f}, new float[] {1.5f}, new float[] {-3f, 3f}, new float[] {1.5f}, new float[] {-3f, 0f}, new float[] {-1.5f}, new float[] {-3f}, new float[] {}, 
														new float[] {-1.5f, 0f, 1.5f}, new float[] {-3f, 3f}, new float[] {-3f, 3f}, new float[] {-3f, 3f}, new float[] {-1.5f, 0f, 1.5f}, new float[] {},
														new float[] {-3f, -1.5f, 0f, 1.5f, 3f}, new float[] {-3f, 3f}, new float[] {-3f, 3f}, new float[] {-3f, 3f}, new float[] {-3f, -1.5f, 0f, 1.5f, 3f}, new float[] {},
														new float[] {-3f, 3f}, new float[] {-1.5f, 1.5f}, new float[] {0f}, new float[] {-1.5f, 1.5f}, new float[] {-3f, 3f}, new float[] {}
													});
		defaultSpawnRates.Add(4, new float[] {
												0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.25f, 
												0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.5f,
												0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.5f,
												0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 2.5f
											});

		//	3.0f						-								-
		//	2.8f					-		-						-		-
		//	2.4f				-				-				-				-
		//	1.6f			-						-		-						-
		//	0.0f		-								-								-
		//	-1.6f			-						-										-						-
		//	-2.4f				-				-												-				-
		//	-2.8f					-		-														-		-
		//	-3.0f						-																-
		//				0	1	2	3	4	5	6	7	8	9	10	11	12	13	14	15	16	17	18	19	20	21	22	23
		defaultSpawnYPositions.Add(5, new float[][] {	
														new float[] {0f}, new float[] {-1.6f, 1.6f}, new float[] {-2.4f, 2.4f}, new float[] {-2.8f, 2.8f}, new float[] {-3f, 3f}, new float[] {-2.8f, 2.8f},  new float[] {-2.4f, 2.4f},  new float[] {-1.6f, 1.6f}, new float[] {0f}, new float[] {1.6f}, new float[] {2.4f}, new float[] {2.8f}, new float[] {3f}, new float[] {2.8f},  new float[] {2.4f}, new float[] {1.6f}, new float[] {0f}, new float[] {-1.6f}, new float[] {-2.4f}, new float[] {-2.8f}, new float[] {-3f}, new float[] {-2.8f}, new float[] {-2.4f}, new float[] {-1.6f}
													});
		defaultSpawnRates.Add(5, new float[] {
												0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f
											});


		numberSpawnProgram = Random.Range(0, defaultSpawnRates.Count-1);
		//numberSpawnProgram = 4; // test row
		//print("=== >>> Spawn Program = "+numberSpawnProgram);

	}
}
