using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera MainCamera;

    public float CameraSpeed = 0.2f;

    public float CameraZoomDefault = 60;
    public float CameraZoomSpeed = 15f;
    public bool CameraZoomInvert = true;
    public float CameraZoomShift = 1f;

    private float CameraZoomMin = 6; // a few squares
    private float CameraZoomMax = 80; // entire map

    // Start is called before the first frame update
    void Start()
    {
        MainCamera.fieldOfView = CameraZoomDefault;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForCameraPan();
        CheckForCameraZoom();
    }

    private void CheckForCameraPan()
    {
        bool IsShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        bool IsLeftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool IsRightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool IsUpPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool IsDownPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        float currentX = MainCamera.transform.position.x;
        float currentY = MainCamera.transform.position.y;
        float currentZ = MainCamera.transform.position.z;

        float relativeCameraSpeed = CameraSpeed * (MainCamera.fieldOfView / CameraZoomMax);

        float newX = currentX;
        float newZ = currentZ;

        if (!IsShiftPressed) // only pan on non-shifted directions
        {
            if (IsLeftPressed)
            {
                newX = currentX - relativeCameraSpeed;
            }
            if (IsRightPressed)
            {
                newX = currentX + relativeCameraSpeed;
            }
            if (IsUpPressed)
            {
                newZ = currentZ + relativeCameraSpeed;
            }
            if (IsDownPressed)
            {
                newZ = currentZ - relativeCameraSpeed;
            }

            MainCamera.transform.position = new Vector3(newX, currentY, newZ);
        }
    }

    private void CheckForCameraZoom()
    {
        float adj = 0;

        float val = Input.GetAxis("Mouse ScrollWheel");
        bool IsUpPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool IsDownPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        bool IsShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // NOTE: Wheel works for desktop, but not for WebGL publishing
        if (val != 0)
        {
            // adjust based on speed
            adj = val * CameraZoomSpeed;
        }
        else if (IsShiftPressed && (IsUpPressed || IsDownPressed))
        {
            val = CameraZoomShift * CameraSpeed; // NOTE: use speed and not zoom speed because wheel is different than keys

            // flip for up vs down...
            if (IsDownPressed) { val = -val; }

            adj = val;
        }
        else // neither wheel or shift, so do nothing
        {
            return;
        }

        // if either method changed, then...

        // flip for invert
        if (CameraZoomInvert) { adj = -adj; }

        // set new field of view
        MainCamera.fieldOfView += adj;

        //check FOV within bounds
        MainCamera.fieldOfView = Mathf.Max(CameraZoomMin, Mathf.Min(MainCamera.fieldOfView, CameraZoomMax));
    }
}
