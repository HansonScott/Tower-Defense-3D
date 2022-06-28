using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerState
{
    Placing,
    AtRest,
    Seeking,
}

public enum AttackType
{
    /// <summary>
    /// single projectile that shoots directly to the target at high speed, hitting all but the fastest enemies
    /// </summary>
    Bullet,
    /// <summary>
    /// single slow projectile that tracks down the target, despite the enemy's speed and location
    /// </summary>
    Missle,
    /// <summary>
    /// shoots an immediate and direct line to the target, no projectile travel time needed, never misses
    /// </summary>
    Laser,
    /// <summary>
    /// lobs a slow projectile towards the target in an arc, hitting the location it was initially launched to
    /// </summary>
    Bomb,
    /// <summary>
    /// hits all enemies that are within the radius
    /// </summary>
    Aura,
}

public enum EffectType
{
    None,
    Aflame,
    Chill,
    Stun,
    Poison,
    Confuse,
    Weaken,
}

public enum TargetPriority
{
    /// <summary>
    /// The enemy that is the farthest along in the path, closest to the home target
    /// </summary>
    First,
    /// <summary>
    /// The enemy that is the newest along the path, farthest from the home target
    /// </summary>
    Last,
    /// <summary>
    /// the enemy with the most current HP
    /// </summary>
    Strong,
    /// <summary>
    /// the enemy with the least current HP
    /// </summary>
    Weak,
    /// <summary>
    /// the enemy moving at the fastest current speed
    /// </summary>
    Fast,
    /// <summary>
    /// the enemy moving at the slowest current speed
    /// </summary>
    Slow,
    /// <summary>
    /// the enemy with the smallest range away from this tower
    /// </summary>
    Close,
    /// <summary>
    /// the enemy with the largest range away from this tower
    /// </summary>
    Far,
}

public enum TargetConsistency
{
    Stick,
    Jump,
}

public class TowerManager : MonoBehaviour
{
    #region Properties and Fields
    public TowerState CurrentState;

    public TargetPriority CurrentPriority = TargetPriority.First;
    public TargetConsistency CurrentTargetConsistency = TargetConsistency.Stick;
    public EnemyObject CurrentTarget;

    public float RangeMax = 6;
    public float RangeCurrent = 6;

    public float ProjectileSpeed = 200f;

    public float AttackDelayBase;
    public float AttackDelayCurrent;
    private float TimeSinceLastAttack;

    public int TurnSpeed = 100;

    public List<AttackEffect> AttackEffects; // this includes the normal bullet

    public GameObject BulletTemplate;

