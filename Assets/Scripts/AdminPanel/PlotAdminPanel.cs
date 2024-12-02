using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class PlotAdminPanel : BaseAdminPanel
{
    private string plotName = "";
    private float plotGrowthBuff = 1.0f;
    private int gridSizeX = 1;
    private int gridSizeY = 1;
    private int assetId = 0;
    private Texture2D gridPreviewTexture;

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
            DisplayAddNewPlot();
        }
        finally
        {
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }

    private void DisplayHeader()
    {
        GUILayout.Box("Plot Management Panel", backgroundStyle, GUILayout.ExpandWidth(true));

        if (!string.IsNullOrEmpty(operationStatus))
        {
            GUILayout.Box(operationStatus, backgroundStyle, GUILayout.ExpandWidth(true));
        }
    }

    private void DisplayCurrentData()
    {
        GUILayout.Box("Current Plots", backgroundStyle, GUILayout.ExpandWidth(true));

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

    private void DisplayAddNewPlot()
    {
        GUILayout.Space(20);
        GUILayout.Box("Add New Plot", backgroundStyle, GUILayout.ExpandWidth(true));

        GUILayout.BeginVertical(backgroundStyle);
        try
        {
            plotName = DrawInputField("Plot Name:", plotName);
            plotGrowthBuff = DrawFloatField("Growth Buff:", plotGrowthBuff);
            gridSizeX = DrawIntField("Grid Size X:", gridSizeX);
            gridSizeY = DrawIntField("Grid Size Y:", gridSizeY);
            assetId = DrawIntField("Asset ID:", assetId);

            DrawGridPreview();

            GUI.enabled = !isOperationInProgress;
            if (GUILayout.Button("Add Plot", buttonStyle, GUILayout.Width(100)))
            {
                ProcessAsyncOperation(InsertPlot());
            }
            GUI.enabled = true;
        }
        finally
        {
            GUILayout.EndVertical();
        }
    }

    private void DrawGridPreview()
    {
        float cellSize = 20f;
        float previewWidth = gridSizeX * cellSize;
        float previewHeight = gridSizeY * cellSize;

        float maxSize = 400f;
        if (previewWidth > maxSize || previewHeight > maxSize)
        {
            float scale = Mathf.Min(maxSize / previewWidth, maxSize / previewHeight);
            previewWidth *= scale;
            previewHeight *= scale;
            cellSize *= scale;
        }

        if (gridPreviewTexture == null ||
            gridPreviewTexture.width != (int)previewWidth ||
            gridPreviewTexture.height != (int)previewHeight)
        {
            if (gridPreviewTexture != null)
            {
                Destroy(gridPreviewTexture);
            }
            gridPreviewTexture = new Texture2D((int)previewWidth, (int)previewHeight);
        }

        // Draw grid
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

        // Draw lines
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

        GUILayout.Label("Grid Preview:", labelStyle);
        GUILayout.Box(gridPreviewTexture, GUILayout.Width(previewWidth), GUILayout.Height(previewHeight));
        GUILayout.Label($"Grid Size: {gridSizeX} x {gridSizeY}", labelStyle);
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
                        PlotID as 'ID',
                        PlotName as 'Name',
                        UniversalGrowthBuffPercentage as 'Growth Buff',
                        GridSizeX as 'Width',
                        GridSizeY as 'Height',
                        AssetID as 'Asset ID'
                    FROM Plots
                    ORDER BY PlotID";

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

                // Get next PlotID
                string getMaxPlotIDQuery = "SELECT ISNULL(MAX(PlotID), 0) FROM Plots;";
                int nextPlotID;
                using (var command = new SqlCommand(getMaxPlotIDQuery, connection))
                {
                    nextPlotID = Convert.ToInt32(await command.ExecuteScalarAsync()) + 1;
                }

                string query = @"
                    INSERT INTO Plots 
                    (PlotID, PlotName, UniversalGrowthBuffPercentage, GridSizeX, GridSizeY, AssetID) 
                    VALUES 
                    (@PlotID, @Name, @Buff, @SizeX, @SizeY, @AssetID)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlotID", nextPlotID);
                    command.Parameters.AddWithValue("@Name", plotName);
                    command.Parameters.AddWithValue("@Buff", plotGrowthBuff);
                    command.Parameters.AddWithValue("@SizeX", gridSizeX);
                    command.Parameters.AddWithValue("@SizeY", gridSizeY);
                    command.Parameters.AddWithValue("@AssetID", assetId);
                    await command.ExecuteNonQueryAsync();
                }

                ClearInputs();
                await RefreshData();
            }
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
        plotGrowthBuff = 1.0f;
        gridSizeX = 1;
        gridSizeY = 1;
        assetId = 0;
    }

    private void OnDestroy()
    {
        if (gridPreviewTexture != null)
        {
            Destroy(gridPreviewTexture);
        }
    }
}