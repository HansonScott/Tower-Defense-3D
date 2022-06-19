using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapTokens
{
    GroundElevation01 = 1,
    GroundElevation02 = 2,
    GroundElevation03 = 3,
    Path = 5,
    Home = 10

}
public class EnvironmentSetup : MonoBehaviour
{
    public static Vector3 CurrentSpawnPoint;
    public static Vector3 CurrentHomePoint;

    public GameObject eCube01;
    public GameObject eCube02;
    public GameObject eCube03;

    public GameObject pathCube;

    public GameObject Spawn;
    public GameObject Home;

    UnityEngine.SceneManagement.Scene CurrentScene;

    public static int gridSize = 50;
    float[][] ElevationValues = new float[gridSize][];
    internal static readonly int HomeNode = gridSize - 1; // -1 for the 0-based array

    static Vector3[] CurrentPath = new Vector3[gridSize];

    internal static Vector3 GetNextTarget(int NodeIndex)
    {
        if(NodeIndex >= CurrentPath.Length) { return new Vector3(); }

        return CurrentPath[NodeIndex];
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateEnvironment();
    }

    internal static bool IsValidTowerPlacement(GameObject tower)
    {
        // see if the x & z coordinates are on the path or not
        float x = tower.transform.position.x;
        float z = tower.transform.position.z;

        bool onX = false;
        bool onZ = false;

        #region Check Path
        foreach (Vector3 p in CurrentPath)
        {
            if(p.x == x) 
            { 
                onX = true; 
            }
            if(p.z == z) 
            { 
                onZ = true; 
            }

            if (onX && onZ) 
            { 
                return false; 
            }
        }
        #endregion

        // if we didn't match, then we're still good.
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateEnvironment()
    {
        CreateStartAndEndPoints();
        CreatePath();
        GenerateElevations();
        CreateElevationCubes();
    }
    private void CreateStartAndEndPoints()
    {
        // set up grid arrays
        for (int i = 0; i < gridSize; i++)
        {
            ElevationValues[i] = new float[gridSize];
        }

        // place spawn token
        GameObject s = Instantiate(Spawn);
        s.transform.position = new Vector3(20f - (gridSize / 2), 1.5f,-(gridSize / 2));
        CurrentSpawnPoint = s.transform.position;

        // place home tokens
        GameObject h = Instantiate(Home);
        h.transform.position = new Vector3(20f - (gridSize / 2), 1.5f, 49 - (gridSize / 2));
        CurrentHomePoint = h.transform.position;
    }

    private Vector2 ChooseRandomStartingPosition()
    {
        // choose a random edge to start and a random point near an edge to finish
        int Side = UnityEngine.Random.Range(1, 4);
        int Pos = UnityEngine.Random.Range(0, gridSize);

        int posX = 0;
        int posZ = 0;

        switch (Side)
        {
            case 1: // North
                posX = gridSize / 2;
                posZ = Pos;
                break;
            case 2: // South
                posX = -gridSize / 2;
                posZ = Pos;
                break;
            case 3: // East
                posX = Pos;
                posZ = gridSize / 2;
                break;
            case 4: // West
                posX = Pos;
                posZ = -gridSize / 2;
                break;
            default:
                break;
        }

        return new Vector2(posX, posZ);
    }

    private void GenerateElevations()
    {
        float min = 1;
        float max = 3;
        for (int i = 0; i < 50; i++)
        {
            //ElevationValues[i] = new float[50];

            for (int j = 0; j < 50; j++)
            {
                if(ElevationValues[i][j] != 5)
                {
                    ElevationValues[i][j] = UnityEngine.Random.Range(min, max);
                }
            }
        }
    }
    private void CreateElevationCubes()
    {
        for (int i = 0; i < ElevationValues.Length; i++)
        {
            for (int j = 0; j < ElevationValues[i].Length; j++)
            {
                // create an elevation cube at this location with this height value
                CreateElevationCube(i, j, ElevationValues[i][j]);
            }
        }
    }
    private void CreateElevationCube(float x, float z, float height)
    {
        height = (int)Math.Round((decimal)height, 0);

        GameObject o = null;

        switch(height)
        {
            case 01:
                o = Instantiate(eCube01);
                break;
            case 02:
                o = Instantiate(eCube02);
                break;
            case 03:
                o = Instantiate(eCube03);
                break;
            case 05:
                o = Instantiate(pathCube);
                break;
            default:
                break;
        }

        o.transform.position = new Vector3(x - 25, 1, z - 25);
    }
    private void CreatePath()
    {
        // level-dependent?
        // based on level, add multiple paths
        // or choose a random point
        //Vector2 SpawnLocation = ChooseRandomStartingPosition();


        // hardcode for now, a straight line
        #region default path
        for (int i = 0; i < gridSize; i++)
        {
            CurrentPath[i] = new Vector3(20 - (gridSize / 2), (int)MapTokens.Path, i - (gridSize / 2));
            ElevationValues[20][i] = (float)MapTokens.Path;
        }
        #endregion
    }
}
