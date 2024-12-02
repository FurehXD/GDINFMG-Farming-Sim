using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class ItemAdminPanel : BaseAdminPanel
{
    private string itemName = "";
    private int selectedCategoryIndex = 0;
    private readonly string[] categories = { "Crop", "Fertilizer", "Upgrade" };

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

        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none, backgroundStyle);

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        
        try
        {
            DisplayHeader();
            DisplayCurrentData();
            DisplayAddNewItem();
        }
        finally
        {
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }

    private void DisplayHeader()
    {
        GUILayout.Box("Item Management Panel", backgroundStyle, GUILayout.ExpandWidth(true));
        
        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Box(operationStatus, backgroundStyle, GUILayout.ExpandWidth(true));
        }
    }

    private void DisplayCurrentData()
    {
        GUILayout.Box("Current Items", backgroundStyle, GUILayout.ExpandWidth(true));
        
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

    private void DisplayAddNewItem()
    {
        GUILayout.Space(20);
        GUILayout.Box("Add New Item", backgroundStyle, GUILayout.ExpandWidth(true));

        GUILayout.BeginVertical(backgroundStyle);
        try
        {
            itemName = DrawInputField("Item Name:", itemName);

            // Category Selection
            GUILayout.Label("Item Category:", labelStyle);
            GUILayout.BeginHorizontal();
            for (int i = 0; i < categories.Length; i++)
            {
                if (GUILayout.Toggle(selectedCategoryIndex == i, categories[i], buttonStyle))
                {
                    selectedCategoryIndex = i;
                }
            }
            GUILayout.EndHorizontal();

            GUI.enabled = !isOperationInProgress;
            if (GUILayout.Button("Add Item", buttonStyle, GUILayout.Width(100)))
            {
                ProcessAsyncOperation(InsertItem());
            }
            GUI.enabled = true;
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
                    ItemID as 'ID',
                    ItemName as 'Name',
                    ItemCategory as 'Category'
                FROM Items
                ORDER BY ItemID";

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
            Debug.LogError($"Error refreshing item data: {e.Message}");
            throw;
        }
    }

    private async Task InsertItem()
    {
        try
        {
            if (string.IsNullOrEmpty(itemName))
            {
                throw new Exception("Item name cannot be empty");
            }

            using (var connection = new SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                // Get next ItemID
                string getMaxItemIDQuery = "SELECT ISNULL(MAX(ItemID), 0) FROM Items;";
                int nextItemID;
                using (var command = new SqlCommand(getMaxItemIDQuery, connection))
                {
                    nextItemID = Convert.ToInt32(await command.ExecuteScalarAsync()) + 1;
                }

                string query = @"
                INSERT INTO Items 
                (ItemID, ItemName, ItemCategory) 
                VALUES 
                (@ItemID, @Name, @Category)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ItemID", nextItemID);
                    command.Parameters.AddWithValue("@Name", itemName);
                    command.Parameters.AddWithValue("@Category", categories[selectedCategoryIndex]);
                    await command.ExecuteNonQueryAsync();
                }

                ClearInputs();
                await RefreshData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inserting item: {e.Message}");
            throw;
        }
    }

    private void ClearInputs()
    {
        itemName = "";
        selectedCategoryIndex = 0;
    }
}