using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject Camera;

    public float CameraSpeed = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool IsLeftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool IsRightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool IsUpPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool IsDownPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        float currentX = Camera.transform.position.x;
        float currentY = Camera.transform.position.y;
        float currentZ = Camera.transform.position.z;

        if (IsLeftPressed)
        {
            Camera.transform.position = new Vector3(currentX - CameraSpeed, currentY, currentZ);
        }
        else if (IsRightPressed)
        {
            Camera.transform.position = new Vector3(currentX + CameraSpeed, currentY, currentZ);
        }
        else if (IsUpPressed)
        {
            Camera.transform.position = new Vector3(currentX, currentY, currentZ + CameraSpeed);
        }
        else if (IsDownPressed)
        {
            Camera.transform.position = new Vector3(currentX, currentY, currentZ - CameraSpeed);
        }
    }
}
