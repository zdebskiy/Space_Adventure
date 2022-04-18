using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabGenerator : MonoBehaviour {
	
	public GameObject Prefab;

	// Use this for initialization
	void Start () {

		GameObject go = Instantiate (Prefab) as GameObject;
		Transform t = go.transform;
		t.SetParent (transform);
		t.position = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
/*		if (game.GameOver()) return;

		//Shift ();

		spawnTimer += Time.deltaTime;
		if (spawnTimer > spawnRate) {
			Spawn ();
			spawnTimer = 0;
		}*/
	}

/*	void Spawn (){
		Transform t = GetPoolObject ();
		if (t == null) return;
		Vector3 pos = Vector3.zero;
		pos.x = defaultSpawnPos.x;
		pos.y = Random.Range(ySpawnRange.min, ySpawnRange.min);
		t.position = pos;

	}*/
}
