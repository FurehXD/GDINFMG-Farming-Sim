using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Season 
{
    public int SeasonID;
    public string SeasonName;

    public List<int> FertileCrops;
    public List<int> InfertileCrops;

    public int SeasonDuration;

    public string AssetPath;

    public Color SeasonColor;

    public Season(int seasonID, string seasonName, List<int> fertileCrops, List<int> infertileCrops, int seasonDuration, string assetPath, Color seasonColor)
    {
        SeasonID = seasonID;
        SeasonName = seasonName;
        FertileCrops = fertileCrops;
        InfertileCrops = infertileCrops;
        SeasonDuration = seasonDuration;
        AssetPath = assetPath;
        SeasonColor = seasonColor;
    }
}
