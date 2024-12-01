using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlotAreaRetriever : MonoBehaviour
{
    public static PlotAreaRetriever Instance;

    public List<PlotArea> AvailablePlotAreas = new();

    public PlotArea CurrentPlotArea;
    public string CurrentPlotName = "";
    public bool PlotAreaDataHasChanged = false;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        this.AvailablePlotAreas = this.RetrievePlotAreas();
    }
    private void OnEnable()
    {
        PlotAreaBroadcaster.OnPlotAreaChanged += this.LogCurrentPlotArea;
    }
    private void OnDisable()
    {
        PlotAreaBroadcaster.OnPlotAreaChanged -= this.LogCurrentPlotArea;
    }
    private void Update()
    {
        List<PlotArea> tempHolder = new List<PlotArea>();

        tempHolder = this.RetrievePlotAreas();

        this.PlotAreaDataHasChanged = tempHolder != this.AvailablePlotAreas;

        if (this.PlotAreaDataHasChanged) this.AvailablePlotAreas = this.RetrievePlotAreas();

        Debug.Log(this.PlotAreaDataHasChanged);
    }
    private List<PlotArea> RetrievePlotAreas()
    {
        return DataRetriever.Instance.RetrievePlotAreas();
    }
    public void LogCurrentPlotArea(string currentPlotAreaName)
    {
        this.CurrentPlotArea = this.AvailablePlotAreas.Find(plotName => plotName.PlotName == currentPlotAreaName);
        this.CurrentPlotName = currentPlotAreaName;
        Debug.Log("Current plot area logged (" + currentPlotAreaName + ")");
    }
}
