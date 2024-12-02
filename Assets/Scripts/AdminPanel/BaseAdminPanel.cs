using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

public abstract class BaseAdminPanel : MonoBehaviour
{
    protected Vector2 scrollPosition;
    protected bool isOperationInProgress = false;
    protected string operationStatus = "";
    protected float statusDisplayTime = 0f;
    protected const float STATUS_DISPLAY_DURATION = 3f;
    protected DataTable currentData;

    // Background style
    protected GUIStyle backgroundStyle;
    protected GUIStyle headerStyle;
    protected GUIStyle tableHeaderStyle;
    protected GUIStyle tableCellStyle;
    protected GUIStyle inputStyle;
    protected GUIStyle buttonStyle;
    protected GUIStyle sectionStyle;
    protected GUIStyle labelStyle;

    protected virtual void Awake()
    {
        InitializeStyles();
    }

    protected virtual void OnEnable()
    {
        RefreshData();
    }

    protected virtual void Update()
    {
        if (Time.time > statusDisplayTime)
        {
            operationStatus = "";
        }
    }

    protected virtual void InitializeStyles()
    {
        // Main background style
        backgroundStyle = new GUIStyle();
        backgroundStyle.normal.background = MakeBackgroundTexture(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.95f));
        backgroundStyle.padding = new RectOffset(10, 10, 10, 10);

        // Header style
        headerStyle = new GUIStyle(GUI.skin.box);
        headerStyle.fontSize = 16;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.background = MakeBackgroundTexture(2, 2, new Color(0.3f, 0.3f, 0.3f, 1f));
        headerStyle.normal.textColor = Color.white;
        headerStyle.padding = new RectOffset(10, 10, 10, 10);
        headerStyle.margin = new RectOffset(0, 0, 5, 5);
        headerStyle.alignment = TextAnchor.MiddleCenter;

        // Table header style
        tableHeaderStyle = new GUIStyle(GUI.skin.box);
        tableHeaderStyle.normal.background = MakeBackgroundTexture(2, 2, new Color(0.4f, 0.4f, 0.4f, 1f));
        tableHeaderStyle.normal.textColor = Color.white;
        tableHeaderStyle.fontStyle = FontStyle.Bold;
        tableHeaderStyle.padding = new RectOffset(5, 5, 5, 5);
        tableHeaderStyle.alignment = TextAnchor.MiddleLeft;

        // Table cell style
        tableCellStyle = new GUIStyle(GUI.skin.box);
        tableCellStyle.normal.background = MakeBackgroundTexture(2, 2, new Color(0.25f, 0.25f, 0.25f, 1f));
        tableCellStyle.normal.textColor = Color.white;
        tableCellStyle.padding = new RectOffset(5, 5, 5, 5);
        tableCellStyle.alignment = TextAnchor.MiddleLeft;

        // Input field style
        inputStyle = new GUIStyle(GUI.skin.textField);
        inputStyle.normal.background = MakeBackgroundTexture(2, 2, new Color(0.3f, 0.3f, 0.3f, 1f));
        inputStyle.normal.textColor = Color.white;
        inputStyle.padding = new RectOffset(5, 5, 5, 5);
        inputStyle.margin = new RectOffset(2, 2, 2, 2);

        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.normal.background = MakeBackgroundTexture(2, 2, new Color(0.4f, 0.4f, 0.6f, 1f));
        buttonStyle.hover.background = MakeBackgroundTexture(2, 2, new Color(0.5f, 0.5f, 0.7f, 1f));
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.hover.textColor = Color.white;
        buttonStyle.padding = new RectOffset(10, 10, 5, 5);
        buttonStyle.margin = new RectOffset(5, 5, 5, 5);

        // Section style (for grouping controls)
        sectionStyle = new GUIStyle(GUI.skin.box);
        sectionStyle.normal.background = MakeBackgroundTexture(2, 2, new Color(0.25f, 0.25f, 0.25f, 0.95f));
        sectionStyle.padding = new RectOffset(10, 10, 10, 10);
        sectionStyle.margin = new RectOffset(0, 0, 10, 10);

        // Label style
        labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 12;
        labelStyle.margin = new RectOffset(5, 5, 5, 5);
    }

    protected Texture2D MakeBackgroundTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    protected async void ProcessAsyncOperation(Task operation)
    {
        if (isOperationInProgress) return;

        isOperationInProgress = true;
        operationStatus = "Operation in progress...";

        try
        {
            await operation;
            operationStatus = "Operation completed successfully!";
            await RefreshData();
        }
        catch (Exception e)
        {
            operationStatus = $"Error: {e.Message}";
            Debug.LogError(e);
        }
        finally
        {
            isOperationInProgress = false;
            statusDisplayTime = Time.time + STATUS_DISPLAY_DURATION;
        }
    }

    protected void DrawDataTable()
    {
        if (currentData == null || currentData.Rows.Count == 0)
        {
            GUILayout.Label("No data available", tableCellStyle);
            return;
        }

        // Calculate total width needed for the table
        float totalWidth = 0;
        foreach (DataColumn column in currentData.Columns)
        {
            totalWidth += 120; // Width per column
        }

        // Begin a horizontal scope for the entire table
        GUILayout.BeginHorizontal(GUILayout.Width(totalWidth));

        // Create a vertical layout for the table contents
        GUILayout.BeginVertical();

        // Draw headers
        GUILayout.BeginHorizontal();
        foreach (DataColumn column in currentData.Columns)
        {
            GUILayout.Label(column.ColumnName, tableHeaderStyle, GUILayout.Width(120));
        }
        GUILayout.EndHorizontal();

        // Draw rows
        foreach (DataRow row in currentData.Rows)
        {
            GUILayout.BeginHorizontal();
            foreach (var item in row.ItemArray)
            {
                GUILayout.Label(item.ToString(), tableCellStyle, GUILayout.Width(120));
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    // Helper methods for common input fields
    protected string DrawInputField(string label, string value, float width = 200)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, labelStyle, GUILayout.Width(150));
        string result = GUILayout.TextField(value, inputStyle, GUILayout.Width(width));
        GUILayout.EndHorizontal();
        return result;
    }

    protected float DrawFloatField(string label, float value, float width = 100)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, labelStyle, GUILayout.Width(150));
        float.TryParse(GUILayout.TextField(value.ToString(), inputStyle, GUILayout.Width(width)), out float result);
        GUILayout.EndHorizontal();
        return result;
    }

    protected int DrawIntField(string label, int value, float width = 100)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, labelStyle, GUILayout.Width(150));
        int.TryParse(GUILayout.TextField(value.ToString(), inputStyle, GUILayout.Width(width)), out int result);
        GUILayout.EndHorizontal();
        return result;
    }

    protected decimal DrawDecimalField(string label, decimal value, float width = 100)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, labelStyle, GUILayout.Width(150));
        decimal.TryParse(GUILayout.TextField(value.ToString(), inputStyle, GUILayout.Width(width)), out decimal result);
        GUILayout.EndHorizontal();
        return result;
    }

    // Abstract method that derived classes must implement
    protected abstract Task RefreshData();
}