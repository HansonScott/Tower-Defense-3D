using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum StatusEffect
{
    chilled,
    confused,
    shocked,
    poisoned,
    aflame,
    weakened,
}

public class EnemyObject : MonoBehaviour
{
    [SerializeField]
    public int HPMax
    {
        get;
        set;
    }
    [SerializeField]
    public int HPCurrent
    {
        get;
        set;
    }
    [SerializeField]
    public float SpeedMax
    {
        get;
        set;
    }
    [SerializeField]
    public float SpeedCurrent
    {
        get;
        set;
    }
    [SerializeField]
    public int ArmorMax
    {
        get;
        set;
    }
    [SerializeField]
    public int ArmorCurrent
    {
        get;
        set;
    }

    public int DmgMax
    {
        get;
        set;
    }
    public int DmgCurrent
    {
        get;
        set;
    }


    // Start is called before the first frame update
    void Start()
    {
        // change later?
        DmgMax = 1;
        DmgCurrent = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// randomly rolls the three attributes adding up to the total power provided
    /// </summary>
    /// <param name="totalPower"></param>
    /// <returns></returns>
    public static Color GetRandomEnemyProperties(int totalPower)
    {
        int red = Mathf.Clamp(Random.Range(1, totalPower), 0, 255);
        int green = (int)Mathf.Clamp(Random.Range(1, totalPower - red), 0, 255);
        int blue = Mathf.Clamp(Random.Range(0, (totalPower - red - green)), 0, 255);

        //Color result = new Color(red, green, blue);
        //Color result = new Color((red / 255), (green / 255), (blue / 255)); // seems to be not the right scale, examples in code ref are between 0 and 1 for each...
        Color result = new Color((red / 25), (green / 25), (blue / 25)); // seems to be not the right scale, examples in code ref are between 0 and 1 for each...
        return result;
    }

    internal void ApplyPropertiesFromColor(Color c)
    {
        this.HPMax = (int)c.r;
        this.HPCurrent = HPMax;

        this.SpeedMax = 0.01f + (((c.g / 255) * 2) / 100); // percentage of max, weighted twice, then divided into thousandths, which is the movement norm.
        this.SpeedCurrent = this.SpeedMax;

        this.ArmorMax = (int)c.b;
        this.ArmorCurrent = this.ArmorMax;
    }
}
