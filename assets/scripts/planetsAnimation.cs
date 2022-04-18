using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planetsAnimation : MonoBehaviour
{

	void OnStart () {        	
        DisablePlanets();
	}

	void OnEnable () {
		StartCoroutine("PlanetSpawner");
	}

    void OnDisable () {
        DisablePlanets();
    }

    void DisablePlanets (){
	    for (int i = 0; i < gameObject.transform.childCount; i++) {
			gameObject.transform.GetChild (i).gameObject.SetActive(false);            
		}		     
    }

	IEnumerator PlanetSpawner(){
		
	    for (int i = 0; i < gameObject.transform.childCount; i++) {
			gameObject.transform.GetChild (i).gameObject.SetActive(true);            
			yield return new WaitForSeconds (0.25f);
		}		
	}

}
