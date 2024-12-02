using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Linq;
/*
 * Retrives Data from the database then assigns it to any respective attribute
 */
public class DataRetriever : MonoBehaviour
{
    public static DataRetriever Instance { get; private set; }
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

    public async Task<List<int>> RetrieveAllCropIDs()
    {
        List<int> cropIDs = new List<int>();
        try
        {
            using (SqlConnection connection = new SqlConnection(dbManager.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT CropID FROM Crops";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cropIDs.Add(Convert.ToInt32(reader["CropID"]));
                    }
                }

                Debug.Log($"Retrieved {cropIDs.Count} crop IDs from the database");
                return cropIDs;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error retrieving crop IDs: {e.Message}");
            return new List<int>();
        }
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

    public async Task<int> RetrieveCropGrowthID(int cropID)
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

    public async Task<int> RetrieveCropMarketID(int cropID)
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

    public async Task<decimal> RetrieveCropSellingPrice(int marketID)
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
        List<Rarity> rarities = new List<Rarity>();
        try
        {
            using (SqlConnection connection = new SqlConnection(dbManager.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Rarity";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Create a Color from RGB values in database
                        Color rarityColor = new Color(
                            Convert.ToInt32(reader["RarityColorRed"]) / 255f,
                            Convert.ToInt32(reader["RarityColorGreen"]) / 255f,
                            Convert.ToInt32(reader["RarityColorBlue"]) / 255f,
                            1f);  // Alpha set to 1 for full opacity

                        Rarity rarity = new Rarity(
                            Convert.ToInt32(reader["RarityID"]),
                            reader["RarityType"].ToString(),
                            Convert.ToSingle(reader["PriceBuffPercentage"]),
                            Convert.ToSingle(reader["Probability"]),
                            rarityColor
                        );
                        rarities.Add(rarity);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error retrieving rarities: {e.Message}");
            return null;
        }

        return rarities;
    }

    public async Task<List<QualityData>> RetrieveQualities()
    {
        return await dbManager.GetQualities();
    }

    public string RetrieveCropAssetDirectoryTemp(int cropID)
    {
        switch (cropID)
        {
            case 1:
                return "Sprites/Crops/Apple/Apple";
        }
        Debug.LogError("NO CROP ASSET DIRECTORY WAS RETRIEVED");
        return "";
    }
    //@TODO
    public async Task<string> RetrieveCropAssetDirectory(int cropID)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(dbManager.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT AssetPath FROM Assets WHERE AssetID = @AssetID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AssetID", cropID);
                    var result = await command.ExecuteScalarAsync();
                    if (result == null)
                    {
                        Debug.LogError($"No asset found for CropID: {cropID}");
                        return "";
                    }
                    return result.ToString();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error retrieving asset directory: {e.Message}");
            return "";
        }
    }

    //@TODO
    public List<PlotArea> RetrievePlotAreas()
    {
        List<PlotArea > plotAreas = new List<PlotArea>();

        plotAreas.Add(new PlotArea(1, "Outdoor", 1.1f, new Vector2(8,5), "Sprites/Plot Area/Outdoor"));
        plotAreas.Add(new PlotArea(2, "Greenhouse", 0.9f, new Vector2(5,5), "Sprites/Plot Area/Greenhouse"));
        plotAreas.Add(new PlotArea(3, "Lakeside", 1.0f, new Vector2(6,5), "Sprites/Plot Area/Lakeside"));

        return plotAreas;
    }
    //@TODO
    public async Task<int> RetrieveAvailableCropCount()
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(dbManager.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT COUNT(*) FROM Assets WHERE AssetPath LIKE 'Sprites/Crops/%'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var result = await command.ExecuteScalarAsync();
                    int cropCount = result != null ? Convert.ToInt32(result) : 0;
                    Debug.Log($"Found {cropCount} available crops in the database");
                    return cropCount;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error retrieving available crop count: {e.Message}");
            return 0;
        }
    }

    //@TODO
    public async Task<List<Season>> RetrieveSeasons()
    {
        List<Season> seasons = new List<Season>();
        try
        {
            using (SqlConnection connection = new SqlConnection(dbManager.ConnectionString))
            {
                await connection.OpenAsync();
                string query = @"
                SELECT 
                    SeasonID,
                    SeasonName,
                    FertileCrops,
                    InfertileCrops,
                    Duration,
                    ColorR,
                    ColorG,
                    ColorB,
                    AssetPath
                FROM Season s";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // Convert comma-separated strings to List<int>
                        List<int> fertileCrops = reader["FertileCrops"].ToString()
                            .Split(',')
                            .Select(int.Parse)
                            .ToList();

                        List<int> infertileCrops = reader["InfertileCrops"].ToString()
                            .Split(',')
                            .Select(int.Parse)
                            .ToList();

                        // Create Unity Color from RGB values
                        Color seasonColor = new Color(
                            Convert.ToInt32(reader["ColorR"]) / 255f,
                            Convert.ToInt32(reader["ColorG"]) / 255f,
                            Convert.ToInt32(reader["ColorB"]) / 255f,
                            1f);  // Alpha set to 1 for full opacity

                        Season season = new Season(
                            Convert.ToInt32(reader["SeasonID"]),
                            reader["SeasonName"].ToString(),
                            fertileCrops,
                            infertileCrops,
                            Convert.ToInt32(reader["Duration"]),
                            reader["AssetPath"].ToString(),
                            seasonColor
                        );

                        seasons.Add(season);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error retrieving seasons: {e.Message}");
            return new List<Season>();
        }

        return seasons;
    }
}