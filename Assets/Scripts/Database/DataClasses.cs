using System;
using UnityEngine;

[Serializable]
public class CropData
{
    public int CropID { get; set; }
    public string CropName { get; set; }
    public GrowthData Growth { get; set; }
    public FertilizerData Fertilizer { get; set; }
    public SeasonData Season { get; set; }
    public MarketData Market { get; set; }
}

[Serializable]
public class GrowthData
{
    public int GrowthID { get; set; }
    public float GrowthRate { get; set; }
}

[Serializable]
public class FertilizerData
{
    public int FertilizerID { get; set; }
    public string FertilizerName { get; set; }
    public float GrowthRateBuffPercentage { get; set; }
    public int ItemID { get; set; }
}

[Serializable]
public class SeasonData
{
    public int SeasonID { get; set; }
    public string SeasonName { get; set; }
    public string FertileCrops { get; set; }
    public string InfertileCrops { get; set; }
    public int Duration { get; set; }
}

[Serializable]
public class MarketData
{
    public int MarketID { get; set; }
    public decimal PurchasingPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int ItemID { get; set; }
}


[Serializable]
public class RarityData
{
    public int RarityID { get; set; }
    public string RarityType { get; set; }
    public float PriceBuffPercentage { get; set; }
    public float Probability { get; set; }
    public int RarityColorRed { get; set; }
    public int RarityColorGreen { get; set; }
    public int RarityColorBlue { get; set; }
}

[Serializable]
public class ItemData
{
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public string ItemCategory { get; set; }
}

[Serializable]
public class PlotData
{
    public int PlotID { get; set; }
    public string PlotName { get; set; }
    public float UniversalGrowthBuffPercentage { get; set; }
    public int GridSizeX { get; set; }
    public int GridSizeY { get; set; }
}

[Serializable]
public class CalendarData
{
    public int CalendarID { get; set; }
    public string CalendarName { get; set; }
}

[Serializable]
public class UpgradeData
{
    public int UpgradeID { get; set; }
    public string UpgradeName { get; set; }
    public string Description { get; set; }
    public int ItemID { get; set; }
    public int MarketID { get; set; }
}