using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

public class UpgradeAdminPanel : BaseAdminPanel
{
    private string upgradeName = "";
    private string description = "";
    private List<(int id, string name)> existingItems = new List<(int id, string name)>();
    private List<(int id, decimal price)> existingMarkets = new List<(int id, decimal price)>();
    private int selectedItemId = 24; // Starting from ItemID 24 for upgrades
    private int selectedMarketId = 1;

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

                // Load Upgrade-related Items
                string itemQuery = @"
                    SELECT ItemID, ItemName 
                    FROM Items 
                    WHERE ItemCategory = 'Upgrade'
                    ORDER BY ItemID";
                using (var command = new SqlCommand(itemQuery, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    existingItems.Clear();
                    while (await reader.ReadAsync())
                    {
                        existingItems.Add((
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
                        u.UpgradeID as 'ID',
                        u.UpgradeName as 'Name',
                        u.Description,
                        i.ItemName as 'Item',
                        m.SellingPrice as 'Price'
                    FROM Upgrades u
                    JOIN Items i ON u.ItemID = i.ItemID
                    JOIN Market m ON u.MarketID = m.MarketID
                    ORDER BY u.UpgradeID";

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
            Debug.LogError($"Error refreshing upgrade data: {e.Message}");
            throw;
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

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none, backgroundStyle);

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        try
        {
            DisplayHeader();
            DisplayCurrentData();
            DisplayAddNewUpgrade();
        }
        finally
        {
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }

    private void DisplayHeader()
    {
        GUILayout.Box("Upgrade Management Panel", backgroundStyle, GUILayout.ExpandWidth(true));

        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Box(operationStatus, backgroundStyle, GUILayout.ExpandWidth(true));
        }
    }

    private void DisplayCurrentData()
    {
        GUILayout.Box("Current Upgrades", backgroundStyle, GUILayout.ExpandWidth(true));

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

    private void DisplayAddNewUpgrade()
    {
        GUILayout.Space(20);
        GUILayout.Box("Add New Upgrade", backgroundStyle, GUILayout.ExpandWidth(true));

        GUILayout.BeginVertical(backgroundStyle);
        try
        {
            upgradeName = DrawInputField("Upgrade Name:", upgradeName);
            description = DrawInputField("Description:", description);

            // Market Selection
            GUILayout.Label("Select Market:", labelStyle);
            if (existingMarkets.Count > 0)
            {
                int index = existingMarkets.FindIndex(m => m.id == selectedMarketId);
                string[] options = existingMarkets.Select(m => $"{m.id} - ${m.price}").ToArray();
                index = GUILayout.SelectionGrid(index, options, 1, buttonStyle);
                if (index >= 0 && index < existingMarkets.Count)
                {
                    selectedMarketId = existingMarkets[index].id;
                }
            }

            GUI.enabled = !isOperationInProgress;
            if (GUILayout.Button("Add Upgrade", buttonStyle, GUILayout.Width(100)))
            {
                ProcessAsyncOperation(InsertUpgrade());
            }
            GUI.enabled = true;
        }
        finally
        {
            GUILayout.EndVertical();
        }
    }

    private async Task InsertUpgrade()
    {
        try
        {
            if (string.IsNullOrEmpty(upgradeName))
            {
                throw new Exception("Upgrade name cannot be empty");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new Exception("Description cannot be empty");
            }

            using (var connection = new SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // First, insert into Items table
                        string getMaxItemIDQuery = "SELECT ISNULL(MAX(ItemID), 0) FROM Items;";
                        int nextItemID;
                        using (var command = new SqlCommand(getMaxItemIDQuery, connection, transaction))
                        {
                            nextItemID = Convert.ToInt32(await command.ExecuteScalarAsync()) + 1;
                        }

                        string insertItemQuery = @"
                        INSERT INTO Items 
                        (ItemID, ItemName, ItemCategory) 
                        VALUES 
                        (@ItemID, @Name, @Category)";

                        using (var command = new SqlCommand(insertItemQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ItemID", nextItemID);
                            command.Parameters.AddWithValue("@Name", upgradeName);
                            command.Parameters.AddWithValue("@Category", "Upgrade");
                            await command.ExecuteNonQueryAsync();
                        }

                        // Then, insert into Upgrades table
                        string getMaxUpgradeIDQuery = "SELECT ISNULL(MAX(UpgradeID), 0) FROM Upgrades;";
                        int nextUpgradeID;
                        using (var command = new SqlCommand(getMaxUpgradeIDQuery, connection, transaction))
                        {
                            nextUpgradeID = Convert.ToInt32(await command.ExecuteScalarAsync()) + 1;
                        }

                        string insertUpgradeQuery = @"
                        INSERT INTO Upgrades 
                        (UpgradeID, UpgradeName, Description, ItemID, MarketID) 
                        VALUES 
                        (@UpgradeID, @Name, @Description, @ItemID, @MarketID)";

                        using (var command = new SqlCommand(insertUpgradeQuery, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@UpgradeID", nextUpgradeID);
                            command.Parameters.AddWithValue("@Name", upgradeName);
                            command.Parameters.AddWithValue("@Description", description);
                            command.Parameters.AddWithValue("@ItemID", nextItemID);  // Use the newly created ItemID
                            command.Parameters.AddWithValue("@MarketID", selectedMarketId);
                            await command.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                        Debug.Log($"Successfully added upgrade {upgradeName} with ItemID {nextItemID}");

                        ClearInputs();
                        await LoadExistingData(); // Reload the items list
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
            Debug.LogError($"Error inserting upgrade: {e.Message}");
            throw;
        }
    }

    private void ClearInputs()
    {
        upgradeName = "";
        description = "";
        selectedItemId = 24;
        selectedMarketId = 1;
    }
}