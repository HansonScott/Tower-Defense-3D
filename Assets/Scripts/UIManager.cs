using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject Tower01Template;
    public GameObject BulletTemplate;

    public Material ValidTowerMaterial;
    public Material InvalidTowerMaterial;
    public Material TowerCanonInteriorMaterial;

    Vector3 latestObjectLocationInWorld = new Vector3();

    public EnemyObject EnemySourceForInfoBox;
    public GameObject EnemyInfoBox;
    public TMP_Text txtHP;
    public TMP_Text txtSpeed;
    public TMP_Text txtArmor;

    public TMP_Text txtHomeHP;
    public TMP_Text txtWave;
    public TMP_Text txtScore;
    public TMP_Text txtMoney;

    public Button btnWave;
    public Button btnCanon;

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

    internal void RefreshWaveLabel(int currentWave)
    {
        txtWave.text = "Wave: " + currentWave;
    }
    internal void RefreshScoreLabel(int currentScore)
    {
        txtScore.text = "Score: " + currentScore;
    }
    internal void RefreshMoneyLabel(int currentMoney)
    {
        txtMoney.text = "$" + currentMoney;
    }


    private void HandlePlaceTowerClick()
    {
        // catch the first mouse click after the initial one, and don't place the tower on the same button click
        if(CurrentUIState == UIState.StartedPlacingTower)
        {
            CurrentUIState = UIState.StillPlacingTower;
            return;
        }

        if(EnvironmentManager.IsValidTowerPlacement(CurrentlySelectedTower))
        {
            // NOTE: need to move this to a library or reference or factory, etc.
            AttackEffect ae = Instantiate<AttackEffect>(BulletTemplate.GetComponent<AttackEffect>());
            CurrentlySelectedTower.GetComponent<TowerManager>().AttackEffects.Add(ae);

            bool worked = EnvironmentManager.CurrentEnvironment.PlaceNewTower(CurrentlySelectedTower);

            if(worked)
            {
                // remove the template
                Destroy(CurrentlySelectedTower);

                // and set our UI back to normal
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

    private void HandleEnemyInfoClick(GameObject o)
    {
        if (o != null &&
            o.GetComponent<EnemyObject>() != null)
        {
            EnemySourceForInfoBox = o.GetComponent<EnemyObject>();
            RefreshEnemyInfoBox();
        }

        // enable the enemy info box

    }

    public void RefreshEnemyInfoBox()
    {
        if(EnemySourceForInfoBox != null)
        {
            string DesiredFormat = "N1";
            txtHP.text = "HP: " + EnemySourceForInfoBox.HPCurrent.ToString(DesiredFormat) + " / " + EnemySourceForInfoBox.HPMax.ToString(DesiredFormat);
            txtSpeed.text = "Speed: " + (EnemySourceForInfoBox.SpeedCurrent * 100).ToString(DesiredFormat); // instead of a decimal...
            txtArmor.text = "Armor: " + EnemySourceForInfoBox.ArmorCurrent.ToString(DesiredFormat);

            // list immunities / specials

            // color labels if status effects active, etc.

        }
        else
        {
            EnemyInfoBox.SetActive(false);
            //txtHP.text = "HP: X / Y";
            //txtSpeed.text = "Speed: S";
            //txtArmor.text = "Armor: A";

            // list immunities / specials

            // color labels if status effects active, etc.

        }
    }

    private void HandleTowerInfoClick(GameObject gameObject)
    {
        throw new NotImplementedException();
    }

    private void ColorSelectedTowerBasedOnValidity()
    {
        bool isValid = EnvironmentManager.IsValidTowerPlacement(CurrentlySelectedTower);
        MeshRenderer[] mats = CurrentlySelectedTower.GetComponentsInChildren<MeshRenderer>();

        if (isValid)
        {
            // color tower valid colors
            for (int i = 0; i < mats.Length; i++)
            {
                if(mats[i].name != "CanonInterior" &&
                    mats[i].name != "SelectionPlane")
                {
                    mats[i].material.color = ValidTowerMaterial.color;
                }
            }
        }
        else
        {
            // color tower invalid colors
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i].name != "CanonInterior" &&
                    mats[i].name != "SelectionPlane")
                {
                    mats[i].material.color = InvalidTowerMaterial.color;
                }
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
            else if (rHit.transform.gameObject.name.StartsWith("Selection")) { return; }

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
        return Instantiate(Tower01Template);
    }

}
