using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class QualityAdminPanel : BaseAdminPanel
{
    private string qualityName = "";
    private float qualityGrowthBuff = 0f;

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
        GUILayout.Box("Quality Management Panel", GUILayout.ExpandWidth(true));

        // Status message if any
        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Box(operationStatus, GUILayout.ExpandWidth(true));
        }

        // Display current data
        GUILayout.Box("Current Qualities", GUILayout.ExpandWidth(true));
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

        // Add new quality section
        GUILayout.Space(20);
        GUILayout.Box("Add New Quality", GUILayout.ExpandWidth(true));

        // Input fields
        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Quality Name:");
        qualityName = GUILayout.TextField(qualityName, GUILayout.Width(200));

        GUILayout.Label("Growth Rate Buff %:");
        string buffString = GUILayout.TextField(qualityGrowthBuff.ToString(), GUILayout.Width(100));
        float.TryParse(buffString, out qualityGrowthBuff);

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Quality", GUILayout.Width(100)))
        {
            ProcessAsyncOperation(InsertQuality());
        }
        GUI.enabled = true;

        GUILayout.EndVertical();

        // End the scroll view
        GUILayout.EndScrollView();

        // End the main area
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
                        QualityID as 'ID',
                        QualityName as 'Name',
                        GrowthRateBuffPercentage as 'Growth Buff %'
                    FROM Quality
                    ORDER BY QualityID";

                using (var command = new SqlCommand(query, connection))
                {
                    var adapter = new SqlDataAdapter(command);
                    currentData = new DataTable();
                    adapter.Fill(currentData);
                    Debug.Log($"Loaded {currentData.Rows.Count} quality records");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error refreshing quality data: {e.Message}");
            throw;
        }
    }

    private async Task InsertQuality()
    {
        if (string.IsNullOrEmpty(qualityName))
        {
            throw new Exception("Quality name cannot be empty");
        }

        try
        {
            using (var connection = new SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                string getMaxIdQuery = "SELECT ISNULL(MAX(QualityID), 0) FROM Quality";
                int nextId;

                using (var command = new SqlCommand(getMaxIdQuery, connection))
                {
                    var result = await command.ExecuteScalarAsync();
                    nextId = Convert.ToInt32(result) + 1;
                }

                string insertQuery = "INSERT INTO Quality (QualityID, QualityName, GrowthRateBuffPercentage) VALUES (@ID, @Name, @Buff)";

                using (var command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", nextId);
                    command.Parameters.AddWithValue("@Name", qualityName);
                    command.Parameters.AddWithValue("@Buff", qualityGrowthBuff);
                    await command.ExecuteNonQueryAsync();
                }

                Debug.Log($"Successfully inserted quality: {qualityName}");
                ClearInputs();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inserting quality: {e.Message}");
            throw;
        }
    }

    private void ClearInputs()
    {
        qualityName = "";
        qualityGrowthBuff = 0f;
    }
}