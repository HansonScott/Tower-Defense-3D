using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public AttackType AttackTypeBase;

    public EffectType AttackEffectBase;

    public float Damage = 1;

    public float Duration = 0f;

    public int ChainCountRemaining = 0;

    public float SplashDamagePercentage = 0f;

    public float SplashRange = 2.0f;

    public float SpeedReductionPercentage = 0f;

    public float WeakenPercentage = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
