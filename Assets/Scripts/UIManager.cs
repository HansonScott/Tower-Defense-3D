using System;
using TMPro;
using UnityEngine;

public enum UIState
{
    Normal,
    StartedPlacingTower,
    StillPlacingTower,
}

public class UIManager : MonoBehaviour
{
    public static UIManager CurrentUIManager;

    public UIState CurrentUIState = UIState.Normal;

    public Camera MainCamera;
    public GameObject CurrentlySelectedTower;
    public GameObject Tower01;
    public Material ValidTowerMaterial;
    public Material InvalidTowerMaterial;

    Vector3 latestObjectLocationInWorld = new Vector3();

    public GameObject EnemyInfoBox;
    public TMP_Text txtHP;
    public TMP_Text txtSpeed;
    public TMP_Text txtArmor;

    public TMP_Text txtHomeHP;

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
                if (Input.GetMouseButtonUp(0)) // if finishing left mouse click
                {
                    //print("mouse click captured");
                    HandleInfoClick();
                }
                break;
            case UIState.StartedPlacingTower:
            case UIState.StillPlacingTower:
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) // if we're not moving, then don't change anything.
                {
                    MoveCurrentlySelectedTowerToMousePosition();
                    ColorSelectedTowerBasedOnValidity();
                }
                if (Input.GetMouseButtonUp(0)) // if finishing left mouse click
                {
                    //print("mouse click captured");
                    HandlePlaceTowerClick();
                }


                break;
        }
    }

    private void HandlePlaceTowerClick()
    {
        // catch the first mouse click after the initial one, and don't place the tower on the same button click
        if(CurrentUIState == UIState.StartedPlacingTower)
        {
            CurrentUIState = UIState.StillPlacingTower;
            return;
        }

        if(EnvironmentSetup.IsValidTowerPlacement(CurrentlySelectedTower))
        {
            bool worked = EnvironmentSetup.PlaceNewTower(CurrentlySelectedTower);

            if(worked)
            {
                CurrentUIState = UIState.Normal;
            }
        }
        else
        {
            // notify?
        }
    }

    private void HandleInfoClick()
    {
        // user clicked the mouse, but is not placing a tower

        // see if they clicked on an object, if so, show that object's details.
        Vector3 mousePosition = Input.mousePosition;
        RaycastHit rHit;

        // only do this work if user clicked an object
        if (Physics.Raycast(MainCamera.ScreenPointToRay(mousePosition), out rHit))
        {
            // if we hit our own canon, skip it.
            //print("Mouse clicked on " + rHit.transform.gameObject.name);

            if (rHit.transform.gameObject.name.StartsWith("Enemy01"))
            {
                HandleEnemyInfoClick(rHit.transform.gameObject);
                EnemyInfoBox.SetActive(true);
            }
            else if (rHit.transform.gameObject.name.StartsWith("Tower01"))
            {
                HandleTowerInfoClick(rHit.transform.gameObject);
            }
            else // nothing worth getting info about
            {
                EnemyInfoBox.SetActive(false);
            }
        }
        else
        {
            EnemyInfoBox.SetActive(false);
        }

    }

    private void HandleEnemyInfoClick(GameObject gameObject)
    {
        if(gameObject != null &&
            gameObject.GetComponent<EnemyObject>() != null)
        {
            EnemyObject e = gameObject.GetComponent<EnemyObject>();
            txtHP.text = "HP: " + e.HPCurrent + " / " + e.HPMax;
            txtSpeed.text = "Speed: " + (e.SpeedCurrent * 100); // instead of a decimal...
            txtArmor.text = "Armor: " + e.ArmorCurrent;

            // list immunities / specials

            // color labels if status effects active, etc.


            // enable the enemy info box

        }
    }

    private void HandleTowerInfoClick(GameObject gameObject)
    {
        throw new NotImplementedException();
    }

    private void ColorSelectedTowerBasedOnValidity()
    {
        bool isValid = EnvironmentSetup.IsValidTowerPlacement(CurrentlySelectedTower);
        MeshRenderer[] mats = CurrentlySelectedTower.GetComponentsInChildren<MeshRenderer>();

        if (isValid)
        {
            // color tower valid colors
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].material.color = ValidTowerMaterial.color;
            }
        }
        else
        {
            // color tower invalid colors
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].material.color = InvalidTowerMaterial.color;
            }
        }
    }

    private void MoveCurrentlySelectedTowerToMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        RaycastHit rHit;
        // only do this work if we're over an environment element
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

        CurrentlySelectedTower.transform.position = latestObjectLocationInWorld;
    }

    // tower menu item clicked
    public void TowerClicked()
    {
        if (CurrentUIState == UIState.Normal)
        {
            CurrentUIState = UIState.StartedPlacingTower;
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
