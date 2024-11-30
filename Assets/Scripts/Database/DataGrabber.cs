using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;

public class DataGrabber : MonoBehaviour
{
    public static DataGrabber Instance { get; private set; }
    private DatabaseManager dbManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            dbManager = DatabaseManager.Instance;
        }
        else
            Destroy(gameObject);
    }

    public async Task<float> RetrieveCropGrowthRate(int growthID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(dbManager.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT GrowthRate FROM Growth WHERE GrowthID = @GrowthID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GrowthID", growthID);
                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToSingle(result) : 0f;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error retrieving growth rate: {e.Message}");
            return 0f;
        }
    }

    public async Task<int> RetrieveGrowthID(int cropID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(dbManager.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT GrowthID FROM Crops WHERE CropID = @CropID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CropID", cropID);
                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error retrieving growth ID: {e.Message}");
            return 0;
        }
    }

    public async Task<string> RetrieveCropName(int cropID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(dbManager.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT CropName FROM Crops WHERE CropID = @CropID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CropID", cropID);
                    var result = await command.ExecuteScalarAsync();
                    return result?.ToString() ?? "";
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error retrieving crop name: {e.Message}");
            return "";
        }
    }

    public async Task<int> RetrieveMarketID(int cropID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(dbManager.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT MarketID FROM Crops WHERE CropID = @CropID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CropID", cropID);
                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error retrieving market ID: {e.Message}");
            return 0;
        }
    }

    public async Task<decimal> RetrieveSellingPrice(int marketID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(dbManager.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT SellingPrice FROM Market WHERE MarketID = @MarketID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MarketID", marketID);
                    var result = await command.ExecuteScalarAsync();
                    return result != null ? Convert.ToDecimal(result) : 0m;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error retrieving selling price: {e.Message}");
            return 0m;
        }
    }

    public async Task<List<Rarity>> RetrieveRarities()
    {
        return await dbManager.GetRarities();
    }

    public async Task<List<QualityData>> RetrieveQualities()
    {
        return await dbManager.GetQualities();
    }

    // Keep this as is since it's asset-related and not database-related
    public string RetrieveCropAssetDirectory(int cropID)
    {
        switch (cropID)
        {
            case 1:
                return "Sprites/Crops/Apple/Apple";
        }
        Debug.LogError("NO CROP ASSET DIRECTORY WAS RETRIEVED");
        return "";
    }
}