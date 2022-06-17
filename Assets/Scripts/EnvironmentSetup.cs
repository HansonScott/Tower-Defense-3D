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


    public GameObject eCube01;
    public GameObject eCube02;
    public GameObject eCube03;

    public GameObject pathCube;

    public GameObject Spawn;
    public GameObject Home;

    UnityEngine.SceneManagement.Scene CurrentScene;

    public static int gridSize = 50;
    float[][] ElevationValues = new float[gridSize][];

    // Start is called before the first frame update
    void Start()
    {
        CreateEnvironment();
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

        // level-dependent?
        // based on level, add multiple paths
        // or choose a random point
        //Vector2 SpawnLocation = ChooseRandomStartingPosition();

        // hardcode for now
        #region default path
        ElevationValues[20][0] = (int)MapTokens.Path;
        ElevationValues[20][1] = (int)MapTokens.Path;
        ElevationValues[20][2] = 5;
        ElevationValues[20][3] = 5;
        ElevationValues[20][4] = 5;
        ElevationValues[20][5] = 5;
        ElevationValues[20][6] = 5;
        ElevationValues[20][7] = 5;
        ElevationValues[20][8] = 5;
        ElevationValues[20][9] = 5;
        ElevationValues[20][10] = 5;
        ElevationValues[20][11] = 5;
        ElevationValues[20][12] = 5;
        ElevationValues[20][13] = 5;
        ElevationValues[20][14] = 5;
        ElevationValues[20][15] = 5;
        ElevationValues[20][16] = 5;
        ElevationValues[20][17] = 5;
        ElevationValues[20][18] = 5;
        ElevationValues[20][19] = 5;
        ElevationValues[20][20] = 5;
        ElevationValues[20][21] = 5;
        ElevationValues[20][22] = 5;
        ElevationValues[20][23] = 5;
        ElevationValues[20][24] = 5;
        ElevationValues[20][25] = 5;
        ElevationValues[20][26] = 5;
        ElevationValues[20][27] = 5;
        ElevationValues[20][28] = 5;
        ElevationValues[20][29] = 5;
        ElevationValues[20][30] = 5;
        ElevationValues[20][31] = 5;
        ElevationValues[20][32] = 5;
        ElevationValues[20][33] = 5;
        ElevationValues[20][34] = 5;
        ElevationValues[20][35] = 5;
        ElevationValues[20][36] = 5;
        ElevationValues[20][37] = 5;
        ElevationValues[20][38] = 5;
        ElevationValues[20][39] = 5;
        ElevationValues[20][40] = 5;
        ElevationValues[20][41] = 5;
        ElevationValues[20][42] = 5;
        ElevationValues[20][43] = 5;
        ElevationValues[20][44] = 5;
        ElevationValues[20][45] = 5;
        ElevationValues[20][46] = 5;
        ElevationValues[20][47] = 5;
        ElevationValues[20][48] = 5;
        ElevationValues[20][49] = 5;
        #endregion

        // place spawn token
        GameObject s = Instantiate(Spawn);
        s.transform.position = new Vector3(20f - (gridSize / 2), 1.5f,-(gridSize / 2));

        // place home tokens
        GameObject h = Instantiate(Home);
        h.transform.position = new Vector3(20f - (gridSize / 2), 1.5f, 49 - (gridSize / 2));
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

    }
}
