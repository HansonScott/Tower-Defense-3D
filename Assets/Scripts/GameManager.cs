using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    SessionMenu,
    SessionStart,
    WaveStart,
    WaveActive,
    WavePause,
    SessionEnd,
}

public class GameManager : MonoBehaviour
{
    public GameState CurrentState = GameState.SessionMenu;

    public GameObject Enemy01;

    public int TicksSinceLastSpawn = 0;
    public int SpawnDelay = 500;
    private bool haveSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
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
                HandleWaveStart();
                break;
            case GameState.WaveActive:
                HandleWaveActive();
                break;
            case GameState.WavePause:
                HandleWavePause();
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

        CurrentState = GameState.WaveStart;
    }

    private void HandleWaveStart()
    {
        // start the wave variables, then set to active
        CurrentState = GameState.WaveActive;
    }

    private void HandleWaveActive()
    {
        // check for enemy spawn count, etc.

        // check for spawn delay
        if (TicksSinceLastSpawn > SpawnDelay && !haveSpawned)
        {
            SpawnEnemy();
            TicksSinceLastSpawn = 0;
            haveSpawned = true;
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
    private void SpawnEnemy()
    {
        GameObject e = Instantiate(Enemy01);
        e.transform.position = EnvironmentSetup.CurrentSpawnPoint;
    }
}