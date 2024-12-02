using UnityEngine;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class BaseAdminPanel : MonoBehaviour
{
    protected Vector2 scrollPosition;
    protected string operationStatus;
    protected bool isOperationInProgress;
    protected DataTable currentData;

    protected GUIStyle backgroundStyle;
    protected GUIStyle labelStyle;
    protected GUIStyle inputStyle;
    protected GUIStyle buttonStyle;

    protected virtual void InitializeStyles()
    {
        if (GUI.skin == null) return;

        backgroundStyle = new GUIStyle(GUI.skin.box);
        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            padding = new RectOffset(5, 5, 5, 5)
        };

        inputStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = 12,
            padding = new RectOffset(5, 5, 5, 5)
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 12,
            padding = new RectOffset(10, 10, 5, 5)
        };
    }

    protected string DrawInputField(string label, string value, float width = 200f)
    {
        if (labelStyle == null || inputStyle == null)
        {
            InitializeStyles();
            return value;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label(label, labelStyle, GUILayout.Width(150));
        string result = GUILayout.TextField(value, inputStyle, GUILayout.Width(width));
        GUILayout.EndHorizontal();
        return result;
    }

    protected float DrawFloatField(string label, float value, float width = 200f)
    {
        string stringValue = DrawInputField(label, value.ToString(), width);
        float result;
        return float.TryParse(stringValue, out result) ? result : value;
    }

    protected int DrawIntField(string label, int value, float width = 200f)
    {
        string stringValue = DrawInputField(label, value.ToString(), width);
        int result;
        return int.TryParse(stringValue, out result) ? result : value;
    }

    protected virtual void Awake()
    {
        InitializeStyles();
    }

    protected virtual void OnEnable()
    {
        ProcessAsyncOperation(RefreshData());
    }

    protected async void ProcessAsyncOperation(Task operation)
    {
        if (operation == null) return;

        isOperationInProgress = true;
        operationStatus = "Processing...";

        try
        {
            await operation;
            operationStatus = "Operation completed successfully";
        }
        catch (Exception e)
        {
            operationStatus = $"Error: {e.Message}";
            Debug.LogError($"Operation failed: {e}");
        }
        finally
        {
            isOperationInProgress = false;
        }
    }

    protected virtual Task RefreshData()
    {
        return Task.CompletedTask;
    }
}