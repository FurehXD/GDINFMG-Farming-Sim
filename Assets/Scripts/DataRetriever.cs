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
                return 5.0f;
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
                return "Sprites/Crops/Apple";
                break;
        }
        Debug.LogError("NO CROP ASSET DIRECTORY WAS RETRIEVED");
        return "";
    }
}
