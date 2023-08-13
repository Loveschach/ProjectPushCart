using UnityEngine;
using UnityEditor;

public class SimpleDataTableSettingsWindow : EditorWindow
{
    SimpleDataTableSettings Settings;

    [MenuItem("Simple Data Tables/Settings")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SimpleDataTableSettingsWindow));
    }

    private void OnGUI()
    {
        Settings = (SimpleDataTableSettings)Resources.Load("SimpleDataTableSettings");
        EditorUtility.SetDirty(Settings);
        GUILayout.Label("Template Editor Settings");
        for (int i = 0; i < Settings.DataTypes.Count; i++)
        {
            if(Settings.DefaultColours.Count <= i)
            {
                Settings.DefaultColours.Add(new Color());
                Settings.DefaultSize.Add(50);
            }
            GUILayout.BeginHorizontal();
                GUILayout.Label(Settings.DataTypes[i], GUILayout.Width(100));
                Settings.DefaultColours[i] =    EditorGUILayout.ColorField(Settings.DefaultColours[i]);
                Settings.DefaultSize[i] =       EditorGUILayout.IntField(Settings.DefaultSize[i]);
            GUILayout.EndHorizontal();
            if (Settings.DataTypes[i].Equals("Key"))
            {
                GUILayout.Space(20);
            }
        }
        GUILayout.Space(20);
        Settings.DefaultDataType = EditorGUILayout.Popup("Default Data Type", Settings.DefaultDataType, Settings.GetUserDataType());
        EditorGUIUtility.labelWidth = 300;
        Settings.AutomaticlyApplyColours = EditorGUILayout.Toggle("Automaticly Apply Color Data in Template Editor", Settings.AutomaticlyApplyColours);
        Settings.AutomaticlyApplyWidths = EditorGUILayout.Toggle("Automaticly Apply Width Data in Template Editor", Settings.AutomaticlyApplyWidths);
        GUILayout.Space(20);

        GUILayout.Label("Instance Editor Settings");
        
        Settings.CellColor = EditorGUILayout.ColorField("Cell Color", Settings.CellColor);
        Settings.AltCellColor = EditorGUILayout.ColorField("Alt Cell Color", Settings.AltCellColor);
        Settings.SelectedRowColor = EditorGUILayout.ColorField("Selected Row Color", Settings.SelectedRowColor);
    }
}

[CustomEditor(typeof(SimpleDataTableSettings))]
public class SimpleDataTableSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("Settings can be edited from \"Simple Data Tables/Settings\"");
        if(GUILayout.Button("Open Window"))
        {
            SimpleDataTableSettingsWindow inst = EditorWindow.GetWindow<SimpleDataTableSettingsWindow>();
            inst.Show();
        }
        //base.OnInspectorGUI();
    }
}