using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class CropAdminPanel : BaseAdminPanel
{
    // Crop input fields
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

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Crop Management Panel", GUI.skin.box);

        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Label(operationStatus, GUI.skin.box);
        }

        GUILayout.Label("Current Crops", GUI.skin.box);
        DrawDataTable();

        GUILayout.Space(20);
        GUILayout.Label("Add New Crop", GUI.skin.box);

        cropName = DrawInputField("Crop Name:", cropName);
        growthRate = DrawFloatField("Growth Rate:", growthRate);

        GUILayout.Label("Fertilizer Details:", GUI.skin.box);
        fertilizerName = DrawInputField("Name:", fertilizerName);
        fertilizerBuff = DrawFloatField("Growth Buff %:", fertilizerBuff);
        fertilizerItemId = DrawIntField("Item ID:", fertilizerItemId);

        GUILayout.Label("Season Details:", GUI.skin.box);
        seasonName = DrawInputField("Name:", seasonName);
        fertileCrops = DrawInputField("Fertile Crops (comma-separated):", fertileCrops);
        infertileCrops = DrawInputField("Infertile Crops (comma-separated):", infertileCrops);
        seasonDuration = DrawIntField("Duration (days):", seasonDuration);

        GUILayout.Label("Market Details:", GUI.skin.box);
        purchasePrice = DrawDecimalField("Purchase Price:", purchasePrice);
        sellingPrice = DrawDecimalField("Selling Price:", sellingPrice);
        marketItemId = DrawIntField("Market Item ID:", marketItemId);

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Crop"))
        {
            ProcessAsyncOperation(InsertCrop());
        }
        GUI.enabled = true;

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
                        s.Duration as SeasonDuration,
                        m.PurchasingPrice,
                        m.SellingPrice
                    FROM Crops c
                    JOIN Growth g ON c.GrowthID = g.GrowthID
                    JOIN Fertilizer f ON c.FertilizerID = f.FertilizerID
                    JOIN Season s ON c.SeasonID = s.SeasonID
                    JOIN Market m ON c.MarketID = m.MarketID";

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

    private string DrawInputField(string label, string value)
    {
        GUILayout.Label(label);
        return GUILayout.TextField(value, GUILayout.Width(200));
    }

    private float DrawFloatField(string label, float value)
    {
        GUILayout.Label(label);
        float.TryParse(GUILayout.TextField(value.ToString(), GUILayout.Width(100)), out float result);
        return result;
    }

    private int DrawIntField(string label, int value)
    {
        GUILayout.Label(label);
        int.TryParse(GUILayout.TextField(value.ToString(), GUILayout.Width(100)), out int result);
        return result;
    }

    private decimal DrawDecimalField(string label, decimal value)
    {
        GUILayout.Label(label);
        decimal.TryParse(GUILayout.TextField(value.ToString(), GUILayout.Width(100)), out decimal result);
        return result;
    }

    private async Task InsertCrop()
    {
        try
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                // First, insert Growth data and get the ID
                string growthQuery = "INSERT INTO Growth (GrowthRate) VALUES (@GrowthRate); SELECT SCOPE_IDENTITY();";
                int growthId;
                using (var command = new System.Data.SqlClient.SqlCommand(growthQuery, connection))
                {
                    command.Parameters.AddWithValue("@GrowthRate", growthRate);
                    growthId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Insert Fertilizer data
                string fertilizerQuery = "INSERT INTO Fertilizer (FertilizerName, GrowthRateBuffPercentage, ItemID) VALUES (@Name, @Buff, @ItemID); SELECT SCOPE_IDENTITY();";
                int fertilizerId;
                using (var command = new System.Data.SqlClient.SqlCommand(fertilizerQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", fertilizerName);
                    command.Parameters.AddWithValue("@Buff", fertilizerBuff);
                    command.Parameters.AddWithValue("@ItemID", fertilizerItemId);
                    fertilizerId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Insert Season data
                string seasonQuery = "INSERT INTO Season (SeasonName, FertileCrops, InfertileCrops, Duration) VALUES (@Name, @Fertile, @Infertile, @Duration); SELECT SCOPE_IDENTITY();";
                int seasonId;
                using (var command = new System.Data.SqlClient.SqlCommand(seasonQuery, connection))
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
                using (var command = new System.Data.SqlClient.SqlCommand(marketQuery, connection))
                {
                    command.Parameters.AddWithValue("@BuyPrice", purchasePrice);
                    command.Parameters.AddWithValue("@SellPrice", sellingPrice);
                    command.Parameters.AddWithValue("@ItemID", marketItemId);
                    marketId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                // Finally, insert the Crop data
                string cropQuery = "INSERT INTO Crops (CropName, GrowthID, FertilizerID, SeasonID, MarketID) VALUES (@Name, @GrowthID, @FertilizerID, @SeasonID, @MarketID)";
                using (var command = new System.Data.SqlClient.SqlCommand(cropQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", cropName);
                    command.Parameters.AddWithValue("@GrowthID", growthId);
                    command.Parameters.AddWithValue("@FertilizerID", fertilizerId);
                    command.Parameters.AddWithValue("@SeasonID", seasonId);
                    command.Parameters.AddWithValue("@MarketID", marketId);
                    await command.ExecuteNonQueryAsync();
                }
            }
            Debug.Log("Crop added successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inserting crop: {e.Message}");
        }
    }
}