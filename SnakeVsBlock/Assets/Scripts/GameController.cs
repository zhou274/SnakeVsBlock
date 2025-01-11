using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;

public class GameController : MonoBehaviour {

    public enum GameState { MENU, GAME, GAMEOVER }
    public static GameState gameState;

    [Header("Managers")]
    public SnakeMovement SM;
    public BlocksManager BM;

    [Header("Canvas Groups")]
    public CanvasGroup MENU_CG;
    public CanvasGroup GAME_CG;
    public CanvasGroup GAMEOVER_CG;

    [Header("Score Management")]
    public Text ScoreText;
    public Text MenuScoreText;
    public Text BestScoreText;
    public TextMeshProUGUI GameOverScore;
    public TextMeshProUGUI GameOverBestScore;
    
    public static int SCORE;
    public static int BESTSCORE;

    [Header("Some Bool")]
    bool speedAdded;


    public string clickid;
    private StarkAdManager starkAdManager;
    // Use this for initialization
    void Start () {

        //Initially, set the menu and Score is null
        SetMenu();
        SCORE = 0;

        //Initialize some booleans
        speedAdded = false;

        //Load the best score
        BESTSCORE = PlayerPrefs.GetInt("BESTSCORE");

	}
	
	// Update is called once per frame
	void Update () {

        //Update the score text
        ScoreText.text = SCORE + "";
        MenuScoreText.text = SCORE + "";
        GameOverScore.text = SCORE + "";

        //Update the Best Score and the text
        if (SCORE > BESTSCORE)
            BESTSCORE = SCORE;

        BestScoreText.text = BESTSCORE + "";
        GameOverBestScore.text = BESTSCORE + "";

        if (!speedAdded && SCORE > 150)
        {
            SM.speed++;
            speedAdded = true;
        }

    }

    public void SetMenu()
    {
        //Set the GameState
        gameState = GameState.MENU;

        //Manage Canvas Groups
        EnableCG(MENU_CG);
        DisableCG(GAME_CG);
        DisableCG(GAMEOVER_CG);
    }

    public void SetGame()
    {
        //Set the GameState
        gameState = GameState.GAME;

        //Manage Canvas Groups
        EnableCG(GAME_CG);
        DisableCG(MENU_CG);
        DisableCG(GAMEOVER_CG);

        //Reset score
        SCORE = 0;
    }
    public void Restart()
    {
        ShowVideoAd("122i08nk721j8ln94j",
            (bol) => {
                if (bol)
                {

                    //Set the GameState
                    gameState = GameState.GAME;

                    //Manage Canvas Groups
                    EnableCG(GAME_CG);
                    DisableCG(MENU_CG);
                    DisableCG(GAMEOVER_CG);


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
    }

    public void SetGameover()
    {
        //Set the GameState
        gameState = GameState.GAMEOVER;

        //Manage Canvas Groups
        EnableCG(GAMEOVER_CG);
        DisableCG(GAME_CG);
        DisableCG(MENU_CG);

        //Delete all the objects
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Box"))
        {
            Destroy(g);
        }

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Snake"))
        {
            Destroy(g);
        }

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("SimpleBox"))
        {
            Destroy(g);
        }

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Bar"))
        {
            Destroy(g);
        }


        //Spawn the new body parts
        SM.SpawnBodyParts();

        //Reset the previous snake pos to spawn barrier correctly
        BM.SetPreviousSnakePosAfterGameover();

        //Reset the Speed
        speedAdded = false;
        SM.speed = 3;

        //Save the Best Score
        PlayerPrefs.SetInt("BESTSCORE", BESTSCORE);

        //Reset the Simple Blocks List
        BM.SimpleBoxPositions.Clear();

        //Increase AdMob Counter
        AdManager.gameoverCounter++;

        ShowInterstitialAd("1bm5oajsase99d06ff",
            () => {

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
    }

    public void EnableCG(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.blocksRaycasts = true;
        cg.interactable = true;
    }

    public void DisableCG(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }

    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
}
