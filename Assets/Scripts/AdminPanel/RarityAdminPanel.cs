using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class RarityAdminPanel : BaseAdminPanel
{
    private string rarityType = "";
    private float rarityPriceBuff = 0f;
    private float rarityProbability = 0f;
    private Color rarityColor = Color.white;
    private Texture2D colorPreviewTexture;

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
        GUILayout.Box("Rarity Management Panel", GUILayout.ExpandWidth(true));

        // Status message if any
        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Box(operationStatus, GUILayout.ExpandWidth(true));
        }

        // Display current data
        GUILayout.Box("Current Rarities", GUILayout.ExpandWidth(true));
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

        // Add new rarity section
        GUILayout.Space(20);
        GUILayout.Box("Add New Rarity", GUILayout.ExpandWidth(true));

        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Rarity Type:");
        rarityType = GUILayout.TextField(rarityType, GUILayout.Width(200));

        GUILayout.Label("Price Buff %:");
        string priceBuffStr = GUILayout.TextField(rarityPriceBuff.ToString(), GUILayout.Width(100));
        float.TryParse(priceBuffStr, out rarityPriceBuff);

        GUILayout.Label("Probability %:");
        string probStr = GUILayout.TextField(rarityProbability.ToString(), GUILayout.Width(100));
        float.TryParse(probStr, out rarityProbability);

        GUILayout.Label("Rarity Color:");
        GUILayout.BeginHorizontal();
        GUILayout.Label("R:", GUILayout.Width(20));
        string rStr = GUILayout.TextField((rarityColor.r * 255).ToString(), GUILayout.Width(50));
        float.TryParse(rStr, out float r);
        rarityColor.r = Mathf.Clamp01(r / 255f);

        GUILayout.Label("G:", GUILayout.Width(20));
        string gStr = GUILayout.TextField((rarityColor.g * 255).ToString(), GUILayout.Width(50));
        float.TryParse(gStr, out float g);
        rarityColor.g = Mathf.Clamp01(g / 255f);

        GUILayout.Label("B:", GUILayout.Width(20));
        string bStr = GUILayout.TextField((rarityColor.b * 255).ToString(), GUILayout.Width(50));
        float.TryParse(bStr, out float b);
        rarityColor.b = Mathf.Clamp01(b / 255f);
        GUILayout.EndHorizontal();

        // Color preview
        GUILayout.Label("Color Preview:");
        if (colorPreviewTexture != null) Destroy(colorPreviewTexture);
        colorPreviewTexture = new Texture2D(1, 1);
        colorPreviewTexture.SetPixel(0, 0, rarityColor);
        colorPreviewTexture.Apply();
        GUILayout.Box(colorPreviewTexture, GUILayout.Width(100), GUILayout.Height(20));

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Rarity", GUILayout.Width(100)))
        {
            ProcessAsyncOperation(InsertRarity());
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
                        RarityType as 'Type',
                        PriceBuffPercentage as 'Price Buff %',
                        Probability as 'Probability %',
                        CONCAT(RarityColorRed, ', ', RarityColorGreen, ', ', RarityColorBlue) as 'RGB Color'
                    FROM Rarity
                    ORDER BY Probability DESC";

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
            Debug.LogError($"Error refreshing rarity data: {e.Message}");
            throw;
        }
    }

    private async Task InsertRarity()
    {
        try
        {
            using (var connection = new SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Rarity (RarityType, PriceBuffPercentage, Probability, RarityColorRed, RarityColorGreen, RarityColorBlue) " +
                             "VALUES (@Type, @Buff, @Probability, @Red, @Green, @Blue)";

                using (var command = new SqlCommand(query, connection))
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
            ClearInputs();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inserting rarity: {e.Message}");
            throw;
        }
    }

    private void ClearInputs()
    {
        rarityType = "";
        rarityPriceBuff = 0f;
        rarityProbability = 0f;
        rarityColor = Color.white;
    }

    private void OnDestroy()
    {
        if (colorPreviewTexture != null)
        {
            Destroy(colorPreviewTexture);
        }
    }
}