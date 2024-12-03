using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;

public class DropdownBehavior : MonoBehaviour
{
    private TMP_Dropdown plotDropdown;
    public TMP_Dropdown PlotDropdown { get { return plotDropdown; } }
    private void Start()
    {
        this.plotDropdown = this.GetComponent<TMP_Dropdown>();

        if (plotDropdown && PlotAreaRetriever.Instance.AvailablePlotAreas != null)
            this.UpdateDropDown();
    }
    private void Update()
    {
        if (plotDropdown && PlotAreaRetriever.Instance.AvailablePlotAreas != null)
            this.UpdateDropDown();
    }
    private void UpdateDropDown()
    {
        this.plotDropdown.options.Clear();
        foreach (PlotArea plotArea in PlotAreaRetriever.Instance.AvailablePlotAreas)
        {
            this.plotDropdown.options.Add(new TMP_Dropdown.OptionData(plotArea.PlotName));
            this.plotDropdown.RefreshShownValue();
        }
    }
}
