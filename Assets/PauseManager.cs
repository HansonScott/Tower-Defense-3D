using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private static GameSpeed _CurrentGameSpeed;
    public static GameSpeed CurrentGameSpeed
    {
        get { return _CurrentGameSpeed; }
        set
        {
            _CurrentGameSpeed = value;
            Time.timeScale = (float)value;
        }
    }

    public enum GameSpeed
    {
        Paused = 0,
        Play = 1,
        FF1 = 2,
        FF2 = 4,
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // space toggles play and pause
        {
            if (CurrentGameSpeed != GameSpeed.Paused)
            {
                CurrentGameSpeed = GameSpeed.Paused;
            }
            else
            {
                CurrentGameSpeed = GameSpeed.Play;
            }
        }
        // numbers sets speed
        else if (Input.GetKeyDown(KeyCode.Alpha1)) { CurrentGameSpeed = GameSpeed.Play; }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { CurrentGameSpeed = GameSpeed.FF1; }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { CurrentGameSpeed = GameSpeed.FF2; }
        else { }

    }
}
