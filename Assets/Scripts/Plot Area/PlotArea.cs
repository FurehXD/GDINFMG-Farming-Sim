using UnityEngine;

public class PlotArea
{
    public int PlotID;
    public string PlotName;
    public float GrowthBuffPercentage;
    public Vector2 GridSize;
    public string AssetDirectory;

    public PlotArea (int plotID, string plotName, float growthBuffPercentage, Vector2 gridSize, string AssetDirectory)    {
        this.PlotID = plotID;
        this.PlotName = plotName;
        this.GrowthBuffPercentage = growthBuffPercentage;
        this.GridSize = gridSize;
        this.AssetDirectory = AssetDirectory;
    }

}
