using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;      // The Player
    [SerializeField] private Vector3 offset = new Vector3(0, 10, -5); // Position relative to player
    [SerializeField] private float smoothSpeed = 5f; // How "snappy" the camera is
    [SerializeField] private Vector3 mapSettings = new Vector3(0, 80, -120); // Map view position
    [SerializeField] private Vector3 LissanaGahaView = new Vector3(0, 80, -120);
    

    public enum CameraState
    {
        Start,
        Follow,
        MapView,

    }

    public CameraState currentState = CameraState.Follow;

    void Start()
    {
        currentState = CameraState.Start;

        transform.position = mapSettings;
        transform.rotation = Quaternion.Euler(35f, 0, 0); // Look straight down
        GetComponent<Camera>().fieldOfView = 15f; // Adjust FOV for
    }

    void LateUpdate()
    {
        if (target == null ) return;

        if(currentState == CameraState.MapView)
        {
            Vector3 mapPosition = mapSettings;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, mapPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            transform.rotation = Quaternion.Euler(35f, 0, 0); // Look straight down
            float fov = Mathf.Lerp(GetComponent<Camera>().fieldOfView, 15f, smoothSpeed * Time.deltaTime);
            GetComponent<Camera>().fieldOfView = fov; // Adjust FOV for better map view

        }
        else if(currentState == CameraState.Follow)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            float fov = Mathf.Lerp(GetComponent<Camera>().fieldOfView, 35f, smoothSpeed * Time.deltaTime);
            GetComponent<Camera>().fieldOfView = fov;

        }else if(currentState == CameraState.Start)
        {
            

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 1f * Time.deltaTime);
            transform.position = smoothedPosition;
            float fov = Mathf.Lerp(GetComponent<Camera>().fieldOfView, 35f, 1f * Time.deltaTime);
            GetComponent<Camera>().fieldOfView = fov;
            if (Vector3.Distance(transform.position, desiredPosition) < 0.1f)
            {
                currentState = CameraState.Follow;
            }
        }
    }

    public void mapview()
    {
        if(currentState == CameraState.Follow)
        {
            currentState = CameraState.MapView;
        }
        else
        {
            currentState = CameraState.Follow;
        }
       
    }
}