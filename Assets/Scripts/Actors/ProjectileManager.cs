using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Enemy,
    Location
}

public class ProjectileManager : MonoBehaviour
{
    public EnemyObject TargetEnemy;
    public Vector3 TargetLocation;

    public TargetType TargetType;

    public float ProjectileSpeed;

    public int Dmg;
    public AttackType ProjectileType;
    public List<AttackEffect> ProjectileEffects;

    public float HitDistanceMargin = 1.0f;

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
        ProjectileEffects = tm.AttackEffects;

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
        //if reached target, damage it
        if(ReachedTarget())
        {
            ApplyAttacks();

            //destroy self
            Destroy(this);
        }
        else
        {
            // if not on target, move according to type
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {

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
