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

    private void OnGUI()
    {
        if (!gameObject.activeSelf) return;

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Rarity Management Panel", GUI.skin.box);

        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Label(operationStatus, GUI.skin.box);
        }

        // Show current data
        GUILayout.Label("Current Rarities", GUI.skin.box);
        DrawDataTable();

        // Add new rarity section
        GUILayout.Space(20);
        GUILayout.Label("Add New Rarity", GUI.skin.box);

        GUILayout.Label("Rarity Type:");
        rarityType = GUILayout.TextField(rarityType, GUILayout.Width(200));

        GUILayout.Label("Price Buff %:");
        float.TryParse(GUILayout.TextField(rarityPriceBuff.ToString(), GUILayout.Width(100)), out rarityPriceBuff);

        GUILayout.Label("Probability %:");
        float.TryParse(GUILayout.TextField(rarityProbability.ToString(), GUILayout.Width(100)), out rarityProbability);

        GUILayout.Label("Rarity Color:");
        GUILayout.BeginHorizontal();
        GUILayout.Label("R:");
        float.TryParse(GUILayout.TextField(rarityColor.r.ToString(), GUILayout.Width(50)), out float r);
        rarityColor.r = Mathf.Clamp01(r);

        GUILayout.Label("G:");
        float.TryParse(GUILayout.TextField(rarityColor.g.ToString(), GUILayout.Width(50)), out float g);
        rarityColor.g = Mathf.Clamp01(g);

        GUILayout.Label("B:");
        float.TryParse(GUILayout.TextField(rarityColor.b.ToString(), GUILayout.Width(50)), out float b);
        rarityColor.b = Mathf.Clamp01(b);
        GUILayout.EndHorizontal();

        // Color preview
        Texture2D colorPreview = new Texture2D(1, 1);
        colorPreview.SetPixel(0, 0, rarityColor);
        colorPreview.Apply();
        GUILayout.Box(colorPreview, GUILayout.Width(100), GUILayout.Height(20));

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Rarity"))
        {
            ProcessAsyncOperation(InsertRarity());
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
                        RarityType,
                        PriceBuffPercentage,
                        Probability,
                        CONCAT(RarityColorRed, ', ', RarityColorGreen, ', ', RarityColorBlue) as Color
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
            Debug.Log("Rarity added successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inserting rarity: {e.Message}");
            throw;
        }
    }
}