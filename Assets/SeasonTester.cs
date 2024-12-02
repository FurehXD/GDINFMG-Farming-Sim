using UnityEngine;
using System.Collections.Generic;

public class SeasonTester : MonoBehaviour
{
    private DataRetriever dataRetriever;

    private async void Start()
    {
        // Get reference to DataRetriever instance
        dataRetriever = DataRetriever.Instance;

        // Call the function and await its result
        List<Season> seasons = await dataRetriever.RetrieveSeasons();

        // Test output
        if (seasons.Count == 0)
        {
            Debug.LogError("No seasons were retrieved!");
            return;
        }

        foreach (Season season in seasons)
        {
            Debug.Log($"Season: {season.SeasonName}");
            Debug.Log($"Asset Path: {season.AssetPath}");
            Debug.Log($"Fertile Crops: {string.Join(", ", season.FertileCrops)}");
            Debug.Log($"Infertile Crops: {string.Join(", ", season.InfertileCrops)}");
            Debug.Log($"Color: R:{season.SeasonColor.r * 255}, G:{season.SeasonColor.g * 255}, B:{season.SeasonColor.b * 255}");
            Debug.Log("-------------------");
        }
    }
}