using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardDetector : MonoBehaviour
{
    public GameObject detectedBoard { get; private set; } // Stores the detected board

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        // Find the imported model tagged as "CircuitBoard"
        GameObject importedModel = GameObject.FindWithTag("CircuitBoard");

        if (importedModel != null)
        {
            // Immediately set BasePlane as the parent of the imported model
            GameObject basePlane = GameObject.Find("BasePlane");
            if (basePlane != null)
            {
                importedModel.transform.SetParent(basePlane.transform);
                Debug.Log("CircuitBoard parent set to BasePlane.");
            }
            else
            {
                Debug.LogError("BasePlane not found.");
            }

            // Now find and process the detected board
            detectedBoard = FindBoard(importedModel);

            if (detectedBoard != null)
            {
                Debug.Log("Board detected: " + detectedBoard.name);
                Renderer boardRenderer = detectedBoard.GetComponent<Renderer>();
                // Uncomment if you need to log bounds
                // Debug.Log("Detected Board Bounds: " + boardRenderer.bounds);

                GameObject whiskerSpawnPoint = GameObject.Find("WhiskerSpawnPoint");
                if (whiskerSpawnPoint != null)
                {
                    whiskerSpawnPoint.transform.SetParent(detectedBoard.transform);
                }
                else
                {
                    Debug.LogError("WhiskerSpawnPoint not found.");
                }
            }
            else
            {
                Debug.LogError("Board could not be detected.");
            }
        }
        else
        {
            Debug.LogError("No GameObject with tag 'CircuitBoard' found.");
        }
    }

    // Method to find the board based on largest object in heirachy
    private GameObject FindBoard(GameObject rootObject)
    {
        GameObject largestFlatObject = null;
        float largestSurfaceArea = 0;

        // Iterate over all the children of the imported model (all the components)
        foreach (Renderer renderer in rootObject.GetComponentsInChildren<Renderer>())
        {
            Bounds bounds = renderer.bounds;

            //calculates the surface area
            float surfaceArea = bounds.size.x * bounds.size.z;

            //Debug.Log($"Checking Object: {renderer.gameObject.name}, Surface Area: {surfaceArea}, Thickness (Y): {bounds.size.y}, Center Y: {bounds.center.y}");
            //check if the object is realtively flat by comparing its height (Y) with its surface area
            // Example: thickness should be relatively small compared to surface area
            if (surfaceArea > largestSurfaceArea && bounds.size.y < (0.3f * Mathf.Min(bounds.size.x, bounds.size.z)))
            {
                largestSurfaceArea = surfaceArea;
                largestFlatObject = renderer.gameObject;

                //Debug.Log($"Potential Board Detected: {largestFlatObject.name}, Surface Area; {surfaceArea}, thickness (Y): {bounds.size.y}");
            }
        }

        if (largestFlatObject != null)
        {
            //Debug.Log("Largest Flat Object Detected: " + largestFlatObject.name);
        }
        else
        {
            Debug.LogWarning("No flat object detected as the board.");
        }
        return largestFlatObject;
    }
}
