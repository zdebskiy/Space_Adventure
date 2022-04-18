using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class ShopController : MonoBehaviour
{

    public GameObject zastawkaPage;
    public Text tCompleted;
    public Text tWait;

	//AdMob
    private RewardedAd videoAd;  // new
    bool videoAdLoaded = false;

    private string appID;
    private string videoAdID;
    int viewCountTask;    
    int viewCounter;    
    int closeCounter;

	void Start() {        
		AdMobInit(false);
        VideoAdInitAndRequest();
	}

	void OnDisable () {
        viewCounter = 0;
        closeCounter = 0;
        zastawkaPage.SetActive(false);
	}

    void OnEnable() {
        
    }

    public void BuyDiamonds(int count) {
        viewCountTask = count;
        viewCounter = 0;
        closeCounter = 0;
        ShowVideoAd();
    }

	void AdMobInit(bool testMod){		
        appID = "ca-app-pub-1339618403883604~1261030067";  // my ID		         
                        
		if (testMod){
			Debug.Log("Test mod!!!");
        	//interstitialAdID = "ca-app-pub-3940256099942544/1033173712"; // free AD ID		                            
        	videoAdID = "ca-app-pub-3940256099942544/5224354917"; // free AD reward ID
		} else {
			Debug.Log ("Buy mod!!!");
        	//interstitialAdID = "ca-app-pub-1339618403883604/2828234933"; // my AD ID		                            
        	videoAdID = "ca-app-pub-1339618403883604/4737364355"; // my AD reward ID
		}

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
	}

	AdRequest AdRequestBuild(){
        return new AdRequest.Builder().Build();
    }

	// { video AD 
	void VideoAdInitAndRequest (){

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
        closeCounter++;
        //print (viewCounter + " == " + viewCountTask);
        //tText.text = viewCounter.ToString();
        if ((viewCounter == viewCountTask) || (closeCounter == viewCountTask+1)) {
            if (viewCounter == viewCountTask)
            {
                switch (viewCountTask)
                {
                    case 1:
                        // give 1 package
                        AddDiamond(20);
                        break;
                    case 2:
                        // give 2 package
                        AddDiamond(50);
                        break;
                    case 3:
                        // give 3 package
                        AddDiamond(100);
                        break;
                    
                }
            } else {
                // Test Diamonds Add
                // AddDiamond(51);
            }
            viewCounter = 0;
            closeCounter = 0;
            VideoAdDestroy();
            VideoAdInitAndRequest();
        } else {            
            zastawkaPage.SetActive(true);
            tCompleted.text = "COMPLETED \n"+ viewCounter + " of " + viewCountTask;
		    VideoAdDestroy();
    		VideoAdInitAndRequest();
            StartCoroutine("NextAd");
    		//ShowVideoAd();
        }
    }

    IEnumerator NextAd(){
		int count=3;
		for (int i = 0; i < count; i++) {
            tWait.text = "NEXT VIDEO LEFT...  " + (count-i) + " sec";
			yield return new WaitForSeconds (1);
		}
        zastawkaPage.SetActive(false);
		ShowVideoAd();		
	}

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);        
		viewCounter++;
    }
	// ////////

    public void ShowVideoAd(){
        if(videoAd.IsLoaded()){
            videoAd.Show();
        }
    }


	// } video AD    

    public void AddDiamond(int n){
        GameManager.savedScore += n;
        PlayerPrefs.SetInt ("SaveScore", GameManager.savedScore);
        PlayerPrefs.Save(); 
    } 
}
