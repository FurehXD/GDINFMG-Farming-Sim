// DatabaseManager.cs
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class DatabaseManager : MonoBehaviour
{
    private const string API_BASE_URL = "http://localhost:5000/api/"; // Change this to your API URL

    private static DatabaseManager instance;
    public static DatabaseManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<DatabaseManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("DatabaseManager");
                    instance = go.AddComponent<DatabaseManager>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator GetCrops(Action<CropData[]> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(API_BASE_URL + "crops"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                CropData[] crops = JsonConvert.DeserializeObject<CropData[]>(json);
                callback(crops);

                // Debug log example
                foreach (var crop in crops)
                {
                    Debug.Log($"Crop: {crop.CropName}, Growth Rate: {crop.Growth.GrowthRate}");
                }
            }
            else
            {
                Debug.LogError("Error: " + www.error);
                callback(null);
            }
        }
    }

    
}

