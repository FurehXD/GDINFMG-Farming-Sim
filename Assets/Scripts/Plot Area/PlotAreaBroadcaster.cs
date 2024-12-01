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
        this.currentPlotName = this.plotDropdown.options[this.plotDropdown.value].text;
        PlotAreaSingleton.Instance.LogCurrentPlotArea(currentPlotName);
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
