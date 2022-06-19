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
public enum UIState
{
    Normal,
    PlacingTower,
}

public class GameManager : MonoBehaviour
{
    public static GameManager CurrentGame;

    public GameState CurrentState = GameState.SessionMenu;
    public UIState CurrentUIState = UIState.Normal;

    public Camera MainCamera;
    public GameObject Enemy01;
    public GameObject Tower01;

    public int TicksSinceLastSpawn = 0;
    public int SpawnDelay = 100;
    public int EnemyCountInWave = 10;
    private int RemainingEnemies = 10;

    public GameObject CurrentlySelectedTower;

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

        switch(CurrentUIState)
        {
            case UIState.Normal:
                break;
            case UIState.PlacingTower:
                if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) // if we're not moving, then don't change anything.
                {
                    MoveCurrentlySelectedTowerToMousePosition();
                }
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

        // moved this to be button-based
        //CurrentState = GameState.WaveStart;
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
        if (TicksSinceLastSpawn > SpawnDelay && RemainingEnemies > 0)
        {
            SpawnEnemy();
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
    private void SpawnEnemy()
    {
        GameObject e = Instantiate(Enemy01);
        e.transform.position = EnvironmentSetup.CurrentSpawnPoint;
    }

    public void EnemyHitHome(GameObject o)
    {
        // reduce HP of home

        // destroy enemy object
        Destroy(o);
    }

    public void StartWaveClick()
    {
        CurrentState = GameState.WaveStart;
    }

    public void TowerClicked()
    {
        if(CurrentUIState == UIState.Normal)
        {
            CurrentUIState = UIState.PlacingTower;
            CurrentlySelectedTower = GetSelectedTower();
        }
        else
        {
            // we were already placing a tower, what should we do?  Change towers?
            CurrentlySelectedTower = GetSelectedTower();
        }
    }

    private GameObject GetSelectedTower()
    {
        // instantiate the right tower based on what button was pressed
        return Instantiate(Tower01);
    }

    private void MoveCurrentlySelectedTowerToMousePosition()
    {
        Vector3 mouseLocationInWorld = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        //print("mouse position: " + Input.mousePosition.ToString());

        RaycastHit rHit;
        Vector3 objectLocationInWorld = new Vector3();

        if(Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out rHit))
        {
            objectLocationInWorld = rHit.transform.position;
            //print("object location in world" + objectLocationInWorld.ToString());

            // future - get the proper height too.
            //print("target height: " + rHit.transform.localScale.y);
            objectLocationInWorld.y = 1 + (rHit.transform.localScale.y / 2);

            CurrentlySelectedTower.transform.position = objectLocationInWorld;
        }
    }
}
