using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelingManager : MonoBehaviour
{
    public int TargetNode = 1; // default to spawn node (0), plus 1.
    public Vector3 NextTarget;

    public float TargetMargin = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // establish first target
        NextTarget = EnvironmentManager.GetNextTarget(TargetNode++);
    }

    // Update is called once per frame
    void Update()
    {
        // shouldn't need this if the entire game object is destoryed...
        //if (!this.gameObject.GetComponent<EnemyObject>().IsALive) { Destroy(this); return; }

        if(NextTarget == null || // catches the opening case with no target yet
            OnTargetNode())
        {
            // if on home node, then damage and end
            if(TargetNode == EnvironmentManager.HomeNode)
            {
                // then we've reached the home, damage it

                // To Do...
            }

            // progress to next node
            NextTarget = EnvironmentManager.GetNextTarget(TargetNode++);

            if(NextTarget == new Vector3())
            {
                //then we've hit 'home'
                HandleHomeTargetHit();
            }
        }

        // move to the next path node
        MoveTowardsNextNode();
    }

    private bool OnTargetNode()
    {
        Vector3 move = (NextTarget - this.transform.position);
        move.y = 0; // dont' account for vertical
        return (move.magnitude < TargetMargin);
    }

    private void MoveTowardsNextNode()
    {
        // get the direction to the next target
        Vector3 move = (NextTarget - this.transform.position);
        
        move.y = 0; // no need to move vertically

        // normalize on speed
        move = move.normalized * this.gameObject.GetComponent<EnemyObject>().SpeedCurrent;

        // move the enemy in that direction.
        this.transform.position += move;
    }

    private void HandleHomeTargetHit()
    {
        // don't do anything here, but tell the game manager what happened
        GameManager.CurrentGame.EnemyHitHome(this.gameObject);
    }
}
