using System;
using UnityEngine;

public enum UIState
{
    Normal,
    PlacingTower,
}

public class UIManager : MonoBehaviour
{
    public static UIManager CurrentUIManager;

    public UIState CurrentUIState = UIState.Normal;

    public Camera MainCamera;
    public GameObject CurrentlySelectedTower;
    public GameObject Tower01;

    Vector3 latestObjectLocationInWorld = new Vector3();


    // Start is called before the first frame update
    void Start()
    {
        CurrentUIManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentUIState)
        {
            case UIState.Normal:
                break;
            case UIState.PlacingTower:
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) // if we're not moving, then don't change anything.
                {
                    MoveCurrentlySelectedTowerToMousePosition();
                }
                break;
        }
    }
    private void MoveCurrentlySelectedTowerToMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        //print("mouse position: " + mousePosition.ToString());

        #region NOTE: attempt 1: using Raycast isn't stable (flickers, presumably jumping between blocks and floor
        //RaycastHit[] rHit = Physics.RaycastAll(MainCamera.ScreenPointToRay(Input.mousePosition));

        //if (rHit != null && rHit.Length > 0)
        //{
        //    RaycastHit highestHit = GetHighestHitFromList(rHit);

        //    objectLocationInWorld = highestHit.transform.position;

        //    // future - get the proper height too.
        //    //print("target height: " + rHit.transform.localScale.y);
        //    objectLocationInWorld.y = 1 + (highestHit.transform.localScale.y / 2);
        //}
        #endregion

        #region Attempt 2: This doesn't fallow the environment plane, but the camera plane.  Closer.
        //mousePosition.z = 10; // guessing.
        //Vector3 mouseLocationInWorld = MainCamera.ScreenToWorldPoint(mousePosition);
        //print("mouse location in world" + mouseLocationInWorld.ToString());

        //// tweaking needed?
        //objectLocationInWorld = mouseLocationInWorld;
        #endregion

        #region Attempt 3: Combine knowledge of 1 & 2
        RaycastHit rHit;
        if(Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out rHit))
        {
            // if we hit our own canon, skip it.
            if (rHit.transform.gameObject.name.StartsWith("Sphere")) { return; }

            Transform t = rHit.transform;
            //print(t.gameObject.name + "'s top: " + (t.localScale.y + t.position.y));

            // if we hit the floor, skip doing anything. (floor: y = 1)?
            if ((t.localScale.y + t.position.y) > .03f)
            {
                latestObjectLocationInWorld = t.position;

                // adjust the height by the object hit
                //print("setting tower position.y: " + 1 + (t.localScale.y / 2));
                latestObjectLocationInWorld.y = 1 + (t.localScale.y / 2);
            }
        }


        #endregion

        CurrentlySelectedTower.transform.position = latestObjectLocationInWorld;
    }

    //private RaycastHit GetHighestHitFromList(RaycastHit[] rHit)
    //{
    //    RaycastHit result = rHit[0];

    //    if (rHit.Length > 1)
    //    {
    //        foreach(RaycastHit h in rHit)
    //        {
    //            if(h.transform.position.y > result.transform.position.y)
    //            {
    //                result = h;
    //            }
    //        }
    //    }

    //    return result;
    //}

    public void TowerClicked()
    {
        if (CurrentUIState == UIState.Normal)
        {
            CurrentUIState = UIState.PlacingTower;
            CurrentlySelectedTower = GetSelectedTower();
        }
        else
        {
            // we were already placing a tower, what should we do?  Change towers?
            CurrentlySelectedTower = GetSelectedTower();
        }
    }
    private GameObject GetSelectedTower()
    {
        // instantiate the right tower based on what button was pressed
        return Instantiate(Tower01);
    }



}
