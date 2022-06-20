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

    public EnemyObject EnemyTemplate;
    private Color enemyTemplateForThisWave;

    public int TicksSinceLastSpawn = 0;
    public int SpawnDelay = 1000;
    public int EnemyCountInWave = 10;
    private int RemainingEnemies = 10;

    public int CurrentWave = 0;

    // Start is called before the first frame update
    void Start()
    {
        CurrentGame = this;
    }

    // Update is called once per frame
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
                HandleWaveStart(++CurrentWave * 10);
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

        // moved this to be button-based
        //CurrentState = GameState.WaveStart;
    }

    private void HandleWaveStart(int powerLevel)
    {
        // start the wave variables
        enemyTemplateForThisWave = EnemyObject.GetRandomEnemyProperties(powerLevel);

        RemainingEnemies = EnemyCountInWave;

        // then set the wave to active
        CurrentState = GameState.WaveActive;
    }

    private void HandleWaveActive()
    {
        // check for enemy spawn count, etc.

        // check for spawn delay
        if (TicksSinceLastSpawn > SpawnDelay && RemainingEnemies > 0)
        {
            EnemyObject e = Instantiate<EnemyObject>(EnemyTemplate);

            // do anything to this particular one?
            e.gameObject.GetComponent<MeshRenderer>().material.color = enemyTemplateForThisWave;

            e.ApplyPropertiesFromColor(enemyTemplateForThisWave);
            e.transform.position = EnvironmentSetup.CurrentSpawnPoint;

            // reset for next enemy
            TicksSinceLastSpawn = 0;
            RemainingEnemies--;
        }
        else
        {
            TicksSinceLastSpawn++;
        }

        // check for all enemies spawned, then pause wave
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

    public void EnemyHitHome(GameObject o)
    {
        GameObject h = EnvironmentSetup.CurrentEnvironment.GetHomeObjectAt(o.transform.position);

        // reduce HP of home
        h.GetComponent<HomeScript>().HPCurrent -= o.GetComponent<EnemyObject>().DmgCurrent;

        // update the label
        RefreshHomeHP();

        // destroy enemy object
        Destroy(o);

        if(h.GetComponent<HomeScript>().HPCurrent <= 0)
        {
            CurrentState = GameState.WaveFailed;
        }
    }

    private void RefreshHomeHP()
    {
        UIManager.CurrentUIManager.txtHomeHP.text = "HP: " + EnvironmentSetup.CurrentEnvironment.CurrentHome.GetComponent<HomeScript>().HPCurrent;
    }

    public void StartWaveClick()
    {
        if(CurrentState == GameState.SessionStart)
        {
            CurrentState = GameState.WaveStart;
        }

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
