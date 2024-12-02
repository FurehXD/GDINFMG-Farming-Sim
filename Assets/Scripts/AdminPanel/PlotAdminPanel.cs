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
    private Texture2D gridPreviewTexture;

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
        GUILayout.Box("Plot Management Panel", GUILayout.ExpandWidth(true));

        // Status message if any
        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Box(operationStatus, GUILayout.ExpandWidth(true));
        }

        // Display current data
        GUILayout.Box("Current Plots", GUILayout.ExpandWidth(true));
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

        // Add new plot section
        GUILayout.Space(20);
        GUILayout.Box("Add New Plot", GUILayout.ExpandWidth(true));

        GUILayout.BeginVertical(GUI.skin.box);

        GUILayout.Label("Plot Name:");
        plotName = GUILayout.TextField(plotName, GUILayout.Width(200));

        GUILayout.Label("Universal Growth Buff %:");
        string buffStr = GUILayout.TextField(plotGrowthBuff.ToString(), GUILayout.Width(100));
        float.TryParse(buffStr, out plotGrowthBuff);

        GUILayout.Label("Grid Size X:");
        string xSizeStr = GUILayout.TextField(gridSizeX.ToString(), GUILayout.Width(100));
        int.TryParse(xSizeStr, out gridSizeX);

        GUILayout.Label("Grid Size Y:");
        string ySizeStr = GUILayout.TextField(gridSizeY.ToString(), GUILayout.Width(100));
        int.TryParse(ySizeStr, out gridSizeY);

        // Grid preview
        GUILayout.Label("Grid Size Preview:");
        DrawGridPreview();

        GUI.enabled = !isOperationInProgress;
        if (GUILayout.Button("Add Plot", GUILayout.Width(100)))
        {
            ProcessAsyncOperation(InsertPlot());
        }
        GUI.enabled = true;

        GUILayout.EndVertical();

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DrawGridPreview()
    {
        float cellSize = 20f;
        float previewWidth = gridSizeX * cellSize;
        float previewHeight = gridSizeY * cellSize;

        // Limit preview size to reasonable dimensions
        float maxSize = 400f;
        if (previewWidth > maxSize || previewHeight > maxSize)
        {
            float scale = Mathf.Min(maxSize / previewWidth, maxSize / previewHeight);
            previewWidth *= scale;
            previewHeight *= scale;
            cellSize *= scale;
        }

        // Create preview texture
        if (gridPreviewTexture != null)
        {
            Destroy(gridPreviewTexture);
        }

        gridPreviewTexture = new Texture2D((int)previewWidth, (int)previewHeight);
        Color gridColor = new Color(0.3f, 0.3f, 0.3f);
        Color lineColor = new Color(0.7f, 0.7f, 0.7f);

        // Fill background
        for (int x = 0; x < previewWidth; x++)
        {
            for (int y = 0; y < previewHeight; y++)
            {
                gridPreviewTexture.SetPixel(x, y, gridColor);
            }
        }

        // Draw grid lines
        for (int x = 0; x <= gridSizeX; x++)
        {
            int xPos = (int)(x * cellSize);
            if (xPos < previewWidth)
            {
                for (int y = 0; y < previewHeight; y++)
                {
                    gridPreviewTexture.SetPixel(xPos, y, lineColor);
                }
            }
        }

        for (int y = 0; y <= gridSizeY; y++)
        {
            int yPos = (int)(y * cellSize);
            if (yPos < previewHeight)
            {
                for (int x = 0; x < previewWidth; x++)
                {
                    gridPreviewTexture.SetPixel(x, yPos, lineColor);
                }
            }
        }

        gridPreviewTexture.Apply();

        // Draw the preview with a border
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Box(gridPreviewTexture, GUILayout.Width(previewWidth), GUILayout.Height(previewHeight));
        GUILayout.Label($"Grid Size: {gridSizeX} x {gridSizeY}", labelStyle);
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
                        PlotName as 'Name',
                        UniversalGrowthBuffPercentage as 'Growth Buff %',
                        GridSizeX as 'Width',
                        GridSizeY as 'Height',
                        CONCAT(GridSizeX, ' x ', GridSizeY) as 'Grid Size'
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
            // Validate input
            if (string.IsNullOrEmpty(plotName))
            {
                throw new Exception("Plot name cannot be empty");
            }

            if (gridSizeX < 1 || gridSizeY < 1)
            {
                throw new Exception("Grid size must be at least 1x1");
            }

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
            ClearInputs();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error inserting plot: {e.Message}");
            throw;
        }
    }

    private void ClearInputs()
    {
        plotName = "";
        plotGrowthBuff = 0f;
        gridSizeX = 1;
        gridSizeY = 1;
    }

    private void OnDestroy()
    {
        if (gridPreviewTexture != null)
        {
            Destroy(gridPreviewTexture);
        }
    }
}