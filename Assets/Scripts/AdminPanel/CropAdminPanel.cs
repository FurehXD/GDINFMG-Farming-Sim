using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class CropAdminPanel : BaseAdminPanel
{
    private string cropName = "";
    private float growthRate = 1.0f;
    private string fertilizerName = "";
    private float fertilizerBuff = 0f;
    private int fertilizerItemId = 0;
    private string seasonName = "";
    private string fertileCrops = "";
    private string infertileCrops = "";
    private int seasonDuration = 30;
    private decimal purchasePrice = 0m;
    private decimal sellingPrice = 0m;
    private int marketItemId = 0;

    private void OnGUI()
    {
        if (!gameObject.activeSelf) return;

        // Draw the background
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none, GUI.skin.box);

        // Begin the main area
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

        // Begin the scroll view
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Header
        GUILayout.Box("Crop Management Panel", GUILayout.ExpandWidth(true));

        // Status message if any
        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Box(operationStatus, GUILayout.ExpandWidth(true));
        }

        // Display current data
        GUILayout.Box("Current Crops", GUILayout.ExpandWidth(true));
        if (currentData != null && currentData.Rows.Count > 0)
        {
            // Headers
            GUILayout.BeginHorizontal();
            foreach (DataColumn column in currentData.Columns)
            {
                GUILayout.Box(column.ColumnName, GUILayout.Width(120));
            }
            GUILayout.EndHorizontal();

            // Data rows
            foreach (DataRow row in currentData.Rows)
            {
                GUILayout.BeginHorizontal();
                foreach (var item in row.ItemArray)
                {
                    GUILayout.Box(item.ToString(), GUILayout.Width(120));
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("No data available");
        }

        // Add new crop section
        GUILayout.Space(20);
        GUILayout.Box("Add New Crop", GUILayout.ExpandWidth(true));

        // Input fields
        GUILayout.BeginVertical(GUI.skin.box);

        // Basic crop details
        GUILayout.Label("Crop Name:");
        cropName = GUILayout.TextField(cropName, GUILayout.Width(200));

        GUILayout.Label("Growth Rate:");
        string growthRateStr = GUILayout.TextField(growthRate.ToString(), GUILayout.Width(100));
        float.TryParse(growthRateStr, out growthRate);

        // Fertilizer section
        GUILayout.Box("Fertilizer Details", GUILayout.ExpandWidth(true));
        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Fertilizer Name:");
        fertilizerName = GUILayout.TextField(fertilizerName, GUILayout.Width(200));

        GUILayout.Label("Growth Buff %:");
        string buffStr = GUILayout.TextField(fertilizerBuff.ToString(), GUILayout.Width(100));
        float.TryParse(buffStr, out fertilizerBuff);

        GUILayout.Label("Item ID:");
        string itemIdStr = GUILayout.TextField(fertilizerItemId.ToString(), GUILayout.Width(100));
        int.TryParse(itemIdStr, out fertilizerItemId);

        GUILayout.EndVertical();

        // Season section
        GUILayout.Box("Season Details", GUILayout.ExpandWidth(true));
        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Season Name:");
        seasonName = GUILayout.TextField(seasonName, GUILayout.Width(200));

        GUILayout.Label("Fertile Crops (comma-separated):");
        fertileCrops = GUILayout.TextField(fertileCrops, GUILayout.Width(300));

        GUILayout.Label("Infertile Crops (comma-separated):");
        infertileCrops = GUILayout.TextField(infertileCrops, GUILayout.Width(300));

        GUILayout.Label("Duration (days):");
        string durationStr = GUILayout.TextField(seasonDuration.ToString(), GUILayout.Width(100));
        int.TryParse(durationStr, out seasonDuration);

        GUILayout.EndVertical();

        // Market section
        GUILayout.Box("Market Details", GUILayout.ExpandWidth(true));
        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Purchase Price:");
        string purchaseStr = GUILayout.TextField(purchasePrice.ToString(), GUILayout.Width(100));
        decimal.TryParse(purchaseStr, out purchasePrice);

        GUILayout.Label("Selling Price:");
        string sellingStr = GUILayout.TextField(sellingPrice.ToString(), GUILayout.Width(100));
        decimal.TryParse(sellingStr, out sellingPrice);

        GUILayout.Label("Market Item ID:");
        string marketIdStr = GUILayout.TextField(marketItemId.ToString(), GUILayout.Width(100));
        int.TryParse(marketIdStr, out marketItemId);

        GUILayout.EndVertical();

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Crop", GUILayout.Width(100)))
        {
            ProcessAsyncOperation(InsertCrop());
        }
        GUI.enabled = true;

        GUILayout.EndVertical();

        GUILayout.EndScrollView();
        GUILayout.EndArea();
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

                // First, insert Growth data
                string growthQuery = "INSERT INTO Growth (GrowthRate) VALUES (@GrowthRate); SELECT SCOPE_IDENTITY();";
                int growthId;
                using (var command = new SqlCommand(growthQuery, connection))
                {
                    command.Parameters.AddWithValue("@GrowthRate", growthRate);
                    growthId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Insert Fertilizer data
                string fertilizerQuery = "INSERT INTO Fertilizer (FertilizerName, GrowthRateBuffPercentage, ItemID) VALUES (@Name, @Buff, @ItemID); SELECT SCOPE_IDENTITY();";
                int fertilizerId;
                using (var command = new SqlCommand(fertilizerQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", fertilizerName);
                    command.Parameters.AddWithValue("@Buff", fertilizerBuff);
                    command.Parameters.AddWithValue("@ItemID", fertilizerItemId);
                    fertilizerId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Insert Season data
                string seasonQuery = "INSERT INTO Season (SeasonName, FertileCrops, InfertileCrops, Duration) VALUES (@Name, @Fertile, @Infertile, @Duration); SELECT SCOPE_IDENTITY();";
                int seasonId;
                using (var command = new SqlCommand(seasonQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", seasonName);
                    command.Parameters.AddWithValue("@Fertile", fertileCrops);
                    command.Parameters.AddWithValue("@Infertile", infertileCrops);
                    command.Parameters.AddWithValue("@Duration", seasonDuration);
                    seasonId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Insert Market data
                string marketQuery = "INSERT INTO Market (PurchasingPrice, SellingPrice, ItemID) VALUES (@BuyPrice, @SellPrice, @ItemID); SELECT SCOPE_IDENTITY();";
                int marketId;
                using (var command = new SqlCommand(marketQuery, connection))
                {
                    command.Parameters.AddWithValue("@BuyPrice", purchasePrice);
                    command.Parameters.AddWithValue("@SellPrice", sellingPrice);
                    command.Parameters.AddWithValue("@ItemID", marketItemId);
                    marketId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Finally, insert the Crop data
                string cropQuery = "INSERT INTO Crops (CropName, GrowthID, FertilizerID, SeasonID, MarketID) VALUES (@Name, @GrowthID, @FertilizerID, @SeasonID, @MarketID)";
                using (var command = new SqlCommand(cropQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", cropName);
                    command.Parameters.AddWithValue("@GrowthID", growthId);
                    command.Parameters.AddWithValue("@FertilizerID", fertilizerId);
                    command.Parameters.AddWithValue("@SeasonID", seasonId);
                    command.Parameters.AddWithValue("@MarketID", marketId);
                    await command.ExecuteNonQueryAsync();
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
        fertilizerName = "";
        fertilizerBuff = 0f;
        fertilizerItemId = 0;
        seasonName = "";
        fertileCrops = "";
        infertileCrops = "";
        seasonDuration = 30;
        purchasePrice = 0m;
        sellingPrice = 0m;
        marketItemId = 0;
    }
}