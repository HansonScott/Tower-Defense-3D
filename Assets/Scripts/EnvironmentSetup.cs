using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSetup : MonoBehaviour
{
    public GameObject eCube01;
    public GameObject eCube02;
    public GameObject eCube03;

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
        // choose a random edge to start and a random point near an edge to finish

        // based on level, add multiple paths

    }

    private void GenerateElevations()
    {
        float min = 1;
        float max = 3;
        for (int i = 0; i < 50; i++)
        {
            ElevationValues[i] = new float[50];

            for (int j = 0; j < 50; j++)
            {
                ElevationValues[i][j] = UnityEngine.Random.Range(min, max);
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
            default:
                break;
        }

        o.transform.position = new Vector3(x - 25, 1, z - 25);
    }
    private void CreatePath()
    {

    }
}
