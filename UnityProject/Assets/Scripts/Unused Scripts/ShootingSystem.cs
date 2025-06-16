using UnityEngine;

public class ShootingSystem : MonoBehaviour
{

    [SerializeField]
    private float _maxLength; // the length of a light
    private Ray _ray; // stores the information about the ray

    private RaycastHit _hitData; // stores the information about the hit object

    private LineRenderer _lineRenderer; // the line renderer component

    public Transform cameraOrientation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // If you click the mouse button, the player witl shoot raycast
        if (Input.GetMouseButtonDown(0))
        {
            FireRay();
        }
        // if I release the mouse button, the player will stop shooting raycast
        if (Input.GetMouseButtonUp(0))
        {
            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.zero);
        }
    }
    public void FireRay()
    {
        // create our ray
        // Forward -> z axis
        _ray = new Ray(
            cameraOrientation.position, // the origin of the ray is the position of the camera
            cameraOrientation.forward // the direction of the ray is the forward direction of the camera
        );

        // Visualize Ray in the debug
        Debug.DrawRay(_ray.origin, _ray.direction * 10, Color.red);

        // check if the ray hits something
        if (Physics.Raycast(_ray, out _hitData, _maxLength))
        {
            // If the ray hits something, set the end position of the line renderer to the hit point
            _lineRenderer.SetPosition(0, _ray.origin);
            _lineRenderer.SetPosition(1, _hitData.point);

            // Log the name of the object hit by the ray
            Debug.Log("Hit: " + _hitData.collider.name);
        }
        else
        {
            // If the ray does not hit anything, set the end position of the line renderer to the maximum length
            _lineRenderer.SetPosition(0, _ray.origin);
            _lineRenderer.SetPosition(1, _ray.origin + _ray.direction * _maxLength);
        }
    }
    

}
