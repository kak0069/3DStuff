using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float rotationSpeed = 90000000f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        // Rotate the camera based on user input
         if (Input.GetMouseButton(0))
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
            float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.Rotate(Vector3.up, horizontalRotation);
            transform.Rotate(Vector3.left, verticalRotation);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y,0);
        }

        // Move the camera based on user input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void TopDownView()
    {
        // Move the camera to the target position
        transform.position = new Vector3(0, 6, 0);

        // Rotate the camera to the target rotation
        transform.rotation = Quaternion.Euler(90,180,0);
    }

    public void StandardView()
    {
        // Move the camera to the target position
        transform.position = new Vector3(0, 2, 3);

        // Rotate the camera to the target rotation
        transform.rotation = Quaternion.Euler(33,180,0);
    }
}
