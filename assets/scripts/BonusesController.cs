using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusesController : MonoBehaviour
{
    class BonusData
    {
        public string Name { get; set; }
        public int Chance { get; set; }
    }

    public GameObject[] bonusesPrefabs;

    private BonusData[] bonusesDropChance = new BonusData[8];

    private void Start() {        
        DefaultChances();
    }    

    private void OnEnable() {
        AsteroidController.OnAsteroidCrash += OnAsteroidCrash;
        CrosAsteroidController.OnAsteroidCrash += OnAsteroidCrash;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable() {
        AsteroidController.OnAsteroidCrash -= OnAsteroidCrash;
        CrosAsteroidController.OnAsteroidCrash -= OnAsteroidCrash;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    private void OnAsteroidCrash(Transform tAsteroid, float shiftSpeed){
        CorrectChances();
        //ShowChances();
		GameObject go = Instantiate (bonusesPrefabs[GetBonusNumber()]) as GameObject;
        //GameObject go = Instantiate (bonusesPrefabs[7]) as GameObject;
		Transform t = go.transform;
		t.SetParent (transform);
        t.position = tAsteroid.position;
        t.GetComponent<BonusController>().shiftSpeed = shiftSpeed;        
    }

    private void DefaultChances(){
        bonusesDropChance[0] = new BonusData{Name="Coin",       Chance=81 };
        bonusesDropChance[1] = new BonusData{Name="Diamond",    Chance=2};
        bonusesDropChance[2] = new BonusData{Name="HealPotion", Chance=3};
        bonusesDropChance[3] = new BonusData{Name="Heart",      Chance=4};
        bonusesDropChance[4] = new BonusData{Name="Gun_00",     Chance=4};
        bonusesDropChance[5] = new BonusData{Name="Gun_01",     Chance=3};
        bonusesDropChance[6] = new BonusData{Name="Gun_02",     Chance=2};
        bonusesDropChance[7] = new BonusData{Name="Gun_03",     Chance=2};
    }

    private void CorrectChances(){
        int buf = 0;

        DefaultChances();

        if (GameManager.curHealth < GameManager.fullHealth) {
            // Add chance Heart
            buf = bonusesDropChance[3].Chance;
            bonusesDropChance[3].Chance += buf;
            bonusesDropChance[0].Chance -= buf;
        }

        if (GameManager.curHealth <= GameManager.fullHealth/2) {
            // Add chance HealPotion
            buf = bonusesDropChance[2].Chance;
            bonusesDropChance[2].Chance += buf;
            bonusesDropChance[0].Chance -= buf;            
        }
        
        switch (GameManager.curGun) {
            // Add chance drop another gun
            case 0:
                buf = bonusesDropChance[4].Chance/3;
                bonusesDropChance[4].Chance = 0;
                bonusesDropChance[5].Chance += buf;
                bonusesDropChance[6].Chance += buf;
                bonusesDropChance[7].Chance += buf;                
                break;
            case 1:
                buf = bonusesDropChance[5].Chance/3;
                bonusesDropChance[5].Chance = 0;
                bonusesDropChance[4].Chance += buf;
                bonusesDropChance[6].Chance += buf;            
                bonusesDropChance[7].Chance += buf;                
                break;
            case 2:
                buf = bonusesDropChance[6].Chance/3;
                bonusesDropChance[6].Chance = 0;
                bonusesDropChance[4].Chance += buf;
                bonusesDropChance[5].Chance += buf;            
                bonusesDropChance[7].Chance += buf;                
                break;
            case 3:
                buf = bonusesDropChance[7].Chance/3;
                bonusesDropChance[7].Chance = 0;
                bonusesDropChance[4].Chance += buf;
                bonusesDropChance[5].Chance += buf;
                bonusesDropChance[6].Chance += buf;                
                break;                
        }

        // Balance Bonuses Cances to 100
        int summ=0;
        for (int i = 1; i < bonusesDropChance.Length; i++) {
            summ += bonusesDropChance[i].Chance;
        }
        bonusesDropChance[0].Chance = 100-summ;

    }
    
    private void ShowChances(){
        for (int i = 0; i < bonusesDropChance.Length; i++){        
            print (bonusesDropChance[i].Name+" = "+bonusesDropChance[i].Chance);
        }
    }

    private int GetBonusNumber(){
        int returnNumber = 0;        
        int randomNum = Random.Range(0, 100);        
        int shiftChance = 0;
        for (int i = 0; i < bonusesDropChance.Length; i++){
            //print ("randomNum = "+randomNum+" <= ( "+bonusesDropChance[i].Chance+" + "+shiftChance+" )");
            if (randomNum <= bonusesDropChance[i].Chance+shiftChance){
                returnNumber = i;
                break;
            } else {
                shiftChance+=bonusesDropChance[i].Chance;
            }
        }
        return returnNumber;
    }

    private void OnGameOverConfirmed(){
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

}
