using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum StatusEffect
{
    Chilled,
    Confused,
    Shocked,
    Poisoned,
    Aflame,
    Weakened,
}

public class EnemyObject : MonoBehaviour
{
    public bool IsALive = true;

    public float NearbyDistance = 1.0f;

    #region Properties
    public float HPMax
    {
        get;
        set;
    }
    private float _HPCurrent;
    public float HPCurrent
    {
        get { return _HPCurrent; }
        set
        {
            _HPCurrent = value;

            if(_HPCurrent <= 0)
            {
                // then we've died.
                this.IsALive = false;

                // and report this out to the game
                GameManager.CurrentGame.CurrentScore += 1; // future: different points per enemy?
                UIManager.CurrentUIManager.RefreshEnemyInfoBox();
            }
        }
    }
    public float SpeedMax
    {
        get;
        set;
    }
    public float SpeedCurrent
    {
        get;
        set;
    }
    public float ArmorMax
    {
        get;
        set;
    }
    public float ArmorCurrent
    {
        get;
        set;
    }
    public float DmgMax
    {
        get;
        set;
    }
    public float DmgCurrent
    {
        get;
        set;
    }

    public List<AttackEffect> CurrentAttackEffects;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // change later?
        DmgMax = 1;
        DmgCurrent = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsALive) { Destroy(this.gameObject);}
        else 
        {
            ApplyAnyActiveAttackEffects();
        }

    }

    public static Color GetRandomEnemyProperties(int totalPower)
    {
        int red = Mathf.Clamp(Random.Range(1, totalPower), 0, 255);
        int green = (int)Mathf.Clamp(Random.Range(1, totalPower - red), 0, 255);
        int blue = Mathf.Clamp(Random.Range(0, (totalPower - red - green)), 0, 255);

        //Color result = new Color(red, green, blue);
        //Color result = new Color((red / 255), (green / 255), (blue / 255)); // seems to be not the right scale, examples in code ref are between 0 and 1 for each...
        Color result = new Color((red / 25), (green / 25), (blue / 25)); // guessed at the scale, not sure why 25 results in color change...
        return result;
    }

    internal void ApplyPropertiesFromColor(Color c)
    {
        this.HPMax = (int)c.r;
        this.HPCurrent = HPMax;

        this.SpeedMax = 0.01f + (((c.g / 255) * 2) / 100); // percentage of max, weighted twice, then divided into thousandths, which is the movement norm.
        this.SpeedCurrent = this.SpeedMax;

        this.ArmorMax = (int)c.b;
        this.ArmorCurrent = this.ArmorMax;
    }

    internal void ApplyTowerAttack(AttackEffect a)
    {
        switch(a.AttackEffectBase)
        {
            case EffectType.None:
                this.HPCurrent -= a.Damage;
                break;
            case EffectType.Aflame:
                this.HPCurrent -= a.Damage;

                // look to spread the fire to neighboring enemies
                LookToSpreadFireToNeighbor(a);

                break;
            case EffectType.Chill:
                this.SpeedCurrent = (this.SpeedMax * (1 - a.Damage)); // assuming the damage to be a percentage reduction in speed, between 0 and 1
                break;
            case EffectType.Confuse:
                // nothing special at this point, but keep track, below
                break;
            case EffectType.Poison:
                this.HPCurrent -= a.Damage;
                break;
            case EffectType.Stun:
                this.SpeedCurrent = 0;
                break;
            case EffectType.Weaken:
                this.ArmorCurrent = (this.ArmorMax * (1 - a.Damage)); // assuming the damage to be a percentage reduction in armor, between 0 and 1.
                break;
            default:
                break;
        }

        // now, apply specials, regardless of above
        EnemyObject[] allEnemies = EnvironmentManager.CurrentEnvironment.GetAllEnemies();
        List<EnemyObject> nearbyEnemies = GetNearbyEnemies(allEnemies, a.SplashRange);

        if (a.ChainCountRemaining > 0)
        {
            // apply this same attack to nearby enemies (create a new attack), reducing the chain by 1
            a.ChainCountRemaining--;
            int target = Random.Range(0, nearbyEnemies.Count);

            nearbyEnemies[target].ApplyTowerAttack(GameObject.Instantiate(a));

        }

        if(a.SplashDamagePercentage > 0)
        {
            // apply attack to nearby enemies (create a new attack), reducing the damage by the percentage
            a.Damage = 1 - (a.Damage * a.SplashDamagePercentage);
            a.SplashDamagePercentage = 0; // don't double-splash

            foreach(EnemyObject e in nearbyEnemies)
            {
                e.ApplyTowerAttack(GameObject.Instantiate(a));
            }
        }

        if (a.Duration > 0) { this.CurrentAttackEffects.Add(a); }

        if(UIManager.CurrentUIManager.EnemySourceForInfoBox == this)
        {
            UIManager.CurrentUIManager.RefreshEnemyInfoBox();
        }
    }

    private List<EnemyObject> GetNearbyEnemies(EnemyObject[] allEnemies) { return GetNearbyEnemies(allEnemies, NearbyDistance); }
    private List<EnemyObject> GetNearbyEnemies(EnemyObject[] allEnemies, float Range)
    {
        List<EnemyObject> result = new();

        foreach(EnemyObject e in allEnemies)
        {
            if(e == this) { continue; }

            if (e == null || !e.IsALive) { continue; }

            if(Vector3.Distance(e.transform.position, this.transform.position) < Range)
            {
                result.Add(e);
            }
        }

        return result;
    }

    private void ApplyAnyActiveAttackEffects()
    {
        if(CurrentAttackEffects.Count == 0) { return; }

        for (int i = CurrentAttackEffects.Count - 1; i <= 0; i--)
        {
            AttackEffect a = CurrentAttackEffects[i];
            if(a.Duration > 0)
            {
                switch (a.AttackEffectBase)
                {
                    //case EffectType.None: // shouldn't be any here
                    //    break;
                    case EffectType.Aflame:
                        HPCurrent -= a.Damage;

                        // look to spread the fire to neighboring enemies
                        LookToSpreadFireToNeighbor(a);

                        break;
                    case EffectType.Chill:
                        // NOTE: attempt to apply again, since it might have changed based on other factors
                        this.SpeedCurrent = Mathf.Min(this.SpeedCurrent, (this.SpeedMax * (1 - a.Damage))); // assuming the damage to be a percentage reduction in speed, between 0 and 1
                        break;
                    case EffectType.Poison:
                        HPCurrent -= a.Damage;
                        break;
                    case EffectType.Stun:
                        this.SpeedCurrent = 0;
                        break;
                    case EffectType.Weaken:
                        this.ArmorCurrent = Mathf.Min(this.ArmorCurrent, (this.ArmorMax * (1 - a.Damage))); // assuming the damage to be a percentage reduction in armor, between 0 and 1.
                        break;
                    default:
                        break;
                }
            }
            else // duration has expired, remove statuses
            {
                switch (a.AttackEffectBase)
                {
                    case EffectType.Chill:
                        this.SpeedCurrent = this.SpeedMax;
                        break;
                    case EffectType.Stun:
                        this.SpeedCurrent = this.SpeedMax;
                        break;
                    case EffectType.Weaken:
                        this.ArmorCurrent = ArmorMax;
                        break;
                    default:
                        break;
                }

                // since the duration expired, remove it from the list
                CurrentAttackEffects.Remove(a);
            }
        }
    }

    private void LookToSpreadFireToNeighbor(AttackEffect a)
    {
        // 5% chance (just guessing, since this is every tick)
        if (Random.Range(1, 101) <= 5)
        {
            // spread fire to a neighbor
            List<EnemyObject> nearbyEnemies = GetNearbyEnemies(EnvironmentManager.CurrentEnvironment.GetAllEnemies());

            int target = Random.Range(0, nearbyEnemies.Count);

            nearbyEnemies[target].ApplyTowerAttack(a);
        }
    }

}
