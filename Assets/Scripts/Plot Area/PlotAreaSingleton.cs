using System.Collections.Generic;
using UnityEngine;

public class PlotAreaSingleton : MonoBehaviour
{
    public static PlotAreaSingleton Instance;

    private List<PlotArea> availablePlotAreas = new();

    public PlotArea CurrentPlotArea;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
        this.availablePlotAreas = DataRetriever.Instance.RetrievePlotAreas();
    }
    public void LogCurrentPlotArea(string currentPlotAreaName)
    {
        this.CurrentPlotArea = this.availablePlotAreas.Find(plotName => plotName.PlotName == currentPlotAreaName);
        Debug.Log("Current plot area logged (" + currentPlotAreaName + ")");
    }
}
