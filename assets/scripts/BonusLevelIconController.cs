using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusLevelIconController : MonoBehaviour
{
    public Transform tPlanetIcons;

    private void OnEnable() {
        InitPlanetIcons ();
    }
	void InitPlanetIcons() {
		for (int j = 0; j < tPlanetIcons.childCount; j++) {
			//print (tPlanetIcons.GetChild (j).name);
			if (j == LevelManager.curLevel) {
				tPlanetIcons.GetChild (j).gameObject.SetActive (true);
			} else {
				tPlanetIcons.GetChild (j).gameObject.SetActive (false);
			}
		}		
	}    
}
