using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera MainCamera;
    public float CameraSpeed = 0.1f;

    public float CamaraZoomDefault = 60;
    public float CameraZoomSpeed = 10f;
    public bool CamaraZoomInvert = true;

    private float CameraZoomMax = 65;
    //private float CameraZoomMin = 1;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera.fieldOfView = CamaraZoomDefault;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForCamaraPan();
        CheckForCamaraZoom();
    }
    private void CheckForCamaraPan()
    {
        bool IsLeftPressed = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        bool IsRightPressed = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool IsUpPressed = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool IsDownPressed = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        float currentX = MainCamera.transform.position.x;
        float currentY = MainCamera.transform.position.y;
        float currentZ = MainCamera.transform.position.z;

        float relativeCameraSpeed = CameraSpeed * (MainCamera.fieldOfView / CameraZoomMax);

        if (IsLeftPressed)
        {
            MainCamera.transform.position = new Vector3(currentX - relativeCameraSpeed, currentY, currentZ);
        }
        else if (IsRightPressed)
        {
            MainCamera.transform.position = new Vector3(currentX + relativeCameraSpeed, currentY, currentZ);
        }
        else if (IsUpPressed)
        {
            MainCamera.transform.position = new Vector3(currentX, currentY, currentZ + relativeCameraSpeed);
        }
        else if (IsDownPressed)
        {
            MainCamera.transform.position = new Vector3(currentX, currentY, currentZ - relativeCameraSpeed);
        }
    }

    private void CheckForCamaraZoom()
    {
        float val = Input.GetAxis("Mouse ScrollWheel");
        float adj = 0;
        if(val != 0)
        {
            // adjust based on speed
            adj = (val * CameraZoomSpeed);

            // flip for invert
            if (CamaraZoomInvert) { adj = -adj; }

            // set new field of view
            MainCamera.fieldOfView += adj;

            // check FOV within bounds?

        }
    }
}
