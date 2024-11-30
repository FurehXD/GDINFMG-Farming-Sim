
using System;

[Serializable]
public class QualityData
{
    public int QualityID { get; set; }
    public string QualityName { get; set; }
    public float GrowthRateBuffPercentage { get; set; }

    public QualityData(int qualityID, string qualityName, float growthRateBuffPercentage)
    {
        QualityID = qualityID;
        QualityName = qualityName;
        GrowthRateBuffPercentage = growthRateBuffPercentage;
    }
}