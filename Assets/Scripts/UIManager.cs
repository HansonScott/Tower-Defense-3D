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

    public GameManager CurrentGameManager;

    public Camera MainCamera;
    public TowerManager CurrentlySelectedTowerMenuItem;
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

    string DesiredInfoFormat = "N1";

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

        // connect our UI parts to the game object variable changes
        CurrentGameManager.OnCurrentMoneyChange += CurrentMoneyHandler;
        CurrentGameManager.CurrentWaveInfo.onCurrentWaveInfoChange += WaveInfoChangeHandler;
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

    private void WaveInfoChangeHandler(GameManager.WaveInfo info)
    {
        RefreshWaveLabel(info.CurrentWave, (info.EnemyCountInWave - info.RemainingEnemies), info.EnemyCountInWave);
    }
    internal void RefreshWaveLabel(int currentWave, int EnemyProgress, int EnemyTotal)
    {
        txtWave.text = "Wave: " + currentWave.ToString() + " (" + EnemyProgress.ToString() + " / " + EnemyTotal.ToString() + ")";
    }
    internal void RefreshScoreLabel(int currentScore)
    {
        txtScore.text = "Score: " + currentScore;
    }
    private void CurrentMoneyHandler(int val)
    {
        RefreshMoneyLabel(val);
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

        if(EnvironmentManager.IsValidTowerPlacement(CurrentlySelectedTowerMenuItem))
        {
            // NOTE: need to move this to a library or reference or factory, etc.
            AttackEffect ae = Instantiate<AttackEffect>(BulletTemplate.GetComponent<AttackEffect>());

            CurrentlySelectedTowerMenuItem.GetComponent<TowerManager>().AttackEffects.Add(ae);
            EnvironmentManager.CurrentEnvironment.AddTower(CurrentlySelectedTowerMenuItem);

            // turn the tower on
            CurrentlySelectedTowerMenuItem.CurrentState = TowerState.Seeking;
            CurrentlySelectedTowerMenuItem.CurrentlySelected = false;

            // and turn on the collider, now that it's in the environment
            CurrentlySelectedTowerMenuItem.transform.GetComponentInChildren<BoxCollider>().enabled = true;

            // and set our UI back to normal
            CurrentUIState = UIState.Normal;
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
        EnemyInfoBox.SetActive(false);
        SetSelectionForAllEnemies(false);

        TowerInfoBox.SetActive(false);
        SetSelectionForAllTowers(false);

        RaycastHit[] hits = Physics.RaycastAll(MainCamera.ScreenPointToRay(mousePosition));


        // only do this work if user clicked an object
        if (Physics.Raycast(MainCamera.ScreenPointToRay(mousePosition), out rHit))
        {
            if (rHit.transform.gameObject.name.StartsWith("Enemy01"))
            {
                HandleEnemyInfoClick(rHit.transform.gameObject.GetComponent<EnemyObject>());
            }
            else if (rHit.transform.gameObject.name.StartsWith("Tower01"))
            {
                HandleTowerInfoClick(rHit.transform.gameObject.GetComponent<TowerManager>());
            }
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
            tms[i].CurrentlySelected = selected;
        }
    }

    private void HandleEnemyInfoClick(EnemyObject o)
    {
        if (o != null)
        {
            o.CurrentlySelected = true;

            SourceForEnemyInfoBox = o;
            RefreshEnemyInfoBox();
            EnemyInfoBox.SetActive(true);
        }

        // enable the enemy info box

    }

    public void RefreshEnemyInfoBox()
    {
        if (SourceForEnemyInfoBox != null)
        {
            txtEnemyInfoHP.text = "HP: " + SourceForEnemyInfoBox.HPCurrent.ToString(DesiredInfoFormat) + " / " + SourceForEnemyInfoBox.HPMax.ToString(DesiredInfoFormat);
            txtEnemyInfoSpeed.text = "Speed: " + (SourceForEnemyInfoBox.SpeedCurrent * 100).ToString(DesiredInfoFormat); // instead of a decimal...
            txtEnemyInfoArmor.text = "Armor: " + SourceForEnemyInfoBox.ArmorCurrent.ToString(DesiredInfoFormat);

            // list immunities / specials
        }
        else
        {
            EnemyInfoBox.SetActive(false);
        }
    }

    private void HandleTowerInfoClick(TowerManager t)
    {
        if (t != null)
        {
            t.CurrentlySelected = true;

            SourceForTowerInfoBox = t;
            RefreshTowerInfoBox();
            TowerInfoBox.SetActive(true);
        }

        // enable the enemy info box

    }
    private void RefreshTowerInfoBox()
    {

        if (SourceForTowerInfoBox != null)
        {


            txtTowerInfoDmg.text = "Dmg: " + GetDamageFromTowerInfo(SourceForTowerInfoBox);
            txtTowerInfoRange.text = "Range: " + SourceForTowerInfoBox.RangeCurrent.ToString(DesiredInfoFormat);
            txtTowerInfoFireRate.text = "Fire Rate: " + SourceForTowerInfoBox.AttackDelayCurrent.ToString(DesiredInfoFormat);

            // list attack Effects

        }
        else
        {
            EnemyInfoBox.SetActive(false);
        }
    }

    private float GetDamageFromTowerInfo(TowerManager t)
    {
        float result = 0;
        foreach(AttackEffect ae in t.AttackEffects)
        {
            result = Math.Max(ae.Damage, result);
        }

        return result;
    }

    private void ColorSelectedTowerBasedOnValidity()
    {
        bool isValid = EnvironmentManager.IsValidTowerPlacement(CurrentlySelectedTowerMenuItem);
        MeshRenderer[] mats = CurrentlySelectedTowerMenuItem.GetComponentsInChildren<MeshRenderer>();

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

        CurrentlySelectedTowerMenuItem.transform.position = latestObjectLocationInWorld;
    }

    // tower menu item clicked
    public void TowerMenuItemClicked()
    {
        if (CurrentUIState == UIState.Normal)
        {
            CurrentUIState = UIState.StartedPlacingTower;
            CurrentlySelectedTowerMenuItem = CreateSelectedTower();
        }
        else
        {
            // change selection means dropping old one, if there was one
            if (CurrentlySelectedTowerMenuItem!= null)
            {
                Destroy(CurrentlySelectedTowerMenuItem.gameObject);
            }

            // Change to the new tower
            CurrentlySelectedTowerMenuItem = CreateSelectedTower();
        }
    }
    private TowerManager CreateSelectedTower()
    {
        // instantiate the right tower based on what button was pressed
        return Instantiate(Tower01Template.gameObject).GetComponent<TowerManager>();
    }

}
