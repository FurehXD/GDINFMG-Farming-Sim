using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotAreaBackgroundChanger : MonoBehaviour
{
    private Image imageComponent;
    private void Start()
    {
        this.imageComponent = this.GetComponent<Image>();
    }
    private void OnEnable()
    {
        PlotAreaBroadcaster.OnPlotAreaChanged += this.ChangeBackground;
    }
    private void OnDisable()
    {
        PlotAreaBroadcaster.OnPlotAreaChanged -= this.ChangeBackground;
    }
    private void ChangeBackground(string currentPlotAreaName)
    {
        PlotArea currentPlotArea = PlotAreaSingleton.Instance.CurrentPlotArea;

        string plotAssetFolder = currentPlotArea.AssetDirectory;

        plotAssetFolder += "/";
        plotAssetFolder += currentPlotAreaName;
        plotAssetFolder += " ";
        plotAssetFolder += "BG";

        Sprite plotAssetBG = Resources.Load<Sprite>(plotAssetFolder);

        if (plotAssetBG != null && this.imageComponent != null)
        {
            this.imageComponent.sprite = plotAssetBG;
        }
        else if (plotAssetBG == null) Debug.LogError("Sprite under " + plotAssetFolder + " not found!");
    }
}
