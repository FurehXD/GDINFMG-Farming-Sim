using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class PlotAdminPanel : BaseAdminPanel
{
    private string plotName = "";
    private float plotGrowthBuff = 0f;
    private int gridSizeX = 1;
    private int gridSizeY = 1;

    private void OnGUI()
    {
        if (!gameObject.activeSelf) return;

        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Plot Management Panel", GUI.skin.box);

        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Label(operationStatus, GUI.skin.box);
        }

        // Show current data
        GUILayout.Label("Current Plots", GUI.skin.box);
        DrawDataTable();

        // Add new plot section
        GUILayout.Space(20);
        GUILayout.Label("Add New Plot", GUI.skin.box);

        GUILayout.Label("Plot Name:");
        plotName = GUILayout.TextField(plotName, GUILayout.Width(200));

        GUILayout.Label("Universal Growth Buff %:");
        float.TryParse(GUILayout.TextField(plotGrowthBuff.ToString(), GUILayout.Width(100)), out plotGrowthBuff);

        GUILayout.Label("Grid Size X:");
        int.TryParse(GUILayout.TextField(gridSizeX.ToString(), GUILayout.Width(100)), out gridSizeX);

        GUILayout.Label("Grid Size Y:");
        int.TryParse(GUILayout.TextField(gridSizeY.ToString(), GUILayout.Width(100)), out gridSizeY);

        // Preview grid size
        GUILayout.Label("Grid Size Preview:");
        GUILayout.Box("", GUILayout.Width(gridSizeX * 20), GUILayout.Height(gridSizeY * 20));

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Plot"))
        {
            ProcessAsyncOperation(InsertPlot());
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
                        PlotName,
                        UniversalGrowthBuffPercentage,
                        GridSizeX,
                        GridSizeY,
                        CONCAT(GridSizeX, ' x ', GridSizeY) as GridSize
                    FROM Plot
                    ORDER BY PlotName";

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
            Debug.LogError($"Error refreshing plot data: {e.Message}");
            throw;
        }
    }

    private async Task InsertPlot()
    {
        try
        {
            using (var connection = new SqlConnection(DatabaseManager.Instance.ConnectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Plot (PlotName, UniversalGrowthBuffPercentage, GridSizeX, GridSizeY) " +
                             "VALUES (@Name, @Buff, @SizeX, @SizeY)";

                using (var command = new SqlCommand(query, connection))
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
            throw;
        }
    }
}