using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseButton))]
public class BaseButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default Button Inspector
        DrawDefaultInspector();

        // Draw your custom fields below
        BaseButton baseButton = (BaseButton)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Button Properties", EditorStyles.boldLabel);

        baseButton.HasUniqueSize = EditorGUILayout.Toggle("Has Unique Size", baseButton.HasUniqueSize);
        baseButton.OverrideDefault = EditorGUILayout.Toggle("Override Default", baseButton.OverrideDefault);
        baseButton.HoverSFX = (AudioSource)EditorGUILayout.ObjectField("Hover SFX", baseButton.HoverSFX, typeof(AudioSource), true);
        baseButton.ClickSFX = (AudioSource)EditorGUILayout.ObjectField("Click SFX", baseButton.ClickSFX, typeof(AudioSource), true);
    }
}
