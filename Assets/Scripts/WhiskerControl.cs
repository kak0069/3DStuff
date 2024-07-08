using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Data;
using System.Linq;
using System.IO;
 // Add this namespace for LINQ functionality

public class WhiskerControl : MonoBehaviour
{
    public Dropdown gravity;
    private ConstantForce cForce;
    private Vector3 forceDirection;
    public int selectedIndex = 0;
    public Material targetMaterial;
    public int bridges = 0;
    bool confirmGravity; // used to tell program if gravity has been added or not.
    private List<WhiskerData> bridgedWhiskers = new List<WhiskerData>();
    public GameObject UIObject;
    public UIScript uiScript;
 
    
    // Start is called before the first frame update
    void Start()
    {
        uiScript = UIObject.GetComponent<UIScript>();
        //Initialize the dictionary for the conductor pad NEW NEW NEW
        if (!bridgesPerConductor.ContainsKey(gameObject.name))
        {
            bridgesPerConductor[gameObject.name] = 0;
        }
    }

    // Update is called once per frame
    public float detectionRadius = 0.5f; // Adjust this value based on your requirements
    public float rayDistance = 1.0f;
    public LayerMask conductorLayer; // Set this layer to the layer of your conductor objects

    void Update()
    {
        if(confirmGravity)
        {
            GetGravitySelection(gravity.value);
        }
    }
 
    //gravityStuff
public void ConfirmButtonPressed()
{
    GetGravitySelection(gravity.value);
    confirmGravity = true;
    UIObject.GetComponent<UIScript>().startSim = true;//NEW
}

public void GetGravitySelection(int val)
{
    ApplyGravity(val); //selectedIndex
}

public void ApplyGravity(int val)
{
    GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");

    Vector3 forceDirection = Vector3.zero;

    switch (val)
    {
        case 0:
            forceDirection = new Vector3(0, -10, 0);
            break;
        case 1:
            forceDirection = new Vector3(0, -2, 0);
            break;
        case 2:
            forceDirection = new Vector3(0, -4, 0);
            break;
    }

    foreach (GameObject obj in objectsWithTag)
    {
        ConstantForce cForce = obj.GetComponent<ConstantForce>();
        if (cForce == null)
        {
            // If the ConstantForce component is not found, add it
            cForce = obj.AddComponent<ConstantForce>();
        }
        cForce.force = forceDirection;
    }
}
    public void ResetGravity()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");

        foreach (GameObject obj in objectsWithTag)
        {
            cForce = GetComponent<ConstantForce>();
            forceDirection = new Vector3(0,0,0);
            cForce.force = forceDirection;
        }
        confirmGravity = false;
    }

    public bool haveLoggedConnection;
    public bool haveMadeOneConnection;
    public string firstConnection;
    public bool haveMadeSecondConnection;
    public string secondConnection;
    public int connectionsMade;
    public static Dictionary<string, int> bridgesPerConductor = new Dictionary<string, int>(); 
    private Renderer objectRenderer; // for highlighting
    public Color bridgeDetectedColor = Color.red; // for highlighting
    public Color defaultColor = Color.white; // for highlighting
    public HashSet<string> currentConnections = new HashSet<string>();
    public Color[] colors;
    private Dictionary<GameObject, int> triggerInteractionCounts = new Dictionary<GameObject, int>();
    

    // BRIDGING DETECTION ORIGINAL
    private void  OnTriggerStay(Collider trigger) 
    {
        if (trigger.gameObject.CompareTag("ConductorTrigger")) 
        {
            // Add the conductor to the set of current connections
            currentConnections.Add(trigger.gameObject.name);
           
            // Check if we have two or more connections
            if (currentConnections.Count >= 2)
            {
                // Only increment connectionsMade if this is the first time we're logging the connection
                if (!haveLoggedConnection)
                {
                    objectRenderer = GetComponent<Renderer>();
                    print("TRIGGER");
                    bridgesPerConductor[gameObject.name]++;
                    UIObject.GetComponent<UIScript>().bridgesDetected++;
                    UIObject.GetComponent<UIScript>().bridgesPerRun++;
                    objectRenderer.material.color = bridgeDetectedColor;
                    haveLoggedConnection = true;

                    TrackBridgedWhiskers(gameObject);
                    
                }
            }
        }
    }

     private void OnTriggerExit(Collider trigger)//OnCollisionExit(Collision collision)//OnTriggerExit(Collider trigger) 
    {
        if (trigger.gameObject.CompareTag("ConductorTrigger")) //collision
        {
            // Remove the conductor from the set of current connections
            currentConnections.Remove(trigger.gameObject.name); //collision

            // Reset state if there are fewer than 2 connections
            if (currentConnections.Count < 2 && haveLoggedConnection)
            {
                ResetConnectionState();
            }
        }
    }
    
    private void ResetConnectionState()
    {
        currentConnections.Clear();
        haveLoggedConnection = false;
        bridgesPerConductor[gameObject.name]--; //NEW
        UIObject.GetComponent<UIScript>().bridgesDetected--;
        UIObject.GetComponent<UIScript>().bridgesPerRun--;
        objectRenderer.material.color = defaultColor;
    }

     public void TrackBridgedWhiskers(GameObject whisker)
    {
        Vector3 scale = whisker.transform.localScale;
        float length = scale.y*2; // Y axis is the length
        float diameter = (scale.x + scale.z) / 2; // Average of X and Z axes for diameter

        // Access material properties based on currentMaterial from UIScript
        UIScript.MaterialProperties currentProps = uiScript.materialProperties[uiScript.currentMaterial];

        float resistance = CalculateResistance(length, diameter, currentProps);

        WhiskerData data = new WhiskerData(length*1000, diameter*1000, resistance);
        bridgedWhiskers.Add(data);

        SaveBridgedWhiskerData(data);
    }
    private float CalculateResistance(float length, float diameter, UIScript.MaterialProperties materialProps)
    {
        // Implement your resistance calculation here.
        // This is a placeholder formula. Replace with your actual calculation.
        float area = Mathf.PI * Mathf.Pow((diameter*1000)/2, 2);
        return materialProps.resistivity * (length*1000)/area;
    }

    private void SaveBridgedWhiskerData(WhiskerData data)
    {
        string directoryPath = @"D:/Unity";
        string filePath = Path.Combine(directoryPath, "bridged_whisker_data.csv");

        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{data.Length},{data.Diameter},{data.Resistance}");
            }
            Debug.Log($"Bridged whisker data saved successfully to {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save bridged whisker data: {ex.Message}");
        }
    }

    public class WhiskerData
    {
        public float Length { get; set; }
        public float Diameter { get; set; }
        public float Resistance { get; set; }

        public WhiskerData(float length, float diameter, float resistance)
        {
            Length = length;
            Diameter = diameter;
            Resistance = resistance;
        }
    }
}
