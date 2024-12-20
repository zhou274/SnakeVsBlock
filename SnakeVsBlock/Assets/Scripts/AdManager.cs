using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour {

	public static AdManager Instance{set;get;}
	public string InterstitialId;
	public string BannerId;

	private BannerView bannerView;
	private InterstitialAd interstitial;

    public int gameoverToShowAd;
    public static int gameoverCounter;

	// Use this for initialization
	private void Start () {

		Instance = this;
		DontDestroyOnLoad (gameObject);

		bannerView = new BannerView (BannerId, AdSize.Banner, AdPosition.Bottom);
		interstitial = new InterstitialAd (InterstitialId);

        ShowBanner();

        LoadVideo();
	}

    private void Update()
    {
        //PlayerPrefs.DeleteAll();

        if(gameoverCounter == gameoverToShowAd)
        {
            ShowVideo();
            gameoverCounter = 0;

        }
    }

    public void LoadVideo() {
		AdRequest request = new AdRequest.Builder ().Build ();
		interstitial.LoadAd (request);

	}

	public void ShowVideo()
	{
        if (interstitial.IsLoaded())
            interstitial.Show();
        else
        {
            AdRequest request = new AdRequest.Builder().Build();
            interstitial.LoadAd(request);
        }
	}

	public void ShowBanner()
	{
		AdRequest request = new AdRequest.Builder ().Build ();
		bannerView.LoadAd (request);

		bannerView.Show ();
	}

	public void RemoveBanner()
	{
		bannerView.Hide ();
	}

}
