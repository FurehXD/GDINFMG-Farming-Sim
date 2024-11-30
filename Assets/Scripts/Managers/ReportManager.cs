using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public class ReportManager : MonoBehaviour {
    [SerializeField]
    public List<string> reportLines;

    public static ReportManager Instance;
    [SerializeField]
    public TMP_Text reportText;
    void OnAwake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetDailyReport() {
        this.reportLines.Clear();
    }

    public void AddCropReport(string cropName, float growth, string rarity, int sellingPrice, int currentNumber, int expectedHarvestTime) {
        string newReportLine = "";
        //
    }

    public void PrintReportLines() {
        foreach (string reportLine in reportLines)
        {
            //
        }
    }
}
