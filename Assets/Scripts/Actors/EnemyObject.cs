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
    private bool _IsAlive = true;
    public bool IsAlive
    {
        get
        {
            return _IsAlive;
        }
        set 
        {
            _IsAlive = value;
            if (onIsAliveChange != null)
                onIsAliveChange(this, _IsAlive);
        }
    }
    public delegate void OnIsAliveChangeDelegate(EnemyObject e, bool alive);
    public event OnIsAliveChangeDelegate onIsAliveChange;

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
                this.IsAlive = false; // this triggers all the game manager event stuff.

                // this may update the enemy info box (move to event?)
                UIManager.CurrentUIManager.RefreshEnemyInfoBox();
            }
            else
            {
                UpdateHealthBar(HPCurrent, HPMax);
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
    
    private bool _CurrentlySelected;
    public bool CurrentlySelected
    {
        get { return _CurrentlySelected; }
        set 
        {
            try
            {
                _CurrentlySelected = value;
                this.transform.Find("SelectionPlane").gameObject.SetActive(value);
                this.transform.Find("RangePlane").gameObject.SetActive(value);
            }
            catch (System.Exception e)
            {
                // do we wan tto do anything here? the object is gone, no need to follow up?
                print(e.ToString());
            }
        }
    }

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
        if(GameManager.CurrentGame == null ||
            GameManager.CurrentGame.CurrentState == GameState.WaveFailed) { return; } // dont' do anything at this time

        if (!IsAlive) { Destroy(this.gameObject);}
        else 
        {
            if(PauseManager.CurrentGameSpeed != PauseManager.GameSpeed.Paused)
            {
                ApplyAnyActiveAttackEffects();
            }
        }

    }
    private void UpdateHealthBar(float c, float m)
    {
        try
        {
            if (gameObject == null) { return; }
        }
        catch { return; } // at this point, if we get an error here, we know exactly why, and don't want to do anything with it.

        Transform healthBar = gameObject.transform.GetChild(0);

        if(c == m) 
        {
            // make it invisible
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            // make it visible
            healthBar.gameObject.SetActive(true);

            // adjust width (x) as a percentage (it defaults to 1)
            float percentHealth = (HPCurrent / HPMax);
            healthBar.localScale = new Vector3(percentHealth, healthBar.localScale.y, healthBar.localScale.z);

            float newR = 0;
            float newG = 0;
            float newB = 0;

            // adjust color as a percentage (blue => green => yellow => orange => red)
            // blue     (100-80) =  0,0,255
            // green    (79-60)  =  0,255,0
            // yellow   (59-40)  =  255,255,0
            // orange   (39-20)  =  255,175,0
            // red      (19-0)   =  255,0,0

            #region Option 1: chunks - simple, but not elegant
            //if (percentHealth > .8)
            //{
            //    newR = 0;
            //    newG = 0;
            //    newB = 255;
            //}
            //else if (percentHealth > .6)
            //{
            //    newR = 0;
            //    newG = 255;
            //    newB = 0;
            //}
            //else if (percentHealth > .4)
            //{
            //    newR = 255;
            //    newG = 255;
            //    newB = 0;
            //}
            //else if (percentHealth > .2)
            //{
            //    newR = 255;
            //    newG = 175;
            //    newB = 0;
            //}
            //else //(newX > 0)
            //{
            //    newR = 255;
            //    newG = 0;
            //    newB = 0;
            //}
            #endregion

            #region Option2: Gradient w/ chunks
            #region red
            if (percentHealth > .6)
            {
                // red moves from 0 to 0
                newR = 0;
            }
            else if (percentHealth > .4)
            {
                // red moves from 0 to 255
                newR = (1f - ((percentHealth - 0.6f) * 5f)) * 255f;
            }
            else if (percentHealth > .2)
            {
                // red moves from 255 to 255
                newR = 255;
            }
            else //(newX > 0)
            {
                // red moves from 255 to 255
                newR = 255;
            }
            #endregion
            #region green
            if (percentHealth > .6)
            {
                // moves from 0 to 255
                newG = (1 - ((percentHealth - 0.8f) * 5f)) * 255f;
            }
            else if (percentHealth > .4)
            {
                // from 255 to 255
                newG = 255;
            }
            else if (percentHealth > .2)
            {
                // from 255 to 175
                newG = 175 + (((percentHealth - 0.5f) * 5f) * (255 - 175));
            }
            else //(newX > 0)
            {
                // from 175 to 0
                newG = (((percentHealth - 0.5f) * 5f) * 175);
            }
            #endregion
            #region Blue
            // blue
            if (percentHealth > .6)
            {
                // from 255 to 0
                newB = ((percentHealth - 0.6f) * 5f) * 255f;
            }
            else if (percentHealth > .4)
            {
                newB = 0;
            }
            else if (percentHealth > .2)
            {
                newB = 0;
            }
            else //(newX > 0)
            {
                newB = 0;
            }
            #endregion
            #endregion
        
            Color newColor = new Color(newR, newG, newB, 0.0f);
            healthBar.gameObject.GetComponent<MeshRenderer>().material.color = newColor;
        }
    }


    public static Color GetRandomEnemyProperties(int totalPower)
    {
        float red = Mathf.Clamp(Random.Range(1, totalPower), 0, 255);
        float green = (int)Mathf.Clamp(Random.Range(1, totalPower - red), 0, 255);
        float blue = Mathf.Clamp(Random.Range(0, (totalPower - red - green)), 0, 255);

        Color result = new Color();
        result.r = (red / 255);
        result.g = (green / 255);
        result.b = (blue / 255);

        //Color result = new Color((red / 255), (green / 255), (blue / 255)); // seems to be not the right scale, examples in code ref are between 0 and 1 for each...
        //Color result = new Color((red / 25f), (green / 25f), (blue / 25f)); // guessed at the scale, not sure why 25 results in color change...
        return result;
    }

    internal void ApplyPropertiesFromColor(Color c)
    {
        this.HPMax = (float)c.r * 100;
        this.HPCurrent = HPMax;
       
        this.SpeedMax = 1f + (c.g / 10);
        this.SpeedCurrent = this.SpeedMax;

        this.ArmorMax = (float)c.b;
        this.ArmorCurrent = this.ArmorMax;
    }

    internal void ApplyTowerAttack(AttackEffect a)
    {
        switch(a.AttackEffectBase)
        {
            case EffectType.None:
                this.HPCurrent -= Mathf.Max((a.Damage - this.ArmorCurrent), 1); // armor can block all but 1 dmg
                break;
            case EffectType.Aflame:
                this.HPCurrent -= a.Damage; // armor doesn't block fire

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
                this.HPCurrent -= a.Damage; // armor doesn't block poison
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
        List<EnemyObject> allEnemies = EnvironmentManager.CurrentEnvironment.GetAllEnemies();
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

        if(UIManager.CurrentUIManager.SourceForEnemyInfoBox == this)
        {
            UIManager.CurrentUIManager.RefreshEnemyInfoBox();
        }
    }

    private List<EnemyObject> GetNearbyEnemies(List<EnemyObject> allEnemies) { return GetNearbyEnemies(allEnemies, NearbyDistance); }
    private List<EnemyObject> GetNearbyEnemies(List<EnemyObject> allEnemies, float Range)
    {
        List<EnemyObject> result = new();

        foreach(EnemyObject e in allEnemies)
        {
            lock(e)
            {
                if (e == this) { continue; }

                if (e == null || !e.IsAlive) { continue; }
                if(this == null || !this.IsAlive || this.transform == null) { return null; }

                if (Vector3.Distance(e.transform.position, this.transform.position) < Range)
                {
                    result.Add(e);
                }
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
            if(nearbyEnemies == null) { return; }

            int target = Random.Range(0, nearbyEnemies.Count);

            nearbyEnemies[target].ApplyTowerAttack(a);
        }
    }

}
