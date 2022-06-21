using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Enemy,
    Location
}

public enum ProjectileState
{
    Ready,
    Traveling,
    Complete,
}

public class ProjectileManager : MonoBehaviour
{
    public EnemyObject TargetEnemy;
    public Vector3 TargetLocation;

    public TargetType TargetType;

    public float ProjectileSpeed;

    public ProjectileState CurrentState = ProjectileState.Ready;

    public int Dmg;
    public AttackType ProjectileType;
    public List<AttackEffect> ProjectileEffects;

    public float HitDistanceMargin = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ApplyPropertiesFromSource(GameObject source)
    {
        // starting location
        this.transform.position = source.transform.position;

        // attack details
        TowerManager tm = source.GetComponent<TowerManager>();
        ProjectileEffects.AddRange(tm.AttackEffects);

        // projectileSpeed
        ProjectileSpeed = tm.ProjectileSpeed;
    }

    public void ApplyTargetEnemy(EnemyObject e)
    {
        TargetEnemy = e;
        TargetType = TargetType.Enemy;
    }

    public void ApplyTargetLocation(Vector3 loc)
    {
        TargetLocation = loc;
        TargetType = TargetType.Location;
    }



    // Update is called once per frame
    void Update()
    {
        // first check that our destination still exists, or just quit
        if(TargetType == TargetType.Enemy &&
            TargetEnemy == null || TargetEnemy.gameObject == null) { Destroy(this); Destroy(this.gameObject); }


        if(CurrentState == ProjectileState.Traveling)
        {
            //if reached target, damage it
            if (ReachedTarget())
            {
                CurrentState = ProjectileState.Complete;

                ApplyAttacks();

                //destroy self
                Destroy(this.gameObject);
            }
            else
            {
                // if not on target, move according to type
                MoveTowardsTarget();
            }
        }
    }

    private void MoveTowardsTarget()
    {
        // get the direction to the next target
        Vector3 NextTarget = this.TargetEnemy.transform.position;
        if(TargetType == TargetType.Location)
        {
            NextTarget = this.TargetLocation;
        }

        Vector3 move = (NextTarget - this.transform.position);

        //move.y = 0; // no need to move vertically

        // normalize on speed
        move = move.normalized * ProjectileSpeed;

        // move the enemy in that direction.
        this.transform.position += move;

    }

    private bool ReachedTarget()
    {
        if(TargetType == TargetType.Enemy) // if the enemy is dead and gone, then quit
        {
            if(TargetEnemy == null)
            {
                return true;
            }

            return (Vector3.Distance(TargetEnemy.transform.position, this.transform.position) < HitDistanceMargin);
        }
        else
        {
            return (Vector3.Distance(TargetLocation, this.transform.position) < HitDistanceMargin);
        }
    }

    private void ApplyAttacks()
    {
        foreach(AttackEffect a in this.ProjectileEffects)
        {
            ApplyAttack(a);
        }
    }
    private void ApplyAttack(AttackEffect a)
    {
        switch(a.AttackTypeBase)
        {
            case AttackType.Aura:
                break;
            case AttackType.Bomb:
                break;
            case AttackType.Bullet:
                // damage the target
                TargetEnemy.ApplyTowerAttack(a);
                break;
            case AttackType.Laser:
                break;
            case AttackType.Missle:
                break;
            default:
                break;
        }
    }
}
