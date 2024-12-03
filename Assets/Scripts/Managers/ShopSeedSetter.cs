using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSeedSetter : MonoBehaviour
{
    [SerializeField]
    private CropButton cropTemplate;
    [SerializeField]
    private GridLayoutGroup seedArea;

    private List<CropButton> seedReferences = new();

    private TMP_Dropdown areaSelectorDropdown;
    private List<PlotArea> plots;
    private int maxSeedID = 12;//the last num for seeds in the market table

    private void Start()
    {
        this.areaSelectorDropdown = this.GetComponentInChildren<TMP_Dropdown>();
        this.plots = DataRetriever.Instance.RetrievePlotAreas();
        this.UpdateGameLocation();
    }
    private void Update()
    {
        this.plots = DataRetriever.Instance.RetrievePlotAreas();
    }
    public void UpdateGameLocation()
    {
        this.ResetPlots();

        string currentPlotAreaString = this.areaSelectorDropdown.options[this.areaSelectorDropdown.value].text;

        PlotArea currentPlotArea = this.plots.Find(plotName => plotName.PlotName == currentPlotAreaString);

        if (currentPlotArea != null && this.cropTemplate != null)
        {
            float areaSizeTemp = currentPlotArea.GridSize.x * currentPlotArea.GridSize.y;
            int areaSize = (int)areaSizeTemp;

            for(int i = 1; i<this.maxSeedID + 1; i++)
            {
                if(this.seedArea)
                    this.seedReferences.Add(Instantiate(this.cropTemplate, this.seedArea.transform));
                    this.seedReferences[i].CropID = i;
            }

        }
    }
    private void ResetPlots()
    {
        foreach(CropButton cropButton in this.seedReferences)
            Destroy(cropButton.gameObject);

        this.seedReferences.Clear();
    }

}
