using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    /// <summary>
    /// Time to wait before starting level, seconds
    /// </summary>
    public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    public static GameManager Instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100; // todo replace.
    [HideInInspector] public bool playersTurn = true;
    public static Logger log;

    private Text LevelText;
    private GameObject LevelImage;
    private int level = 3; //for testing only
    private List<Creature> enemies;
    private bool enemiesMoving = false;
    private bool doingSetup;

    private void GetInstance()
    {
        Instance = AssureSingletonAndDestroyExtras<GameManager>(Instance, this);
    }

    public static T AssureSingletonAndDestroyExtras<T>(T instance, T obj) where T : MonoBehaviour
    {
        if (instance == null)
        {
            instance = obj;
        }
        else if (!instance.Equals(obj))
        {
            Destroy(obj.gameObject);
            Debug.LogWarning("Destroyed nth instance of game Object.");
        }
        return instance;
    }

    private void Start()
    {
        GetInstance();
        DontDestroyOnLoad(gameObject);
        enemies = new List<Creature>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }


    private void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }
    private void InitGame()
    {
        doingSetup = true;
        try { 
        LevelImage = GameObject.Find("LevelImage");
        LevelText = GameObject.Find("LevelText").GetComponent<Text>();
        LevelText.text = "Day " + level;
        }
        catch
        {
            LevelImage = new GameObject();    
        }
        ShowLevelImage(true);
        
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        ShowLevelImage(false);
    }
    private void ShowLevelImage(bool turnOn = false)
    {
        LevelImage.SetActive(turnOn);
        doingSetup = turnOn;   
        
        if(turnOn)
        {
            // Todo this is brittle - fix it.
            Invoke("HideLevelImage", levelStartDelay);
        }
    }

    // Use this for initialization
    public void GameOver () {
        LevelText.text = "You survived " + level + " days!";
        ShowLevelImage(true);
        enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (playersTurn || enemiesMoving || doingSetup)
        { return; }

        StartCoroutine(MoveEnemies());
	}

    /// <summary>
    /// Enemies need to register with Game manager so that it can issue orders to them.
    /// </summary>
    public void AddEnemyToList(Creature script)
    {
        enemies.Add(script);
    }

    private IEnumerator MoveEnemies()
    {
        enemiesMoving = true; 
        yield return new WaitForSeconds(turnDelay);
        if(!enemies.Any())
        {
            yield return new WaitForSeconds(turnDelay);
        }

        foreach(var enemy in enemies)
        {
            enemy.MoveEnemy();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
