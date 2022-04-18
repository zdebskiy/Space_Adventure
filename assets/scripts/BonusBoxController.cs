using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BonusBoxController : MonoBehaviour
{    

    public GameObject goMsgBody;
    public GameObject goBonusBox;
    public GameObject goBonuses;
    public GameObject goPriceText;

    public GameObject[] prefabBonuses;

    public GameObject goBtnOpen;
    public GameObject goBtnAdOpen;
    public GameObject goBtnOK;

    public AudioSource soundBox;
    public AudioClip[] soundBoxWindowClips;
    public AudioClip[] soundBoxOpenClips;
    public AudioClip[] soundBoxErrorClips;

    public GameManager gameManager;

    private Animation animWindowsBody;
    private Animation animBonusBox;
    private Animation animBonuses;
    private Animation animBoxBonus;
    private Animation animTopBox;
    private Animation animBottomBox;

    private Transform tBgBonuses;
    private Transform tBoxBonuses;
    private int PRICE_OPEN = 1;
    private int tryOpenCounter=0;

    private void Awake() {
        animWindowsBody = goMsgBody.GetComponent <Animation> ();
        tBgBonuses = goBonuses.transform.GetChild(0);
        tBoxBonuses = goBonuses.transform.GetChild(1);
    }

    void Start()    
    {
        InitBonusBoxMsg();
        animBoxBonus = gameObject.GetComponent <Animation>();

        animTopBox = goBonusBox.transform.GetChild(0).GetComponent<Animation>();
        animBottomBox = goBonusBox.transform.GetChild(1).GetComponent<Animation>();
    }

    private void OnEnable() {

        GameManager.OnWtchedRewardedVideo += OnWtchedRewardedVideo;

        InitBonusBoxMsg();
        animWindowsBody.Play("msg_body_enabled");
        // sound effect
        soundBox.clip = soundBoxWindowClips[UnityEngine.Random.Range(0, soundBoxWindowClips.Length)];
        soundBox.Play();        
    }

    private void OnDisable() {
        GameManager.OnWtchedRewardedVideo -= OnWtchedRewardedVideo;
    }

    private void InitBonusBoxMsg(){
        goMsgBody.transform.localScale = Vector3.zero;
        tryOpenCounter = 0;

        goBtnOpen.SetActive(true);
        goBtnAdOpen.SetActive(true);
        goBtnOK.SetActive(false);       
        goBonuses.SetActive(false);
        tBoxBonuses.localPosition = Vector3.zero;

        goBonusBox.transform.GetChild(0).localPosition = new Vector3(0, -28.5f, 0);
        goBonusBox.transform.GetChild(1).localPosition = Vector3.zero;

       for (int i = 0; i < tBoxBonuses.childCount; i++) {
           Destroy(tBoxBonuses.GetChild(i).gameObject);
       }

        if ((GameManager.savedNumSession == 1) && (LevelManager.curLevel == 0)){
            PRICE_OPEN = 0;
        } else {
            PRICE_OPEN = 1;
            //PRICE_OPEN = 0; //(test row)
        }

        goPriceText.GetComponent<Text>().text = PRICE_OPEN.ToString();       

    }

    public void OpenBonusBox(bool watchAD){
        Dictionary <string, int> dictionaryBonuses = new Dictionary<string, int>();                
        GameObject go;
        Transform t;

        //if (GameManager.savedDiamond >= PRICE_OPEN){
        if ((watchAD) || (GameManager.savedDiamond >= PRICE_OPEN)){

            // sound effect
			soundBox.clip = soundBoxOpenClips[UnityEngine.Random.Range(0, soundBoxOpenClips.Length)];
			soundBox.Play();

            if (!watchAD) {
                GameManager.savedDiamond -= PRICE_OPEN;
                PlayerPrefs.SetInt ("SaveDiamond", GameManager.savedDiamond);
                PlayerPrefs.Save();
            }

            goBtnOpen.SetActive(false);
            goBtnAdOpen.SetActive(false);
            goBtnOK.SetActive(true);
            goBonuses.SetActive(true);
            animTopBox.Play("open_top_box");
            animBottomBox.Play("open_bottom_box");

            foreach (var itemBonus in prefabBonuses) {            
                switch (itemBonus.transform.name){
                    case "Coins":
                        dictionaryBonuses.Add("Coins", BonusCoinsGenerator());
                        go = Instantiate (itemBonus) as GameObject;
                        t = go.transform;
                        t.SetParent (tBoxBonuses);
                        t.localPosition = Vector3.zero;
                        t.localScale = Vector3.zero;
                        t.GetChild(1).GetComponent<Text>().text = "+"+dictionaryBonuses["Coins"].ToString();

                        GameManager.savedScore += dictionaryBonuses["Coins"];
                        PlayerPrefs.SetInt ("SaveScore", GameManager.savedScore);
                        PlayerPrefs.Save();                        

                        //tBoxBonuses.GetChild(0);
                        break;
                    case "Diamonds":
                        int diamonds = BonusDiamondsGenerator();
                        if (diamonds > 0){
                            dictionaryBonuses.Add("Diamonds", diamonds);
                            go = Instantiate (itemBonus) as GameObject;
                            t = go.transform;
                            t.SetParent (tBoxBonuses);
                            t.localPosition = new Vector3(130, 0, 0);                        
                            t.GetChild(1).GetComponent<Text>().text = "+"+dictionaryBonuses["Diamonds"].ToString();

                            GameManager.savedDiamond += dictionaryBonuses["Diamonds"];
                            PlayerPrefs.SetInt ("SaveDiamond", GameManager.savedDiamond);
                            PlayerPrefs.Save();
                        }
                        //tBoxBonuses.GetChild(0);
                        break;
                }
            }
            //print("Count = "+dictionaryBonuses.Count);
            // Set bonuses to center position of screen 
            if (dictionaryBonuses.Count % 2 == 0) {
                tBoxBonuses.localPosition = new Vector3 (-65.0f, 0, 0);
            } else {
                tBoxBonuses.localPosition = Vector3.zero;
            }

        } else {
            animBoxBonus.Play("need_more_diamonds");
            // sound effect
			soundBox.clip = soundBoxErrorClips[UnityEngine.Random.Range(0, soundBoxErrorClips.Length)];
			soundBox.Play();
            tryOpenCounter++;
            if (tryOpenCounter == 3) {
                tryOpenCounter = 0;
                gameManager.ShowVideoAd("BonusBox");
            }
        }
    }

    private int BonusCoinsGenerator(){
        int range = UnityEngine.Random.Range(0, 9);
        int randomCoins=5;
        if ((range>=0) && (range<7)) {
            randomCoins=5;
        } else if ((range>=7) && (range<9)) {
            randomCoins=10;
        } else if (range>=9) {
            randomCoins=15;
        } else {
            randomCoins=5;
        }  

        if ((GameManager.savedNumSession == 1) && (LevelManager.curLevel == 0)){
            if (GameManager.savedScore < 50) {
                return (50-GameManager.savedScore);
            } else {
                return randomCoins;
            }            
        } else {
            return randomCoins;
        }
    }

    private int BonusDiamondsGenerator(){
        int range = UnityEngine.Random.Range(0, 9);
        if ((range>=0) && (range<7)) {
            return 0;
        } else if ((range>=7) && (range<9)) {
            return 1;
        } else if (range>=9) {
            return 2;
        } else {return 0;}  
    }

    private void OnWtchedRewardedVideo(){
        Debug.Log("BoxBonusController - OpenBOX!!!");
        OpenBonusBox(true);
    }
}
