using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class diamindController : MonoBehaviour {

	public GameObject obj;

	void OnEnable () {		
		rocketController.OnRocketScored += OnRocketScored;
	}

	void OnDisable (){		
		rocketController.OnRocketScored -= OnRocketScored;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnRocketScored (){
		obj.SetActive(false);
		obj.transform.position = Vector3.one * 1000;
	}

}
