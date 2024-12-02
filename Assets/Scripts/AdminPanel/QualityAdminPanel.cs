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

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Quality Management Panel", GUI.skin.box);

        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Label(operationStatus, GUI.skin.box);
        }

        // Show current data
        GUILayout.Label("Current Qualities", GUI.skin.box);
        DrawDataTable();

        // Add new quality section
        GUILayout.Space(20);
        GUILayout.Label("Add New Quality", GUI.skin.box);

        GUILayout.Label("Quality Name:");
        qualityName = GUILayout.TextField(qualityName, GUILayout.Width(200));

        GUILayout.Label("Growth Rate Buff %:");
        float.TryParse(GUILayout.TextField(qualityGrowthBuff.ToString(), GUILayout.Width(100)), out qualityGrowthBuff);

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Quality"))
        {
            ProcessAsyncOperation(InsertQuality());
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
                string query = "SELECT QualityID, QualityName, GrowthRateBuffPercentage FROM Quality ORDER BY QualityID";

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
            Debug.LogError($"Error refreshing quality data: {e.Message}");
            throw;
        }
    }

    private async Task InsertQuality()
    {
        try
        {
            using (var connection = new SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();

                string getMaxIdQuery = "SELECT MAX(QualityID) FROM Quality";
                int nextId;

                using (var command = new SqlCommand(getMaxIdQuery, connection))
                {
                    var result = await command.ExecuteScalarAsync();
                    nextId = (result != DBNull.Value ? Convert.ToInt32(result) : 0) + 1;
                }

                string insertQuery = "INSERT INTO Quality (QualityID, QualityName, GrowthRateBuffPercentage) VALUES (@ID, @Name, @Buff)";

                using (var command = new SqlCommand(insertQuery, connection))
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
}