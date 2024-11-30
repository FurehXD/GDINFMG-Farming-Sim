using UnityEngine;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DatabaseManager : MonoBehaviour
{
    private static DatabaseManager instance;
    private string connectionString;
    public string ConnectionString => connectionString;

    [SerializeField] private string serverAddress = "34.124.240.46";
    [SerializeField] private string databaseName = "FarmGame";
    [SerializeField] private string username = "sqlserver";
    [SerializeField] private string password = "m^=^LX)9l\\RQ\"}&0";

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
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void InitializeDatabase()
    {
        connectionString = $"Server={serverAddress},1433;" +
                         $"Database={databaseName};" +
                         $"User ID={username};" +
                         $"Password={password};" +
                         "Encrypt=True;" +
                         "TrustServerCertificate=True;" +
                         "Connection Timeout=10;";

        await TestConnection();
    }

    public void UpdateConnectionDetails(string server, string database, string user, string pass)
    {
        serverAddress = server;
        databaseName = database;
        username = user;
        password = pass;
        InitializeDatabase();
    }

    private async Task TestConnection()
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Debug.Log($"Attempting to connect to: {serverAddress}");
                await connection.OpenAsync();
                Debug.Log("Database connection successful!");

                // Test if we can actually query the database
                using (SqlCommand command = new SqlCommand("SELECT 1", connection))
                {
                    await command.ExecuteScalarAsync();
                    Debug.Log("Query test successful");
                }
            }
        }
        catch (SqlException e)
        {
            Debug.LogError($"SQL Error Number: {e.Number}");
            Debug.LogError($"Database connection failed: {e.Message}");

            switch (e.Number)
            {
                case -2:
                    Debug.LogError("Timeout expired. Server may be down or unreachable.");
                    break;
                case 4060:
                    Debug.LogError("Invalid database. Check database name.");
                    break;
                case 18456:
                    Debug.LogError("Login failed. Check credentials.");
                    break;
                case 40615:
                    Debug.LogError("Connection encryption error. Check SSL settings.");
                    break;
                default:
                    Debug.LogError($"Unexpected SQL error: {e.Message}");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"General connection error: {e.Message}");
        }
    }

    public async Task<List<CropData>> GetCrops()
    {
        List<CropData> crops = new List<CropData>();
        
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = @"
                    SELECT 
                        c.CropID, c.CropName,
                        g.GrowthID, g.GrowthRate,
                        f.FertilizerID, f.FertilizerName, f.GrowthRateBuffPercentage, f.ItemID as FertilizerItemID,
                        s.SeasonID, s.SeasonName, s.FertileCrops, s.InfertileCrops, s.Duration,
                        m.MarketID, m.PurchasingPrice, m.SellingPrice, m.ItemID as MarketItemID
                    FROM Crops c
                    LEFT JOIN Growth g ON c.GrowthID = g.GrowthID
                    LEFT JOIN Fertilizer f ON c.FertilizerID = f.FertilizerID
                    LEFT JOIN Season s ON c.SeasonID = s.SeasonID
                    LEFT JOIN Market m ON c.MarketID = m.MarketID";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        CropData crop = new CropData
                        {
                            CropID = Convert.ToInt32(reader["CropID"]),
                            CropName = reader["CropName"].ToString(),
                            Growth = new GrowthData
                            {
                                GrowthID = Convert.ToInt32(reader["GrowthID"]),
                                GrowthRate = Convert.ToSingle(reader["GrowthRate"])
                            },
                            Fertilizer = new FertilizerData
                            {
                                FertilizerID = Convert.ToInt32(reader["FertilizerID"]),
                                FertilizerName = reader["FertilizerName"].ToString(),
                                GrowthRateBuffPercentage = Convert.ToSingle(reader["GrowthRateBuffPercentage"]),
                                ItemID = Convert.ToInt32(reader["FertilizerItemID"])
                            },
                            Season = new SeasonData
                            {
                                SeasonID = Convert.ToInt32(reader["SeasonID"]),
                                SeasonName = reader["SeasonName"].ToString(),
                                FertileCrops = reader["FertileCrops"].ToString(),
                                InfertileCrops = reader["InfertileCrops"].ToString(),
                                Duration = Convert.ToInt32(reader["Duration"])
                            },
                            Market = new MarketData
                            {
                                MarketID = Convert.ToInt32(reader["MarketID"]),
                                PurchasingPrice = Convert.ToDecimal(reader["PurchasingPrice"]),
                                SellingPrice = Convert.ToDecimal(reader["SellingPrice"]),
                                ItemID = Convert.ToInt32(reader["MarketItemID"])
                            }
                        };
                        crops.Add(crop);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching crops: {e.Message}");
            return null;
        }

        return crops;
    }

    public async Task<List<QualityData>> GetQualities()
    {
        List<QualityData> qualities = new List<QualityData>();
        
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Quality";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        QualityData quality = new QualityData
                        (
                            Convert.ToInt32(reader["QualityID"]),
                            reader["QualityName"].ToString(),
                            Convert.ToSingle(reader["GrowthRateBuffPercentage"])
                        );
                        qualities.Add(quality);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching qualities: {e.Message}");
            return null;
        }

        return qualities;
    }

    public async Task<List<Rarity>> GetRarities()
    {
        List<Rarity> rarities = new List<Rarity>();

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Rarity";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Color rarityColor = new Color(
                            Convert.ToInt32(reader["RarityColorRed"]) / 255f,
                            Convert.ToInt32(reader["RarityColorGreen"]) / 255f,
                            Convert.ToInt32(reader["RarityColorBlue"]) / 255f,
                            1f
                        );

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
            Debug.LogError($"Error fetching rarities: {e.Message}");
            return null;
        }

        return rarities;
    }

    public async Task<List<PlotData>> GetPlots()
    {
        List<PlotData> plots = new List<PlotData>();
        
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Plot";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        PlotData plot = new PlotData
                        {
                            PlotID = Convert.ToInt32(reader["PlotID"]),
                            PlotName = reader["PlotName"].ToString(),
                            UniversalGrowthBuffPercentage = Convert.ToSingle(reader["UniversalGrowthBuffPercentage"]),
                            GridSizeX = Convert.ToInt32(reader["GridSizeX"]),
                            GridSizeY = Convert.ToInt32(reader["GridSizeY"])
                        };
                        plots.Add(plot);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching plots: {e.Message}");
            return null;
        }

        return plots;
    }

    public async void FetchAllDataExample()
    {
        Debug.Log("=== FETCHING CROPS ===");
        List<CropData> crops = await GetCrops();
        if (crops != null)
        {
            foreach (var crop in crops)
            {
                Debug.Log(
                    "Crop Details:\n" +
                    $"- ID: {crop.CropID}\n" +
                    $"- Name: {crop.CropName}\n\n" +

                    "Growth:\n" +
                    $"- Growth Rate: {crop.Growth.GrowthRate}\n\n" +

                    "Fertilizer:\n" +
                    $"- Name: {crop.Fertilizer.FertilizerName}\n" +
                    $"- Growth Buff: {crop.Fertilizer.GrowthRateBuffPercentage}%\n\n" +

                    "Season:\n" +
                    $"- Name: {crop.Season.SeasonName}\n" +
                    $"- Fertile Crops: {crop.Season.FertileCrops}\n" +
                    $"- Infertile Crops: {crop.Season.InfertileCrops}\n" +
                    $"- Duration: {crop.Season.Duration}\n\n" +

                    "Market:\n" +
                    $"- Buy Price: {crop.Market.PurchasingPrice}\n" +
                    $"- Sell Price: {crop.Market.SellingPrice}\n" +
                    "----------------------------------------"
                );
            }
        }

        Debug.Log("=== FETCHING RARITIES ===");
        List<Rarity> rarities = await GetRarities();
        if (rarities != null)
        {
            foreach (var rarity in rarities)
            {
                Debug.Log(
                    "Rarity Details:\n" +
                    $"- ID: {rarity.RarityID}\n" +
                    $"- Type: {rarity.RarityType}\n" +
                    $"- Price Buff: {rarity.PriceBuffPercentage}%\n" +
                    $"- Probability: {rarity.RarityProbability}%\n" +
                    $"- Color: {rarity.RarityColor}\n" +
                    "----------------------------------------"
                );
            }
        }
    }
}