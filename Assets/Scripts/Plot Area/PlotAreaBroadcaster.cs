using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlotAreaBroadcaster : MonoBehaviour
{
    private TMP_Dropdown plotDropdown;
    private string currentPlotName;

    public static event Action<string> OnPlotAreaChanged;

    private DropdownBehavior dropdownBehavior;
    private void Awake()
    {
        this.plotDropdown = this.GetComponent<TMP_Dropdown>();
        this.dropdownBehavior = this.GetComponent<DropdownBehavior>();
    }
    private void Start()
    {
        StartCoroutine(this.SetCurrentPlotName());
    }

    private IEnumerator SetCurrentPlotName()
    {
        yield return new WaitUntil(()=> dropdownBehavior.PlotDropdown != null);
        yield return new WaitUntil(()=> dropdownBehavior.PlotDropdown.options.Count > 0);

        if (this.plotDropdown && this.dropdownBehavior)
        {
            Debug.Log("here: " + this.plotDropdown.options[this.plotDropdown.value].text);
            PlotAreaRetriever.Instance.SetCurrentPlotName(this.plotDropdown.options[this.plotDropdown.value].text);
        }
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
