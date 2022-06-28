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
    WaveComplete,
    WaveWin,
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
        set
        {
            _CurrentMoney = value;
            // option 1: push to UI
            //UIManager.CurrentUIManager.RefreshMoneyLabel(_CurrentMoney);

            // option 2: create an event for any listeners
            if (OnCurrentMoneyChange != null)
                OnCurrentMoneyChange(_CurrentMoney);
        }
    }
    public delegate void OnCurrentMoneyChangeDelegate(int newVal);
    public event OnCurrentMoneyChangeDelegate OnCurrentMoneyChange;

    public EnemyObject EnemyTemplate;
    private Color enemyTemplateForThisWave;

    private WaveInfo _CurrentWaveInfo;
    public WaveInfo CurrentWaveInfo
    {
        get
        {
            if(_CurrentWaveInfo == null)
            {
                _CurrentWaveInfo = new WaveInfo();
            }

            return _CurrentWaveInfo;
        }
        private set { }
    }
    public class WaveInfo
    {
        private int _MaxWavesForThisSession = 20;
        public int MaxWavesForThisSession
        {
            get { return _MaxWavesForThisSession; }
            set 
            { 
                _MaxWavesForThisSession = value; 
                if (onCurrentWaveInfoChange != null) 
                { onCurrentWaveInfoChange(this); } 
            }
        }

        private int _CurrentWave = 0;
        public int CurrentWave
        {
            get { return _CurrentWave; }
            set
            {
                _CurrentWave = value;
                if (onCurrentWaveInfoChange != null)
                { onCurrentWaveInfoChange(this); }
            }
        }

        private float _TimeSinceNewWave = 0;
        public float TimeSinceNewWave
        {
            get { return _TimeSinceNewWave; }
            set
            {
                _TimeSinceNewWave = value;
                if (onCurrentWaveInfoChange != null)
                { onCurrentWaveInfoChange(this); }
            }
        }

        private float _WaveDelay = 10;
        public float WaveDelay
        {
            get { return _WaveDelay; }
            set
            {
                _WaveDelay = value;
                if (onCurrentWaveInfoChange != null)
                { onCurrentWaveInfoChange(this); }
            }
        }

        private float _TimeSinceLastSpawn = 0;
        public float TimeSinceLastSpawn
        {
            get { return _TimeSinceLastSpawn; }
            set
            {
                _TimeSinceLastSpawn = value;
                if (onCurrentWaveInfoChange != null)
                { onCurrentWaveInfoChange(this); }
            }
        }

        private float _SpawnDelay = 1.5f;
        public float SpawnDelay
        {
            get { return _SpawnDelay; }
            set
            {
                _SpawnDelay = value;
                if (onCurrentWaveInfoChange != null)
                { onCurrentWaveInfoChange(this); }
            }
        }

        private int _EnemyCountInWave = 10;
        public int EnemyCountInWave
        {
            get { return _EnemyCountInWave; }
            set
            {
                _EnemyCountInWave = value;
                if (onCurrentWaveInfoChange != null)
                { onCurrentWaveInfoChange(this); }
            }
        }

        private int _RemainingEnemies = 10;
        public int RemainingEnemies
        {
            get { return _RemainingEnemies; }
            set
            {
                _RemainingEnemies = value;
                if (onCurrentWaveInfoChange != null)
                { onCurrentWaveInfoChange(this); }
            }
        }


        public delegate void OnWaveInfoChangeDelegate(WaveInfo info);
        public event OnWaveInfoChangeDelegate onCurrentWaveInfoChange;
    }


    // Start is called before the first frame update
    void Start()
    {
        CurrentGame = this;
        CurrentWaveInfo = new WaveInfo();
    }

    #region Update and Primary State Handling
    // Update is called once per frame(?)
    void Update()
    {
        switch (CurrentState)
        {
            case GameState.SessionMenu:
                HandleSessionMenu();
                break;
            case GameState.SessionStart:
                HandleSessionStart();
                break;
            case GameState.WaveStart:

                //if (PauseManager.CurrentGameSpeed == PauseManager.GameSpeed.Paused) { return; }

                HandleWaveStart();
                //UIManager.CurrentUIManager.btnWave.enabled = false;
                break;
            case GameState.WaveActive:

                if (PauseManager.CurrentGameSpeed == PauseManager.GameSpeed.Paused) { return; }

                HandleWaveActive();
                break;
            case GameState.WavePause:

                if (PauseManager.CurrentGameSpeed == PauseManager.GameSpeed.Paused) { return; }

                HandleWavePause();
                break;
            case GameState.WaveComplete:

                if (PauseManager.CurrentGameSpeed == PauseManager.GameSpeed.Paused) { return; }

                HandleWaveComplete();
                break;
            case GameState.WaveWin:
                HandleWaveWin();
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

        // future: show menu
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

        if (CurrentWaveInfo.TimeSinceNewWave > CurrentWaveInfo.WaveDelay)
        {
            int powerLevel = ++CurrentWaveInfo.CurrentWave * 100;

            if(CurrentWaveInfo.CurrentWave % 10 == 0)
            {
                // boss wave
                powerLevel *= 10; // 10 times as strong

                // start the wave variables
                enemyTemplateForThisWave = EnemyObject.GetRandomEnemyProperties(powerLevel);

                CurrentWaveInfo.RemainingEnemies = 1;
            }
            else if (CurrentWaveInfo.CurrentWave % 10 == 5)
            {
                // mini boss wave
                powerLevel *= 5; // 10 times as strong

                // start the wave variables
                enemyTemplateForThisWave = EnemyObject.GetRandomEnemyProperties(powerLevel);

                CurrentWaveInfo.RemainingEnemies = 3;
            }
            else
            {
                // start the wave variables
                enemyTemplateForThisWave = EnemyObject.GetRandomEnemyProperties(powerLevel);

                CurrentWaveInfo.RemainingEnemies = CurrentWaveInfo.EnemyCountInWave;

            }

            // then set the wave to active
            CurrentState = GameState.WaveActive;
            CurrentWaveInfo.TimeSinceLastSpawn = CurrentWaveInfo.SpawnDelay; // set initial enemy to spawn immediately
        }
        else
        {
            CurrentWaveInfo.TimeSinceNewWave += Time.deltaTime;
        }
    }

    private void HandleWaveActive()
    {
        // check for enemy spawn count, etc.

        // check for spawn delay
        if (CurrentWaveInfo.TimeSinceLastSpawn >= CurrentWaveInfo.SpawnDelay && CurrentWaveInfo.RemainingEnemies > 0)
        {
            EnemyObject e = EnvironmentManager.CurrentEnvironment.PlaceNewEnemy(EnemyTemplate);

            e.onIsAliveChange += E_onIsAliveChange;

            e.gameObject.GetComponent<MeshRenderer>().material.color = enemyTemplateForThisWave;
            e.ApplyPropertiesFromColor(enemyTemplateForThisWave);

            // boss wave
            if(CurrentWaveInfo.CurrentWave % 10 == 0)
            {
                // even with a higher power level, make sure the HP is way up.
                e.HPMax *= 5;
                e.HPCurrent = e.HPMax;

                // and slower
                e.SpeedMax *= 0.5f;
                e.SpeedCurrent = e.SpeedMax;

                // set the bigger size
                Vector3 s = e.transform.localScale;
                s.Set(1.1f, 1.2f, 1.1f);
                e.transform.localScale = s;

                // and damages home more
                e.DmgMax *= 10;
                e.DmgCurrent = e.DmgMax;
            }
            // mini boss wave
            else if(CurrentWaveInfo.CurrentWave % 10 == 5)
            {
                // even with a higher power level, make sure the HP is way up.
                e.HPMax *= 3;
                e.HPCurrent = e.HPMax;

                // and slower
                e.SpeedMax *= 0.5f;
                e.SpeedCurrent = e.SpeedMax;

                // set the bigger size
                Vector3 s = e.transform.localScale;
                s.Set(1.05f, 1.0f, 1.05f);
                e.transform.localScale = s;

                // and damages home more
                e.DmgMax *= 5;
                e.DmgCurrent = e.DmgMax;
            }

            e.transform.position = EnvironmentManager.CurrentSpawnPoint;

            // reset for next enemy
            CurrentWaveInfo.TimeSinceLastSpawn = 0;
            CurrentWaveInfo.RemainingEnemies--;
        }
        else
        {
            CurrentWaveInfo.TimeSinceLastSpawn += Time.deltaTime;
        }

        // check for all enemies spawned, then pause wave
        if(CurrentWaveInfo.RemainingEnemies == 0)
        {
            // add more money for each wave cleared
            CurrentMoney += CurrentWaveInfo.CurrentWave * 25;

            if (CurrentWaveInfo.CurrentWave <= CurrentWaveInfo.MaxWavesForThisSession)
            {
                CurrentState = GameState.WaveStart;
                CurrentWaveInfo.TimeSinceNewWave = 0;
            }
            else
            {
                CurrentState = GameState.WaveComplete;
            }

        }
    }

    // when an enemy dies
    private void E_onIsAliveChange(EnemyObject e, bool alive)
    {
        CurrentScore += CurrentWaveInfo.CurrentWave; // adds 1 point per enemy per wave (ex: wave 20 = each enemy is worth 20)
        CurrentMoney += CurrentWaveInfo.CurrentWave * 5; // adds $5 per enemy per wave (ex: wave 20 = each enemy is worth $100)
    }

    private void HandleWavePause()
    {
        // check wave complete, or set a timer before next wave

    }

    private void HandleWaveWin()
    {
        // if so, then we win!
        print("All Enemies defated, you win!");
    }

    private void HandleWaveComplete()
    {
        // Since there's no more waves, keep checking for enemies all done,
        if (EnvironmentManager.CurrentEnvironment.GetAllEnemies().Count == 0)
        {
            // if so, then we win!
            print("All Enemies defated, you win!");

            CurrentState = GameState.SessionEnd;
        }
    }
    private void HandleSessionEnd()
    {
        // show final score, return to menu
        CurrentState = GameState.SessionMenu;
    }
    #endregion

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

    public void BtnPauseClick()
    {
        PauseManager.CurrentGameSpeed = PauseManager.GameSpeed.Paused;
    }
    public void btnPlayClick()
    {
        PauseManager.CurrentGameSpeed = PauseManager.GameSpeed.Play;

        if (CurrentState == GameState.SessionStart)
        {
            CurrentState = GameState.WaveStart;
            CurrentWaveInfo.TimeSinceNewWave = CurrentWaveInfo.WaveDelay; // start first wave immeditely
        }
    }
    public void BtnFF1Click()
    {
        PauseManager.CurrentGameSpeed = PauseManager.GameSpeed.FF1;
    }
    public void btnFF2Click()
    {
        PauseManager.CurrentGameSpeed = PauseManager.GameSpeed.FF2;
    }
    private void HandleWaveFailed()
    {
        print("wave failed!");
    }
}