    private bool _CurrentlySelected;
    public bool CurrentlySelected
    {
        get { return _CurrentlySelected; }
        set
        {
            _CurrentlySelected = value;
            this.transform.Find("SelectionPlane").gameObject.SetActive(value);
            this.transform.Find("RangePlane").gameObject.SetActive(value);
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // be able to fire immediately
        TimeSinceLastAttack = AttackDelayCurrent;
    }

    // Update is called once per frame
    void Update()
    {
        // dont' do anything yet...
        if(this.CurrentState == TowerState.Placing) { return; }
        if (GameManager.CurrentGame.CurrentState == GameState.WaveFailed) { return; } // don't do anything at this time

        if (PauseManager.CurrentGameSpeed == PauseManager.GameSpeed.Paused) { return; }

        // find next target
        List<EnemyObject> targets = FindAllEnemiesWithinRange();

        if(targets.Count > 0)
        {
            #region Pick a target
            if (CurrentTarget == null || // don't have a target yet
                !targets.Contains(CurrentTarget) || // last target is out of range
                CurrentTargetConsistency == TargetConsistency.Jump) // or we want to re-evaluate anyway
            {
                if(targets.Count == 1)
                {
                    CurrentTarget = targets[0];
                }
                else // targets.Count > 1
                {
                    CurrentTarget = DecideWhichTarget(targets);
                }
            }
            #endregion

            // turn towards target
            TurnTowardsTarget(CurrentTarget.transform.position);

            // check timing for attack
            if(TimeSinceLastAttack > AttackDelayCurrent)
            {
                // if can, attack
                AttackTarget(CurrentTarget);
                TimeSinceLastAttack = 0;
            }
            else
            {
                // and count towards our next attack
                TimeSinceLastAttack += Time.deltaTime;
            }
        }
    }

    private List<EnemyObject> FindAllEnemiesWithinRange()
    {
        List<EnemyObject> enemies = EnvironmentManager.CurrentEnvironment.GetAllEnemies();
        List<EnemyObject> enemiesWithinRange = new();

        if(enemies.Count > 0)
        {
            foreach(EnemyObject e in enemies)
            {
                if(e == null) { continue; }

                // if within range
                if (Vector3.Distance(e.transform.position, transform.position) <= RangeCurrent)
                {
                    enemiesWithinRange.Add(e);
                }
            }
        }

        return enemiesWithinRange;
    }

    private EnemyObject DecideWhichTarget(List<EnemyObject> targets)
    {
        if(targets.Count == 0) { return null; }
        else if(targets.Count == 1) { return targets[0]; }

        // choose based on priority
        switch(CurrentPriority)
        {
            case TargetPriority.Fast:
                // sort by current speed, desc
                targets.Sort((e1, e2) => e1.SpeedCurrent.CompareTo(
                                        e2.SpeedCurrent));
                break;
            case TargetPriority.Slow:
                // sort by current speed, asc
                targets.Sort((e1, e2) => e2.SpeedCurrent.CompareTo(
                                        e1.SpeedCurrent));
                break;
            case TargetPriority.Strong:
                // sort by current HP, desc
                targets.Sort((e1, e2) => e1.HPCurrent.CompareTo(
                                        e2.HPCurrent));
                break;
            case TargetPriority.Weak:
                // sort by current speed, asc
                targets.Sort((e1, e2) => e2.HPCurrent.CompareTo(
                                        e1.HPCurrent));
                break;
            case TargetPriority.First:
                // sort by path nodes remaining, asc
                targets.Sort((e1, e2) => e2.gameObject.GetComponent<TravelingManager>().TargetNode.CompareTo(
                                        e1.gameObject.GetComponent<TravelingManager>().TargetNode));
                break;
            case TargetPriority.Last:
                // sort by path nodes remaining, desc
                targets.Sort((e1, e2) => e1.gameObject.GetComponent<TravelingManager>().TargetNode.CompareTo(
                                        e2.gameObject.GetComponent<TravelingManager>().TargetNode));
                break;
            case TargetPriority.Close:
                // sort by range, ace
                targets.Sort((e1, e2) => Vector3.Distance(transform.position, e1.gameObject.transform.position).CompareTo(
                                        Vector3.Distance(transform.position, e2.gameObject.transform.position)));
                break;
            case TargetPriority.Far:
                // sort by range, desc
                targets.Sort((e1, e2) => Vector3.Distance(transform.position, e2.gameObject.transform.position).CompareTo(
                                        Vector3.Distance(transform.position, e1.gameObject.transform.position)));
                break;
            default:
                // a default?
                break;
        }

        return targets[0];
    }

    private void TurnTowardsTarget(Vector3 targetPosition)
    {
        // get the difference between tower and enemy target
        Vector3 targetDirection = targetPosition - transform.position;

        // applies rotation to this tower, adjusting by -90 for some reason...
        transform.rotation = Quaternion.LookRotation(targetDirection) * Quaternion.Euler(0,-90,0);
    }

    private void AttackTarget(EnemyObject e)
    {
        // basically create a projectile and pass all the info to it, and we're done.
        foreach(AttackEffect ae in this.AttackEffects)
        {
            switch(ae.AttackTypeBase)
            {
                case AttackType.Bullet:
                    GameObject projectileBullet = Instantiate(BulletTemplate);
                    ProjectileManager pm = projectileBullet.GetComponent<ProjectileManager>();
                    pm.ApplyPropertiesFromSource(this.gameObject);
                    pm.ApplyTargetEnemy(e);
                    pm.CurrentState = ProjectileState.Ready;
                    break;
                case AttackType.Laser:
                    break;
                case AttackType.Missle:
                    break;
                case AttackType.Aura:
                    break;
                default:
                    break;
            }
        }
    }
}
