using System;
using System.Collections.Generic;
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
    public TowerManager CurrentlySelectedTowerTemplate;
    public GameObject Tower01Template;
    public GameObject BulletTemplate;

    public Material ValidTowerMaterial;
    public Material InvalidTowerMaterial;
    public Material TowerCanonInteriorMaterial;

    Vector3 latestObjectLocationInWorld = new Vector3();

    public TMP_Text txtHomeHP;
    public TMP_Text txtWave;
    public TMP_Text txtScore;
    public TMP_Text txtMoney;
    public Button btnWave;
    public Button btnCanon;

    public EnemyObject SourceForEnemyInfoBox;
    public GameObject EnemyInfoBox;
    public TMP_Text txtEnemyInfoHP;
    public TMP_Text txtEnemyInfoSpeed;
    public TMP_Text txtEnemyInfoArmor;

    public TowerManager SourceForTowerInfoBox;
    public GameObject TowerInfoBox;
    public TMP_Text txtTowerInfoDmg;
    public TMP_Text txtTowerInfoRange;
    public TMP_Text txtTowerInfoFireRate;

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
                    HandleMouseClickNonPlacing();
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

        if(EnvironmentManager.IsValidTowerPlacement(CurrentlySelectedTowerTemplate))
        {
            // NOTE: need to move this to a library or reference or factory, etc.
            AttackEffect ae = Instantiate<AttackEffect>(BulletTemplate.GetComponent<AttackEffect>());
            CurrentlySelectedTowerTemplate.GetComponent<TowerManager>().AttackEffects.Add(ae);

            bool worked = EnvironmentManager.CurrentEnvironment.PlaceNewTower(CurrentlySelectedTowerTemplate);

            if(worked)
            {
                // remove the template
                Destroy(CurrentlySelectedTowerTemplate);

                // and set our UI back to normal
                CurrentUIState = UIState.Normal;
            }
        }
        else
        {
            // notify?
        }
    }

    private void HandleMouseClickNonPlacing()
    {
        // see if they clicked on an object, if so, show that object's details.
        Vector3 mousePosition = Input.mousePosition;
        RaycastHit rHit;

        // unselect all towers and enemies, regardless of what we clicked
        SetSelectionForAllEnemies(false);
        SetSelectionForAllTowers(false);

        // only do this work if user clicked an object
        if (Physics.Raycast(MainCamera.ScreenPointToRay(mousePosition), out rHit))
        {
            if (rHit.transform.gameObject.name.StartsWith("Enemy01"))
            {
                HandleEnemyInfoClick(rHit.transform.gameObject);
                TowerInfoBox.SetActive(false);
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
            TowerInfoBox.SetActive(false);
            EnemyInfoBox.SetActive(false);
        }

    }

    private void SetSelectionForAllEnemies(bool selected)
    {
        List<EnemyObject> es = EnvironmentManager.CurrentEnvironment.GetAllEnemies();
        for(int i = 0; i < es.Count; i++)
        {
            es[i].CurrentlySelected = selected;
        }
    }

    private void SetSelectionForAllTowers(bool selected)
    {
        List<TowerManager> tms = EnvironmentManager.CurrentEnvironment.GetAllTowers();
        for (int i = 0; i < tms.Count; i++)
        {
            tms[i].gameObject.SetActive(selected);
        }
    }

    private void HandleEnemyInfoClick(GameObject o)
    {
        // select this enemy
        o.GetComponent<EnemyObject>().CurrentlySelected = true;
        EnemyInfoBox.SetActive(true);

        if (o != null &&
            o.GetComponent<EnemyObject>() != null)
        {
            SourceForEnemyInfoBox = o.GetComponent<EnemyObject>();
            RefreshEnemyInfoBox();
            EnemyInfoBox.SetActive(true);
        }

        // enable the enemy info box

    }

    public void RefreshEnemyInfoBox()
    {
        if(SourceForEnemyInfoBox != null)
        {
            string DesiredFormat = "N1";
            txtEnemyInfoHP.text = "HP: " + SourceForEnemyInfoBox.HPCurrent.ToString(DesiredFormat) + " / " + SourceForEnemyInfoBox.HPMax.ToString(DesiredFormat);
            txtEnemyInfoSpeed.text = "Speed: " + (SourceForEnemyInfoBox.SpeedCurrent * 100).ToString(DesiredFormat); // instead of a decimal...
            txtEnemyInfoArmor.text = "Armor: " + SourceForEnemyInfoBox.ArmorCurrent.ToString(DesiredFormat);

            // list immunities / specials
        }
        else
        {
            EnemyInfoBox.SetActive(false);
        }
    }

    private void HandleTowerInfoClick(GameObject gameObject)
    {
        if (SourceForEnemyInfoBox != null)
        {
            string DesiredFormat = "N1";
            txtEnemyInfoHP.text = "HP: " + SourceForEnemyInfoBox.HPCurrent.ToString(DesiredFormat) + " / " + SourceForEnemyInfoBox.HPMax.ToString(DesiredFormat);
            txtEnemyInfoSpeed.text = "Speed: " + (SourceForEnemyInfoBox.SpeedCurrent * 100).ToString(DesiredFormat); // instead of a decimal...
            txtEnemyInfoArmor.text = "Armor: " + SourceForEnemyInfoBox.ArmorCurrent.ToString(DesiredFormat);

            // list immunities / specials
        }
        else
        {
            EnemyInfoBox.SetActive(false);
        }
    }

    private void ColorSelectedTowerBasedOnValidity()
    {
        bool isValid = EnvironmentManager.IsValidTowerPlacement(CurrentlySelectedTowerTemplate);
        MeshRenderer[] mats = CurrentlySelectedTowerTemplate.GetComponentsInChildren<MeshRenderer>();

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

        CurrentlySelectedTowerTemplate.transform.position = latestObjectLocationInWorld;
    }

    // tower menu item clicked
    public void TowerClicked()
    {
        if (CurrentUIState == UIState.Normal)
        {
            CurrentUIState = UIState.StartedPlacingTower;
            CurrentlySelectedTowerTemplate = CreateSelectedTower();
        }
        else
        {
            // we were already placing a tower, what should we do?  Change towers?
            CurrentlySelectedTowerTemplate = CreateSelectedTower();
        }
    }
    private TowerManager CreateSelectedTower()
    {
        // instantiate the right tower based on what button was pressed
        return Instantiate(Tower01Template.gameObject).GetComponent<TowerManager>();
    }

}
