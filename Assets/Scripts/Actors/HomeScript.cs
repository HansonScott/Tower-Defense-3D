using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScript : MonoBehaviour
{
    public int HPMax = 100;
    public int HPCurrent;


    // Start is called before the first frame update
    void Start()
    {
        ResetHPtoMax();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetHPtoMax()
    {
        HPCurrent = HPMax;
    }

    public void DamageHome(int dmgAmt)
    {
        ChangeCurrentHP(-dmgAmt);
    }

    public void HealHome(int healAmt)
    {
        ChangeCurrentHP(healAmt);
    }

    private void ChangeCurrentHP(int changeAmt)
    {
        HPCurrent += changeAmt;

        if(HPCurrent <= 0 )
        {
            // we died, trigger that game state.
            GameManager.CurrentGame.CurrentState = GameState.WaveFailed;
        }

        HPCurrent = Math.Clamp(HPCurrent, 0, HPMax);
    }
}
