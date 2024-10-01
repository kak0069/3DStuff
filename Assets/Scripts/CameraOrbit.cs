using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public float orbitSpeed = 25f; // Speed of orbiting
    public float distanceFromTarget = 5f; // Distance from the target object
    public Vector3 defaultPosition; // Default position of the camera
    public Vector3 defaultRotation; // Default rotation of the camera

    private List<Transform> bridgedWhiskers; // List of game objects with the tag "bridgedWhisker"
    private int currentTargetIndex = -1; // Current target index for orbiting
    private bool isOrbiting = false; // Is the camera currently orbiting

    void Start()
    {
        // Store the default position and rotation of the camera
        defaultPosition = transform.position;
        defaultRotation = transform.rotation.eulerAngles;

        // Initialize the list of bridged whiskers
        UpdateBridgedWhiskers();
    }

    void Update()
    {
        // Update the list of bridged whiskers every frame in case they are dynamically created or destroyed
        UpdateBridgedWhiskers();

        // Check for the up arrow key to start orbiting
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (bridgedWhiskers.Count > 0)
            {
                isOrbiting = true;
                currentTargetIndex = (currentTargetIndex + 1) % bridgedWhiskers.Count; // Cycle through the targets
            }
        }

        // Check for the down arrow key to reset the camera
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ResetCamera();
        }

        // Check for keys to change the distance from the target
        if (Input.GetKey(KeyCode.W)) // Increase distance
        {
            distanceFromTarget += Time.deltaTime * orbitSpeed; // Change speed if necessary
        }
        if (Input.GetKey(KeyCode.S)) // Decrease distance
        {
            distanceFromTarget = Mathf.Max(1f, distanceFromTarget - Time.deltaTime * orbitSpeed); // Prevent going below 1 unit
        }

        // Handle orbiting the current target
        if (isOrbiting && currentTargetIndex >= 0 && currentTargetIndex < bridgedWhiskers.Count)
        {
            OrbitAround(bridgedWhiskers[currentTargetIndex]);
        }
    }

    void UpdateBridgedWhiskers()
    {
        bridgedWhiskers = new List<Transform>(GameObject.FindGameObjectsWithTag("bridgedWhisker").Length);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("bridgedWhisker"))
        {
            bridgedWhiskers.Add(obj.transform);
        }
    }

    void OrbitAround(Transform target)
    {
        // Calculate the new position of the camera based on the target
        Vector3 direction = (transform.position - target.position).normalized;
        Quaternion rotation = Quaternion.Euler(0, orbitSpeed * Time.deltaTime, 0);
        Vector3 newPosition = target.position + rotation * direction * distanceFromTarget;

        // Update the camera's position and rotation
        transform.position = newPosition;
        transform.LookAt(target.position);
    }

    void ResetCamera()
    {
        transform.position = defaultPosition;
        transform.rotation = Quaternion.Euler(defaultRotation);
        isOrbiting = false;
        currentTargetIndex = -1; // Reset the target index
        distanceFromTarget = 5f; // Reset distance to default if desired
    }
}
