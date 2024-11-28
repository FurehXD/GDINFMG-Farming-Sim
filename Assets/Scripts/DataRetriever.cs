using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Linq;
using UnityEngine;

/*
 * Retrives Data from the database then assigns it to any respective attribute
 */
public class DataRetriever : MonoBehaviour
{
    public static DataRetriever Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance.gameObject);
    }

    public float RetrieveCropGrowthRate(int growthID)
    {
        switch (growthID)
        {
            case 1:
                return 0.8f;
                break;
        }
        Debug.LogError("NO GROWTH RATE WAS RETRIEVED");
        return 0;
    }

    public int RetrieveGrowthID(int cropID)
    {
        switch (cropID)
        {
            case 1:
                return 1;
                break;
        }
        Debug.LogError("NO GROWTH ID WAS RETRIEVED");
        return 0;
    }

    public string RetrieveCropName(int cropID)
    {
        switch (cropID)
        {
            case 1:
                return "Apple";
                break;
        }
        Debug.LogError("NO CROP NAME WAS RETRIEVED");
        return "";
    }
    public string RetrieveCropAssetDirectory(int cropID)
    {
        switch (cropID)
        {
            case 1:
                return "Sprites/Crops/Apple/Apple";
                break;
        }
        Debug.LogError("NO CROP ASSET DIRECTORY WAS RETRIEVED");
        return "";
    }

    public int RetrieveMarketID(int cropID)
    {
        switch (cropID)
        {
            case 1:
                return 1;
                break;
        }
        Debug.LogError("NO CROP MARKET ID WAS RETRIEVED");
        return 0;
    }
    public int RetrieveSellingPrice(int marketID)
    {
        switch (marketID)
        {
            case 1:
                return 20;
                break;
        }
        Debug.LogError("NO CROP SELLING PRICE WAS RETRIEVED");
        return 0;
    }
    public List<Rarity> RetrieveRarities()
    {
        List<Rarity> rarities = new();

        rarities.Add(new Rarity(1, "Common", 0.0f, 0.5f, new Color(255, 255, 255, 255)));
        rarities.Add(new Rarity(2, "Rare", 25.0f, 0.3f, new Color(51, 153, 255, 255)));
        rarities.Add(new Rarity(3, "Epic", 50.0f, 0.15f, new Color(153, 51, 255, 255)));
        rarities.Add(new Rarity(4, "Legendary", 100.0f, 0.05f, new Color(255, 215, 0, 255)));

        return rarities;
    }
}
