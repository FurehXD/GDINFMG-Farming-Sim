using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlotAreaSetter : MonoBehaviour
{
    [SerializeField]
    private PlotButton plotTemplate;
    [SerializeField]
    private GridLayoutGroup farmArea;

    [SerializeField]
    private List<PlotButton> plotReferences = new();

    private TMP_Dropdown areaSelectorDropdown;
    private List<PlotArea> plots;

    private void Start()
    {
        this.areaSelectorDropdown = this.GetComponentInChildren<TMP_Dropdown>();
        this.plots = DataRetriever.Instance.RetrievePlotAreas();
        this.UpdateDropDown();
        this.UpdateGameLocation();
    }
    private void Update()
    {
        this.plots = DataRetriever.Instance.RetrievePlotAreas();
        this.UpdateDropDown();
    }
    private void UpdateDropDown()
    {
        this.areaSelectorDropdown.options.Clear();
        foreach(PlotArea plotArea in this.plots)
        { 
            this.areaSelectorDropdown.options.Add(new TMP_Dropdown.OptionData(plotArea.PlotName));
            this.areaSelectorDropdown.RefreshShownValue();
        }
    }
    public void UpdateGameLocation()
    {
        this.ResetPlots();

        string currentPlotAreaString = this.areaSelectorDropdown.options[this.areaSelectorDropdown.value].text;

        PlotArea currentPlotArea = this.plots.Find(plotName => plotName.PlotName == currentPlotAreaString);

        if (currentPlotArea != null && this.plotTemplate != null)
        {
            float areaSizeTemp = currentPlotArea.GridSize.x * currentPlotArea.GridSize.y;
            int areaSize = (int)areaSizeTemp;

            for(int i = 0; i<areaSize; i++)
            {
                if(this.farmArea)
                    this.plotReferences.Add(Instantiate(this.plotTemplate, this.farmArea.transform));
            }

        }
    }
    private void ResetPlots()
    {
        foreach(PlotButton plotButton in this.plotReferences)
            Destroy(plotButton.gameObject);

        this.plotReferences.Clear();
    }

}
