using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;
using System.IO;
using System.IO.Enumeration;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIScriptFxn : MonoBehaviour
{
    public TMP_InputField lengthMu;
    public TMP_InputField widthMu;
    public TMP_InputField lengthSigma;
    public TMP_InputField widthSigma;
    public TMP_InputField numWhiskers;
    public TMP_InputField xCoord;
    public TMP_InputField yCoord;
    public TMP_InputField zCoord;
    public TMP_InputField numIterations;
    public TextMeshProUGUI whiskerMat;
    public TextMeshProUGUI distributionSelect;
    public TextMeshProUGUI gravSelect;
    public TMP_InputField shockAmp;
    public TMP_InputField shockFreq;
    public TMP_InputField shockDur;
    public TMP_InputField vibrAmp;
    public TMP_InputField vibrFreq;
    public TMP_InputField vibrDur;
    public GameObject whisker;
    public TextMeshProUGUI totalBridges;
    public int bridgesDetected;
    public TextMeshProUGUI bridgesEachRun;
    public int bridgesPerRun;

    // for monte carlo stuff
    public float simtimeElapsed;
    public bool startSim;
    public int simIntComplete;
    public float simTimeThresh;
    public TMP_InputField totalRuns;
    public float moveSpeed = 5f;
    public DistributionType distributionType = DistributionType.Lognormal;
    private List<float> lengths;
    private List<float> widths;

    void Start()
    {
        lengths = new List<float>();
        widths = new List<float>();
    }

    void Update()
    {
        totalBridges.text = "Total Bridges: " + bridgesDetected.ToString();
        bridgesEachRun.text = "This Run: " + bridgesPerRun.ToString();
        print(bridgesDetected);

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        simtimeElapsed += Time.deltaTime;

        if (startSim)
        {
            if (simIntComplete < Convert.ToInt32(totalRuns.text))
            {
                if (simtimeElapsed > simTimeThresh)
                {
                    ReloadWhiskersButton();

                    simIntComplete += 1;
                    simtimeElapsed = 0f;
                }
            }

            if (simIntComplete == Convert.ToInt32(totalRuns.text))
            {
                startSim = false;
            }
        }
    }

    private float GenerateLogNormalValue(float mu_log, float sigma_log)
    {
        float normalVal = RandomFromDistribution.RandomNormalDistribution(mu_log, sigma_log);
        float logNormalVal = Mathf.Exp(normalVal);
        return logNormalVal;
    }

    private float GenerateNormalValue(float mu, float sigma)
    {
        return RandomFromDistribution.RandomNormalDistribution(mu, sigma);
    }

    public float LengthDistributionGenerate()
    {
        float mu_log = float.Parse(lengthMu.text);
        float sigma_log = float.Parse(lengthSigma.text);
        float lengthVal;

        if (distributionType == DistributionType.Lognormal)
        {
            lengthVal = GenerateLogNormalValue(mu_log, sigma_log);
        }
        else
        {
            lengthVal = GenerateNormalValue(mu_log, sigma_log);
        }

        return lengthVal;
    }

    public float WidthDistributionGenerate()
    {
        float mu_log = float.Parse(widthMu.text);
        float sigma_log = float.Parse(widthSigma.text);
        float widthVal;

        if (distributionType == DistributionType.Lognormal)
        {
            widthVal = GenerateLogNormalValue(mu_log, sigma_log);
        }
        else
        {
            widthVal = GenerateNormalValue(mu_log, sigma_log);
        }

        return widthVal;
    }

    public void MakeWhiskerButton()
    {
        for (int i = 0; i < Convert.ToInt32(numWhiskers.text); i++)
        {
            float diameter = WidthDistributionGenerate();
            float length = LengthDistributionGenerate();
            float spawnPointX = UnityEngine.Random.Range(-float.Parse(xCoord.text), float.Parse(xCoord.text));
            float spawnPointY = UnityEngine.Random.Range(1, Convert.ToInt32(yCoord.text));
            float spawnPointZ = UnityEngine.Random.Range(-float.Parse(zCoord.text), float.Parse(zCoord.text));

            Vector3 spawnPos = new Vector3(spawnPointX, spawnPointY, spawnPointZ);

            GameObject whiskerClone = Instantiate(whisker, spawnPos, Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
            whiskerClone.tag = "whiskerClone";

            whiskerClone.transform.localScale = new Vector3(diameter, length, diameter);
            lengths.Add(length);
            widths.Add(diameter);
        }
        SaveListsToCSV("D:/Unity/LogNormal.csv");
    }

    public void ReloadWhiskersButton()
    {
        GameObject[] allWhiskers;
        allWhiskers = GameObject.FindGameObjectsWithTag("whiskerClone");
        foreach (GameObject whisk in allWhiskers)
        {
            Destroy(whisk.gameObject);
        }
        bridgesPerRun = 0;
        MakeWhiskerButton();
    }

    public void SaveListsToCSV(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Length,Width");
            for (int i = 0; i < lengths.Count; i++)
            {
                writer.WriteLine($"{lengths[i]},{widths[i]}");
            }
        }
    }
}