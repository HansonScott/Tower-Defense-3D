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

    public float HitDistanceMargin = 0.1f;
    private float prevousDistanceToTarget;

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
        if (GameManager.CurrentGame.CurrentState == GameState.WaveFailed) { return; } // dont' do anything at this time

        if (PauseManager.CurrentGameSpeed == PauseManager.GameSpeed.Paused) { return; }

            // first check that our destination still exists, or just quit
            if (TargetType == TargetType.Enemy &&
            TargetEnemy == null || TargetEnemy.gameObject == null) { Destroy(this); Destroy(this.gameObject); }

        if (CurrentState == ProjectileState.Complete) { Destroy(this.gameObject); } // if we're done. quit
        else if (CurrentState == ProjectileState.Ready)
        {
            // FIRE!
            MoveTowardsTarget();
        }
        else if (CurrentState == ProjectileState.Traveling)
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

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject == TargetEnemy.gameObject)
        {
            CurrentState = ProjectileState.Complete;
            ApplyAttacks();
            Destroy(this.gameObject);
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 NextTarget = new Vector3();
        if (this.TargetEnemy != null)
        {
            NextTarget = this.TargetEnemy.transform.position;
        }
        else if (TargetType == TargetType.Location)
        {
            NextTarget = this.TargetLocation;
        }

        if(NextTarget == new Vector3()) { return; }


        #region Option 1: move - this causes UI skipping because it is basically teleporting from one position to another per update - NO GOOD
        Vector3 move = (NextTarget - this.transform.position);

        // normalize on speed
        move = move.normalized * ProjectileSpeed * Time.deltaTime;

        // move the enemy in that direction.
        this.transform.position += move;
        #endregion


        // now we're traveling, let it fly
        if(CurrentState == ProjectileState.Ready)
        {
            CurrentState = ProjectileState.Traveling;
        }
    }

    private bool ReachedTarget()
    {
        if(TargetType == TargetType.Enemy) // if the enemy is dead and gone, then quit
        {
            if(TargetEnemy == null)
            {
                return true;
            }
            float dist = Vector3.Distance(TargetEnemy.transform.position, this.transform.position);
            if(prevousDistanceToTarget == 0) { prevousDistanceToTarget = dist; }
            if(dist > prevousDistanceToTarget) { this.CurrentState = ProjectileState.Complete; } // if we're going away from our target, just complete
            return dist < HitDistanceMargin;
        }
        else
        {
            float dist = Vector3.Distance(TargetLocation, this.transform.position);
            if (prevousDistanceToTarget == 0) { prevousDistanceToTarget = dist; }
            if (dist > prevousDistanceToTarget) { this.CurrentState = ProjectileState.Complete; } // if we're going away from our target, just complete
            return dist < HitDistanceMargin;
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
