using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScript : MonoBehaviour
{
    public int HPMax = 100;
    private int _HPCurrent = 100;
    public int HPCurrent 
    {
        get { return _HPCurrent; }
        set 
        {
            _HPCurrent = value;
            if (_HPCurrent > HPMax) { _HPCurrent = HPMax; }
            else if (_HPCurrent < 0)
            {
                _HPCurrent = 0;

                // let the caller check for death...
            }
        } 
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
