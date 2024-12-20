using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        //Set the GameState
        gameState = GameState.GAME;

        //Manage Canvas Groups
        EnableCG(GAME_CG);
        DisableCG(MENU_CG);
        DisableCG(GAMEOVER_CG);
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
}
