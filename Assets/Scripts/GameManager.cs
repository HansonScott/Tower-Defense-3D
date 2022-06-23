using System;
using UnityEngine;

public enum GameState
{
    SessionMenu,
    SessionStart,
    WaveStart,
    WaveActive,
    WavePause,
    WaveFailed,
    SessionEnd,
}

public class GameManager : MonoBehaviour
{
    public static GameManager CurrentGame;

    public GameState CurrentState = GameState.SessionMenu;

    private int _CurrentScore;
    public int CurrentScore
    {
        get { return _CurrentScore; }
        set { _CurrentScore = value; UIManager.CurrentUIManager.RefreshScoreLabel(_CurrentScore); }
    }

    public int StartingMoney = 200;

    private int _CurrentMoney = 0;
    public int CurrentMoney
    { 
        get { return _CurrentMoney; }
        set { _CurrentMoney = value; UIManager.CurrentUIManager.RefreshMoneyLabel(_CurrentMoney); }
    }

    public EnemyObject EnemyTemplate;
    private Color enemyTemplateForThisWave;

    public float TimeSinceNewWave = 0;
    public float WaveDelay = 10;

    public float TimeSinceLastSpawn = 0;
    public float SpawnDelay = 1.5f;
    public int EnemyCountInWave = 10;
    private int RemainingEnemies = 10;

    public int CurrentWave = 1;

    // Start is called before the first frame update
    void Start()
    {
        CurrentGame = this;
    }

    // Update is called once per frame(?)
    void Update()
    {
        switch(CurrentState)
        {
            case GameState.SessionMenu:
                HandleSessionMenu();
                break;
            case GameState.SessionStart:
                HandleSessionStart();
                break;
            case GameState.WaveStart:
                HandleWaveStart();
                UIManager.CurrentUIManager.RefreshWaveLabel(CurrentWave);
                UIManager.CurrentUIManager.btnWave.enabled = false;
                break;
            case GameState.WaveActive:
                HandleWaveActive();
                break;
            case GameState.WavePause:
                HandleWavePause();
                break;
            case GameState.WaveFailed:
                HandleWaveFailed();
                break;
            case GameState.SessionEnd:
                HandleSessionEnd();
                break;
        }

    }

    private void HandleSessionMenu()
    {
        // wait until buttons are pressed to start the session, skip for now
        CurrentState = GameState.SessionStart;
    }

    private void HandleSessionStart()
    {
        // starting the level, reset any numbers, then start the wave
        RefreshHomeHP();
        CurrentMoney = StartingMoney;
        
        // moved this to be button-based
        //CurrentState = GameState.WaveStart;
    }

    private void HandleWaveStart()
    {
        if(TimeSinceNewWave > WaveDelay)
        {
            int powerLevel = ++CurrentWave * 100;

            // start the wave variables
            enemyTemplateForThisWave = EnemyObject.GetRandomEnemyProperties(powerLevel);

            RemainingEnemies = EnemyCountInWave;

            // then set the wave to active
            CurrentState = GameState.WaveActive;
            TimeSinceLastSpawn = SpawnDelay; // set initial enemy to spawn immediately
        }
        else
        {
            TimeSinceNewWave += Time.deltaTime;
        }
    }

    private void HandleWaveActive()
    {
        // check for enemy spawn count, etc.

        // check for spawn delay
        if (TimeSinceLastSpawn >= SpawnDelay && RemainingEnemies > 0)
        {
            EnemyObject e = EnvironmentManager.CurrentEnvironment.PlaceNewEnemy(EnemyTemplate);

            // do anything to this particular one?
            e.gameObject.GetComponent<MeshRenderer>().material.color = enemyTemplateForThisWave;

            e.ApplyPropertiesFromColor(enemyTemplateForThisWave);
            e.transform.position = EnvironmentManager.CurrentSpawnPoint;

            // reset for next enemy
            TimeSinceLastSpawn = 0;
            RemainingEnemies--;
        }
        else
        {
            TimeSinceLastSpawn += Time.deltaTime;
        }

        // check for all enemies spawned, then pause wave
        if(RemainingEnemies == 0)
        {
            CurrentState = GameState.WaveStart;
            TimeSinceNewWave = 0;

            // add more money for each wave cleared
            CurrentMoney += CurrentWave * 25;

            //UIManager.CurrentUIManager.btnWave.enabled = true;
        }
    }

    private void HandleWavePause()
    {
        // check wave complete, or set a timer before next wave
    }

    private void HandleSessionEnd()
    {
        // set final score, return to menu
        CurrentState = GameState.SessionMenu;
    }

    public void EnemyHitHome(float dmg, Vector3 homePosition)
    {
        // find which home object
        GameObject h = EnvironmentManager.CurrentEnvironment.GetHomeObjectAt(homePosition);

        // reduce HP of home
        h.GetComponent<HomeScript>().HPCurrent -= dmg;

        // update the label
        RefreshHomeHP();

        // check for failed wave
        if(h.GetComponent<HomeScript>().HPCurrent <= 0)
        {
            CurrentState = GameState.WaveFailed;
        }
    }

    private void RefreshHomeHP()
    {
        UIManager.CurrentUIManager.txtHomeHP.text = "HP: " + EnvironmentManager.CurrentEnvironment.CurrentHome.GetComponent<HomeScript>().HPCurrent;
    }

    public void StartWaveClick()
    {
        UIManager.CurrentUIManager.btnWave.gameObject.SetActive(false);

        if(CurrentState == GameState.SessionStart)
        {
            CurrentState = GameState.WaveStart;
            TimeSinceNewWave = WaveDelay; // start first wave immeditely
        }

        // restart the next wave if we're active...(?)
        if(CurrentState == GameState.WaveActive)
        {
            CurrentState = GameState.WaveStart;
        }
    }
    private void HandleWaveFailed()
    {
        print("wave failed!");
    }
}
