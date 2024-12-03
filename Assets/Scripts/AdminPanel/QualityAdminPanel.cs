using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class QualityAdminPanel : BaseAdminPanel
{
    private string qualityName = "";
    private float qualityGrowthBuff = 0f;
    private bool isEditing = false;
    private int editingQualityId = -1;
    private DataRow selectedRow;

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
        DisplayCurrentData();

        // Add/Edit quality section
        GUILayout.Space(20);
        DisplayQualitySection();

        // End the scroll view
        GUILayout.EndScrollView();

        // End the main area
        GUILayout.EndArea();
    }

    private void DisplayCurrentData()
    {
        GUILayout.Box("Current Qualities", GUILayout.ExpandWidth(true));
        if (currentData != null && currentData.Rows.Count > 0)
        {
            // Headers
            GUILayout.BeginHorizontal();
            foreach (DataColumn column in currentData.Columns)
            {
                GUILayout.Box(column.ColumnName, GUILayout.Width(120));
            }
            GUILayout.Box("Actions", GUILayout.Width(120));
            GUILayout.EndHorizontal();

            // Data rows
            foreach (DataRow row in currentData.Rows)
            {
                GUILayout.BeginHorizontal();
                foreach (var item in row.ItemArray)
                {
                    GUILayout.Box(item.ToString(), GUILayout.Width(120));
                }

                // Edit button
                if (!isEditing || editingQualityId != Convert.ToInt32(row["ID"]))
                {
                    if (GUILayout.Button("Edit", GUILayout.Width(120)))
                    {
                        StartEditing(row);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("No data available");
        }
    }

    private void DisplayQualitySection()
    {
        GUILayout.Box(isEditing ? "Edit Quality" : "Add New Quality", GUILayout.ExpandWidth(true));

        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Quality Name:");
        qualityName = GUILayout.TextField(qualityName, GUILayout.Width(200));

        GUILayout.Label("Growth Rate Buff %:");
        string buffString = GUILayout.TextField(qualityGrowthBuff.ToString(), GUILayout.Width(100));
        float.TryParse(buffString, out qualityGrowthBuff);

        GUI.enabled = !isOperationInProgress;

        GUILayout.BeginHorizontal();
        if (isEditing)
        {
            if (GUILayout.Button("Update Quality", GUILayout.Width(100)))
            {
                ProcessAsyncOperation(UpdateQuality());
            }
            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                CancelEditing();
            }
        }
        else
        {
            if (GUILayout.Button("Add Quality", GUILayout.Width(100)))
            {
                ProcessAsyncOperation(InsertQuality());
            }
        }
        GUILayout.EndHorizontal();

        GUI.enabled = true;

        GUILayout.EndVertical();
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

    private async Task UpdateQuality()
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

                string updateQuery = @"
                    UPDATE Quality 
                    SET QualityName = @Name, 
                        GrowthRateBuffPercentage = @Buff 
                    WHERE QualityID = @ID";

                using (var command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", editingQualityId);
                    command.Parameters.AddWithValue("@Name", qualityName);
                    command.Parameters.AddWithValue("@Buff", qualityGrowthBuff);
                    await command.ExecuteNonQueryAsync();
                }

                Debug.Log($"Successfully updated quality: {qualityName}");
                CancelEditing();
                await RefreshData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating quality: {e.Message}");
            throw;
        }
    }

    private void StartEditing(DataRow row)
    {
        isEditing = true;
        editingQualityId = Convert.ToInt32(row["ID"]);
        selectedRow = row;

        // Populate fields with current values
        qualityName = row["Name"].ToString();
        qualityGrowthBuff = Convert.ToSingle(row["Growth Buff %"]);
    }

    private void CancelEditing()
    {
        isEditing = false;
        editingQualityId = -1;
        selectedRow = null;
        ClearInputs();
    }


    private void ClearInputs()
    {
        qualityName = "";
        qualityGrowthBuff = 0f;
    }
}