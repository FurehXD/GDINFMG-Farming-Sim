using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseButton), true)] // 'true' allows this editor to apply to derived classes
public class BaseButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector for all serialized fields, including inherited ones
        DrawDefaultInspector();

        // Cast the target to BaseButton
        BaseButton baseButton = (BaseButton)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Button Properties", EditorStyles.boldLabel);

        // Draw the custom BaseButton fields
        baseButton.HasUniqueSize = EditorGUILayout.Toggle("Has Unique Size", baseButton.HasUniqueSize);
        baseButton.OverrideDefault = EditorGUILayout.Toggle("Override Default", baseButton.OverrideDefault);
        baseButton.HoverSFX = (AudioSource)EditorGUILayout.ObjectField("Hover SFX", baseButton.HoverSFX, typeof(AudioSource), true);
        baseButton.ClickSFX = (AudioSource)EditorGUILayout.ObjectField("Click SFX", baseButton.ClickSFX, typeof(AudioSource), true);
    }
}
