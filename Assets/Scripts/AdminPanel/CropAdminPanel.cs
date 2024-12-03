using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

public class CropAdminPanel : BaseAdminPanel
{
    private string cropName = "";
    private float growthRate = 1.0f;
    private int seasonDuration = 30;
    private string fertileCrops = "";
    private string infertileCrops = "";
    private int seasonColorR = 255;
    private int seasonColorG = 255;
    private int seasonColorB = 255;

    private List<(int id, string name)> existingFertilizers = new List<(int id, string name)>();
    private List<(int id, decimal sellPrice)> existingMarkets = new List<(int id, decimal sellPrice)>();
    private int selectedFertilizerId = 1;
    private int selectedMarketId = 1;

    private List<(int id, string name)> existingSeasons = new List<(int id, string name)>();
    private int selectedSeasonId = 1;

    protected override void Awake()
    {
        base.Awake();
        if (backgroundStyle == null)
        {
            InitializeStyles();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ProcessAsyncOperation(LoadExistingData());
    }

    private async Task LoadExistingData()
    {
        try
        {
            using (var connection = new SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                // Load Seasons
                string seasonQuery = "SELECT SeasonID, SeasonName FROM Season";
                using (var command = new SqlCommand(seasonQuery, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    existingSeasons.Clear();
                    while (await reader.ReadAsync())
                    {
                        existingSeasons.Add((
                            reader.GetInt32(0),
                            reader.GetString(1)
                        ));
                    }
                }
                // Load Fertilizers
                string fertilizerQuery = "SELECT FertilizerID, FertilizerName FROM Fertilizer";
                using (var command = new SqlCommand(fertilizerQuery, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    existingFertilizers.Clear();
                    while (await reader.ReadAsync())
                    {
                        existingFertilizers.Add((
                            reader.GetInt32(0),
                            reader.GetString(1)
                        ));
                    }
                }

                // Load Markets
                string marketQuery = "SELECT MarketID, SellingPrice FROM Market";
                using (var command = new SqlCommand(marketQuery, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    existingMarkets.Clear();
                    while (await reader.ReadAsync())
                    {
                        existingMarkets.Add((
                            reader.GetInt32(0),
                            reader.GetDecimal(1)
                        ));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading existing data: {e.Message}");
            operationStatus = $"Error loading data: {e.Message}";
        }
    }

    private void OnGUI()
    {
        if (!gameObject.activeSelf || GUI.skin == null) return;

        if (backgroundStyle == null)
        {
            InitializeStyles();
            return;
        }

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", backgroundStyle);

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        try
        {
            DisplayHeader();
            DisplayCurrentData();
            DisplayAddNewCropSection();
        }
        finally
        {
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }

    private void DisplayHeader()
    {
        GUILayout.Box("Crop Management Panel", backgroundStyle, GUILayout.ExpandWidth(true));

        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Box(operationStatus, backgroundStyle, GUILayout.ExpandWidth(true));
        }
    }

    private void DisplayCurrentData()
    {
        GUILayout.Box("Current Crops", backgroundStyle, GUILayout.ExpandWidth(true));

        if (currentData != null && currentData.Rows.Count > 0)
        {
            // Display Headers
            GUILayout.BeginHorizontal(backgroundStyle);
            foreach (DataColumn column in currentData.Columns)
            {
                GUILayout.Label(column.ColumnName, labelStyle, GUILayout.Width(120));
            }
            GUILayout.EndHorizontal();

            // Display Data
            foreach (DataRow row in currentData.Rows)
            {
                GUILayout.BeginHorizontal(backgroundStyle);
                foreach (var item in row.ItemArray)
                {
                    GUILayout.Label(item.ToString(), labelStyle, GUILayout.Width(120));
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("No data available", labelStyle);
        }
    }

    private void DisplayAddNewCropSection()
    {
        GUILayout.Box("Add New Crop", backgroundStyle, GUILayout.ExpandWidth(true));

        GUILayout.BeginVertical(backgroundStyle);

        try
        {
            cropName = DrawInputField("Crop Name:", cropName);
            growthRate = DrawFloatField("Growth Rate:", growthRate);

            DisplayFertilizerSection();
            DisplaySeasonSection();
            DisplayMarketSection();

            GUI.enabled = !isOperationInProgress;
            if (GUILayout.Button("Add Crop", buttonStyle))
            {
                ProcessAsyncOperation(InsertCrop());
            }
            GUI.enabled = true;
        }
        finally
        {
            GUILayout.EndVertical();
        }
    }

    private void DisplayFertilizerSection()
    {
        GUILayout.Box("Fertilizer Selection", backgroundStyle);

        GUILayout.BeginVertical(backgroundStyle);
        try
        {
            GUILayout.Label("Select Fertilizer:", labelStyle);
            if (existingFertilizers.Count > 0)
            {
                int index = existingFertilizers.FindIndex(f => f.id == selectedFertilizerId);
                string[] options = existingFertilizers.Select(f => f.name).ToArray();
                index = GUILayout.SelectionGrid(index, options, 1, buttonStyle);
                if (index >= 0 && index < existingFertilizers.Count)
                {
                    selectedFertilizerId = existingFertilizers[index].id;
                }
            }
            else
            {
                GUILayout.Label("No fertilizers available", labelStyle);
            }
        }
        finally
        {
            GUILayout.EndVertical();
        }
    }

    private void DisplaySeasonSection()
    {
        GUILayout.Box("Season Selection", backgroundStyle);

        GUILayout.BeginVertical(backgroundStyle);
        try
        {
            GUILayout.Label($"Selected Season ID: {selectedSeasonId}", labelStyle); // Debug line
            GUILayout.Label("Select Season:", labelStyle);
            if (existingSeasons.Count > 0)
            {
                int index = existingSeasons.FindIndex(s => s.id == selectedSeasonId);
                string[] options = existingSeasons.Select(s => $"ID: {s.id} - {s.name}").ToArray();
                index = GUILayout.SelectionGrid(index, options, 1, buttonStyle);
                if (index >= 0 && index < existingSeasons.Count)
                {
                    selectedSeasonId = existingSeasons[index].id;
                }
            }
            else
            {
                GUILayout.Label("No seasons available", labelStyle);
            }
        }
        finally
        {
            GUILayout.EndVertical();
        }
    }

    private void DisplayMarketSection()
    {
        GUILayout.Box("Market Selection", backgroundStyle);

        GUILayout.BeginVertical(backgroundStyle);
        try
        {
            GUILayout.Label("Select Market:", labelStyle);
            if (existingMarkets.Count > 0)
            {
                int index = existingMarkets.FindIndex(m => m.id == selectedMarketId);
                string[] options = existingMarkets.Select(m => $"${m.sellPrice}").ToArray();
                index = GUILayout.SelectionGrid(index, options, 1, buttonStyle);
                if (index >= 0 && index < existingMarkets.Count)
                {
                    selectedMarketId = existingMarkets[index].id;
                }
            }
            else
            {
                GUILayout.Label("No markets available", labelStyle);
            }
        }
        finally
        {
            GUILayout.EndVertical();
        }
    }

    protected override async Task RefreshData()
    {
        try
        {
            using (var connection = new SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();
                string query = @"
                    SELECT 
                        c.CropName,
                        g.GrowthRate,
                        f.FertilizerName,
                        f.GrowthRateBuffPercentage as FertilizerBuff,
                        s.SeasonName,
                        s.FertileCrops,
                        s.InfertileCrops,
                        s.Duration as SeasonDuration,
                        m.PurchasingPrice,
                        m.SellingPrice
                    FROM Crops c
                    JOIN Growth g ON c.GrowthID = g.GrowthID
                    JOIN Fertilizer f ON c.FertilizerID = f.FertilizerID
                    JOIN Season s ON c.SeasonID = s.SeasonID
                    JOIN Market m ON c.MarketID = m.MarketID
                    ORDER BY c.CropName";

                using (var command = new SqlCommand(query, connection))
                {
                    var adapter = new SqlDataAdapter(command);
                    currentData = new DataTable();
                    adapter.Fill(currentData);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error refreshing crop data: {e.Message}");
            operationStatus = $"Error refreshing data: {e.Message}";
            throw;
        }
    }

    private async Task InsertCrop()
    {
        try
        {
            using (var connection = new SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Get next GrowthID and insert Growth data
                        string getMaxGrowthIDQuery = "SELECT ISNULL(MAX(GrowthID), 0) FROM Growth;";
                        int nextGrowthID;
                        using (var command = new SqlCommand(getMaxGrowthIDQuery, connection, transaction))
                        {
                            nextGrowthID = Convert.ToInt32(await command.ExecuteScalarAsync()) + 1;
                        }

                        string growthQuery = "INSERT INTO Growth (GrowthID, GrowthRate) VALUES (@GrowthID, @GrowthRate);";
                        using (var command = new SqlCommand(growthQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@GrowthID", nextGrowthID);
                            command.Parameters.AddWithValue("@GrowthRate", growthRate);
                            await command.ExecuteNonQueryAsync();
                        }

                        // Get next CropID
                        string getMaxCropIDQuery = "SELECT ISNULL(MAX(CropID), 0) FROM Crops;";
                        int nextCropID;
                        using (var command = new SqlCommand(getMaxCropIDQuery, connection, transaction))
                        {
                            nextCropID = Convert.ToInt32(await command.ExecuteScalarAsync()) + 1;
                        }

                        // Insert Crop using existing SeasonID
                        string cropQuery = "INSERT INTO Crops (CropID, CropName, GrowthID, FertilizerID, SeasonID, MarketID) VALUES (@CropID, @Name, @GrowthID, @FertilizerID, @SeasonID, @MarketID)";
                        using (var command = new SqlCommand(cropQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@CropID", nextCropID);
                            command.Parameters.AddWithValue("@Name", cropName);
                            command.Parameters.AddWithValue("@GrowthID", nextGrowthID);
                            command.Parameters.AddWithValue("@FertilizerID", selectedFertilizerId);
                            command.Parameters.AddWithValue("@SeasonID", selectedSeasonId);
                            command.Parameters.AddWithValue("@MarketID", selectedMarketId);
                            await command.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        ClearInputs();
                        await RefreshData();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inserting crop: {e.Message}");
            throw;
        }
    }

    private void ClearInputs()
    {
        cropName = "";
        growthRate = 1.0f;
        seasonDuration = 30;
        fertileCrops = "";
        infertileCrops = "";
        seasonColorR = 255;
        seasonColorG = 255;
        seasonColorB = 255;
        selectedFertilizerId = 1;
        selectedMarketId = 1;
    }
}