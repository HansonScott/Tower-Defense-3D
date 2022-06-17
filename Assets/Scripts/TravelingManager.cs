using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelingManager : MonoBehaviour
{
    public float CurrentTravelSpeed = 0.02f;
    public int TargetNode = 1; // default to spawn node (0), plus 1.
    public Vector3 NextTarget;

    public float TargetMargin = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // establish first target
        NextTarget = EnvironmentSetup.GetNextTarget(TargetNode++);
    }

    // Update is called once per frame
    void Update()
    {
        if(NextTarget == null || // catches the opening case with no target yet
            OnTargetNode())
        {
            // if on home node, then damage and end
            if(TargetNode == EnvironmentSetup.HomeNode)
            {
                // then we've reached the home, damage it

                // To Do...
            }

            // progress to next node
            NextTarget = EnvironmentSetup.GetNextTarget(TargetNode++);
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
        move = move.normalized * CurrentTravelSpeed;

        // move the enemy in that direction.
        this.transform.position += move;
    }
}