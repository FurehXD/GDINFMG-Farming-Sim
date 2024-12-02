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
        ProcessAsyncOperation(RefreshData());
    }

    private void OnGUI()
    {
        if (!gameObject.activeSelf || GUI.skin == null) return;

        if (backgroundStyle == null)
        {
            InitializeStyles();
            return;
        }

        // Draw the background
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none, backgroundStyle);

        // Begin the main area
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        try
        {
            DisplayHeader();
            DisplayCurrentData();
            DisplayAddNewRarity();
        }
        finally
        {
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }

    private void DisplayHeader()
    {
        GUILayout.Box("Rarity Management Panel", backgroundStyle, GUILayout.ExpandWidth(true));

        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Box(operationStatus, backgroundStyle, GUILayout.ExpandWidth(true));
        }
    }

    private void DisplayCurrentData()
    {
        GUILayout.Box("Current Rarities", backgroundStyle, GUILayout.ExpandWidth(true));

        if (currentData != null && currentData.Rows.Count > 0)
        {
            // Headers
            GUILayout.BeginHorizontal();
            foreach (DataColumn column in currentData.Columns)
            {
                GUILayout.Box(column.ColumnName, backgroundStyle, GUILayout.Width(120));
            }
            GUILayout.EndHorizontal();

            // Data rows
            foreach (DataRow row in currentData.Rows)
            {
                GUILayout.BeginHorizontal();
                foreach (var item in row.ItemArray)
                {
                    GUILayout.Box(item.ToString(), backgroundStyle, GUILayout.Width(120));
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("No data available", labelStyle);
        }
    }

    private void DisplayAddNewRarity()
    {
        GUILayout.Space(20);
        GUILayout.Box("Add New Rarity", backgroundStyle, GUILayout.ExpandWidth(true));

        GUILayout.BeginVertical(backgroundStyle);
        try
        {
            rarityType = DrawInputField("Rarity Type:", rarityType);
            rarityPriceBuff = DrawFloatField("Price Buff %:", rarityPriceBuff);
            rarityProbability = DrawFloatField("Probability %:", rarityProbability);

            GUILayout.Label("Rarity Color:", labelStyle);
            DisplayColorPicker();
            DisplayColorPreview();

            GUI.enabled = !isOperationInProgress;
            if (GUILayout.Button("Add Rarity", buttonStyle, GUILayout.Width(100)))
            {
                ProcessAsyncOperation(InsertRarity());
            }
            GUI.enabled = true;
        }
        finally
        {
            GUILayout.EndVertical();
        }
    }

    private void DisplayColorPicker()
    {
        GUILayout.BeginHorizontal();

        float r = rarityColor.r * 255;
        float g = rarityColor.g * 255;
        float b = rarityColor.b * 255;

        r = DrawFloatField("R:", r, 50);
        g = DrawFloatField("G:", g, 50);
        b = DrawFloatField("B:", b, 50);

        rarityColor.r = Mathf.Clamp01(r / 255f);
        rarityColor.g = Mathf.Clamp01(g / 255f);
        rarityColor.b = Mathf.Clamp01(b / 255f);

        GUILayout.EndHorizontal();
    }

    private void DisplayColorPreview()
    {
        GUILayout.Label("Color Preview:", labelStyle);
        if (colorPreviewTexture != null)
        {
            Destroy(colorPreviewTexture);
        }

        colorPreviewTexture = new Texture2D(1, 1);
        colorPreviewTexture.SetPixel(0, 0, rarityColor);
        colorPreviewTexture.Apply();
        GUILayout.Box(colorPreviewTexture, GUILayout.Width(100), GUILayout.Height(20));
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
                        CAST(Probability * 100 as decimal(10,2)) as 'Probability %',
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

                // Get next RarityID
                string getMaxRarityIDQuery = "SELECT ISNULL(MAX(RarityID), 0) FROM Rarity;";
                int nextRarityID;
                using (var command = new SqlCommand(getMaxRarityIDQuery, connection))
                {
                    nextRarityID = Convert.ToInt32(await command.ExecuteScalarAsync()) + 1;
                }

                string query = @"
                    INSERT INTO Rarity 
                    (RarityID, RarityType, PriceBuffPercentage, Probability, RarityColorRed, RarityColorGreen, RarityColorBlue) 
                    VALUES 
                    (@RarityID, @Type, @Buff, @Probability, @Red, @Green, @Blue)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RarityID", nextRarityID);
                    command.Parameters.AddWithValue("@Type", rarityType);
                    command.Parameters.AddWithValue("@Buff", rarityPriceBuff);
                    command.Parameters.AddWithValue("@Probability", rarityProbability / 100f); // Convert percentage to decimal
                    command.Parameters.AddWithValue("@Red", (int)(rarityColor.r * 255));
                    command.Parameters.AddWithValue("@Green", (int)(rarityColor.g * 255));
                    command.Parameters.AddWithValue("@Blue", (int)(rarityColor.b * 255));
                    await command.ExecuteNonQueryAsync();
                }

                ClearInputs();
                await RefreshData();
            }
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