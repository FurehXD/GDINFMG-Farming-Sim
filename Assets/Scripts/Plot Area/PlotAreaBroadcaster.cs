using System;
using TMPro;
using UnityEngine;

public class PlotAreaBroadcaster : MonoBehaviour
{
    private TMP_Dropdown plotDropdown;
    private string currentPlotName;

    public static event Action<string> OnPlotAreaChanged;
    private void Start()
    {
        this.plotDropdown = this.GetComponent<TMP_Dropdown>();

        if(this.plotDropdown && PlotAreaRetriever.Instance)
            PlotAreaRetriever.Instance.SetCurrentPlotName(this.plotDropdown.options[this.plotDropdown.value].text);
    }
    public void AcquireChangedPlot()
    {
        this.currentPlotName = this.plotDropdown.options[this.plotDropdown.value].text;

        this.BroadcastChangedPlot();
    }
    private void BroadcastChangedPlot()
    {
        OnPlotAreaChanged?.Invoke(currentPlotName);
        Debug.Log(currentPlotName + " Was Broadcasted!");
    }
}
