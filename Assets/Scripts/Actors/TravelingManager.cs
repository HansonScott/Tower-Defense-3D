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
        NextTarget = EnvironmentManager.GetNextTargetOnPath(TargetNode++);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.CurrentGame.CurrentState == GameState.WaveFailed) { return; } // dont' do anything at this time


        // shouldn't need this if the entire game object is destoryed...
        //if (!this.gameObject.GetComponent<EnemyObject>().IsALive) { Destroy(this); return; }

        if (NextTarget == null || // catches the opening case with no target yet
            OnTargetNode())
        {
            // progress to next node
            NextTarget = EnvironmentManager.GetNextTargetOnPath(TargetNode++);

            // if on home node, then damage and end
            if (NextTarget == new Vector3())
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
        move = move.normalized * this.gameObject.GetComponent<EnemyObject>().SpeedCurrent * Time.deltaTime;

        // move the enemy in that direction.
        this.transform.position += move;
    }

    private void HandleHomeTargetHit()
    {
        // tell the game manager to hit the home base
        GameManager.CurrentGame.EnemyHitHome(this.gameObject.GetComponent<EnemyObject>().DmgCurrent, this.gameObject.transform.position);

        // and we're done
        Destroy(this.gameObject);
    }
}
