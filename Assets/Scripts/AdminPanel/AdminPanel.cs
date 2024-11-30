using UnityEngine;
using System;
using System.Threading.Tasks;

public class AdminPanel : MonoBehaviour
{
    private Vector2 scrollPosition;
    private bool showCropPanel = false;
    private bool showQualityPanel = false;
    private bool showRarityPanel = false;
    private bool showPlotPanel = false;

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

    // Quality input fields
    private string qualityName = "";
    private float qualityGrowthBuff = 0f;

    // Rarity input fields
    private string rarityType = "";
    private float rarityPriceBuff = 0f;
    private float rarityProbability = 0f;
    private Color rarityColor = Color.white;

    // Plot input fields
    private string plotName = "";
    private float plotGrowthBuff = 0f;
    private int gridSizeX = 1;
    private int gridSizeY = 1;

    // Operation status
    private bool isOperationInProgress = false;
    private string operationStatus = "";
    private float statusDisplayTime = 0f;
    private const float STATUS_DISPLAY_DURATION = 3f;

    private void Update()
    {
        // F1-F4 keys to toggle panels
        if (Input.GetKeyDown(KeyCode.F1))
        {
            showCropPanel = !showCropPanel;
            showQualityPanel = false;
            showRarityPanel = false;
            showPlotPanel = false;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            showCropPanel = false;
            showQualityPanel = !showQualityPanel;
            showRarityPanel = false;
            showPlotPanel = false;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            showCropPanel = false;
            showQualityPanel = false;
            showRarityPanel = !showRarityPanel;
            showPlotPanel = false;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            showCropPanel = false;
            showQualityPanel = false;
            showRarityPanel = false;
            showPlotPanel = !showPlotPanel;
        }

        // Handle status message timing
        if (Time.time > statusDisplayTime)
        {
            operationStatus = "";
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, Screen.height - 20));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        /*GUILayout.Label("Database Admin Panel", GUI.skin.box);
        GUILayout.Label("F1: Crops | F2: Qualities | F3: Rarities | F4: Plots", GUI.skin.box);*/

        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Label(operationStatus, GUI.skin.box);
        }

        // Panel contents
        if (showCropPanel) DrawCropPanel();
        if (showQualityPanel) DrawQualityPanel();
        if (showRarityPanel) DrawRarityPanel();
        if (showPlotPanel) DrawPlotPanel();

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DrawCropPanel()
    {
        GUILayout.Label("Add New Crop", GUI.skin.box);

        GUILayout.Label("Crop Name:");
        cropName = GUILayout.TextField(cropName, GUILayout.Width(200));

        GUILayout.Label("Growth Rate:");
        if (float.TryParse(GUILayout.TextField(growthRate.ToString(), GUILayout.Width(100)), out float newGrowthRate))
        {
            growthRate = newGrowthRate;
        }

        GUILayout.Label("Fertilizer Details:", GUI.skin.box);
        GUILayout.Label("Name:");
        fertilizerName = GUILayout.TextField(fertilizerName, GUILayout.Width(200));
        GUILayout.Label("Growth Buff %:");
        if (float.TryParse(GUILayout.TextField(fertilizerBuff.ToString(), GUILayout.Width(100)), out float newFertBuff))
        {
            fertilizerBuff = newFertBuff;
        }
        GUILayout.Label("Item ID:");
        if (int.TryParse(GUILayout.TextField(fertilizerItemId.ToString(), GUILayout.Width(100)), out int newFertItemId))
        {
            fertilizerItemId = newFertItemId;
        }

        GUILayout.Label("Season Details:", GUI.skin.box);
        GUILayout.Label("Name:");
        seasonName = GUILayout.TextField(seasonName, GUILayout.Width(200));
        GUILayout.Label("Fertile Crops (comma-separated):");
        fertileCrops = GUILayout.TextField(fertileCrops, GUILayout.Width(200));
        GUILayout.Label("Infertile Crops (comma-separated):");
        infertileCrops = GUILayout.TextField(infertileCrops, GUILayout.Width(200));
        GUILayout.Label("Duration (days):");
        if (int.TryParse(GUILayout.TextField(seasonDuration.ToString(), GUILayout.Width(100)), out int newDuration))
        {
            seasonDuration = newDuration;
        }

        GUILayout.Label("Market Details:", GUI.skin.box);
        GUILayout.Label("Purchase Price:");
        if (decimal.TryParse(GUILayout.TextField(purchasePrice.ToString(), GUILayout.Width(100)), out decimal newPurchasePrice))
        {
            purchasePrice = newPurchasePrice;
        }
        GUILayout.Label("Selling Price:");
        if (decimal.TryParse(GUILayout.TextField(sellingPrice.ToString(), GUILayout.Width(100)), out decimal newSellingPrice))
        {
            sellingPrice = newSellingPrice;
        }
        GUILayout.Label("Market Item ID:");
        if (int.TryParse(GUILayout.TextField(marketItemId.ToString(), GUILayout.Width(100)), out int newMarketItemId))
        {
            marketItemId = newMarketItemId;
        }

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Crop"))
        {
            ProcessAsyncOperation(InsertCrop());
        }
        GUI.enabled = true;
    }

    private void DrawQualityPanel()
    {
        GUILayout.Label("Add New Quality", GUI.skin.box);

        GUILayout.Label("Quality Name:");
        qualityName = GUILayout.TextField(qualityName, GUILayout.Width(200));

        GUILayout.Label("Growth Rate Buff %:");
        if (float.TryParse(GUILayout.TextField(qualityGrowthBuff.ToString(), GUILayout.Width(100)), out float newQualityBuff))
        {
            qualityGrowthBuff = newQualityBuff;
        }

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Quality"))
        {
            ProcessAsyncOperation(InsertQuality());
        }
        GUI.enabled = true;
    }

    private void DrawRarityPanel()
    {
        GUILayout.Label("Add New Rarity", GUI.skin.box);

        GUILayout.Label("Rarity Type:");
        rarityType = GUILayout.TextField(rarityType, GUILayout.Width(200));

        GUILayout.Label("Price Buff %:");
        if (float.TryParse(GUILayout.TextField(rarityPriceBuff.ToString(), GUILayout.Width(100)), out float newPriceBuff))
        {
            rarityPriceBuff = newPriceBuff;
        }

        GUILayout.Label("Probability %:");
        if (float.TryParse(GUILayout.TextField(rarityProbability.ToString(), GUILayout.Width(100)), out float newProb))
        {
            rarityProbability = newProb;
        }

        GUILayout.Label("Rarity Color:");
        GUILayout.BeginHorizontal();
        GUILayout.Label("R:");
        if (float.TryParse(GUILayout.TextField(rarityColor.r.ToString(), GUILayout.Width(50)), out float r))
        {
            rarityColor.r = Mathf.Clamp01(r);
        }
        GUILayout.Label("G:");
        if (float.TryParse(GUILayout.TextField(rarityColor.g.ToString(), GUILayout.Width(50)), out float g))
        {
            rarityColor.g = Mathf.Clamp01(g);
        }
        GUILayout.Label("B:");
        if (float.TryParse(GUILayout.TextField(rarityColor.b.ToString(), GUILayout.Width(50)), out float b))
        {
            rarityColor.b = Mathf.Clamp01(b);
        }
        GUILayout.EndHorizontal();

        // Color preview
        GUILayout.Box("", GUILayout.Width(100), GUILayout.Height(20));
        GUI.backgroundColor = rarityColor;

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Rarity"))
        {
            ProcessAsyncOperation(InsertRarity());
        }
        GUI.enabled = true;
    }

    private void DrawPlotPanel()
    {
        GUILayout.Label("Add New Plot", GUI.skin.box);

        GUILayout.Label("Plot Name:");
        plotName = GUILayout.TextField(plotName, GUILayout.Width(200));

        GUILayout.Label("Universal Growth Buff %:");
        if (float.TryParse(GUILayout.TextField(plotGrowthBuff.ToString(), GUILayout.Width(100)), out float newPlotBuff))
        {
            plotGrowthBuff = newPlotBuff;
        }

        GUILayout.Label("Grid Size X:");
        if (int.TryParse(GUILayout.TextField(gridSizeX.ToString(), GUILayout.Width(100)), out int newSizeX))
        {
            gridSizeX = newSizeX;
        }

        GUILayout.Label("Grid Size Y:");
        if (int.TryParse(GUILayout.TextField(gridSizeY.ToString(), GUILayout.Width(100)), out int newSizeY))
        {
            gridSizeY = newSizeY;
        }

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Plot"))
        {
            ProcessAsyncOperation(InsertPlot());
        }
        GUI.enabled = true;
    }

    private async void ProcessAsyncOperation(Task operation)
    {
        if (isOperationInProgress) return;

        isOperationInProgress = true;
        operationStatus = "Operation in progress...";

        try
        {
            await operation;
            operationStatus = "Operation completed successfully!";
        }
        catch (Exception e)
        {
            operationStatus = $"Error: {e.Message}";
            Debug.LogError(e);
        }
        finally
        {
            isOperationInProgress = false;
            statusDisplayTime = Time.time + STATUS_DISPLAY_DURATION;
        }
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

    private async Task InsertQuality()
    {
        try
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                string getMaxIdQuery = "SELECT MAX(QualityID) FROM Quality";
                int nextId;

                using (var command = new System.Data.SqlClient.SqlCommand(getMaxIdQuery, connection))
                {
                    var result = await command.ExecuteScalarAsync();
                    nextId = (result != DBNull.Value ? Convert.ToInt32(result) : 0) + 1;
                }

                string insertQuery = "INSERT INTO Quality (QualityID, QualityName, GrowthRateBuffPercentage) VALUES (@ID, @Name, @Buff)";

                using (var command = new System.Data.SqlClient.SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", nextId);
                    command.Parameters.AddWithValue("@Name", qualityName);
                    command.Parameters.AddWithValue("@Buff", qualityGrowthBuff);
                    await command.ExecuteNonQueryAsync();
                }
            }
            Debug.Log("Quality added successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inserting quality: {e.Message}");
            throw;
        }
    }

    private async Task InsertRarity()
    {
        try
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Rarity (RarityType, PriceBuffPercentage, Probability, RarityColorRed, RarityColorGreen, RarityColorBlue) " +
                             "VALUES (@Type, @Buff, @Probability, @Red, @Green, @Blue)";

                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Type", rarityType);
                    command.Parameters.AddWithValue("@Buff", rarityPriceBuff);
                    command.Parameters.AddWithValue("@Probability", rarityProbability);
                    command.Parameters.AddWithValue("@Red", (int)(rarityColor.r * 255));
                    command.Parameters.AddWithValue("@Green", (int)(rarityColor.g * 255));
                    command.Parameters.AddWithValue("@Blue", (int)(rarityColor.b * 255));
                    await command.ExecuteNonQueryAsync();
                }
            }
            Debug.Log("Rarity added successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inserting rarity: {e.Message}");
        }
    }

    private async Task InsertPlot()
    {
        try
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Plot (PlotName, UniversalGrowthBuffPercentage, GridSizeX, GridSizeY) " +
                             "VALUES (@Name, @Buff, @SizeX, @SizeY)";

                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", plotName);
                    command.Parameters.AddWithValue("@Buff", plotGrowthBuff);
                    command.Parameters.AddWithValue("@SizeX", gridSizeX);
                    command.Parameters.AddWithValue("@SizeY", gridSizeY);
                    await command.ExecuteNonQueryAsync();
                }
            }
            Debug.Log("Plot added successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inserting plot: {e.Message}");
        }
    }
}