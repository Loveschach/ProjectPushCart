using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using CustomEditorTools;
using System;

public class StyleOverridePopup : PopupWindowContent
{
    SerializedObject SerializedObjectRef;
    List<FieldInfo> FieldNames;
    GUIStyle but = new GUIStyle();
    Vector2 ScrollPos;
    SimpleDataTableSettings Settings;
    DataTable CurrentTable;

    public override Vector2 GetWindowSize()
    {
        return new Vector2(300, 500);
    }

    public StyleOverridePopup(SerializedObject NewSerializedObjectRef, List<FieldInfo> NewFieldNames, DataTable DT)
    {
        SerializedObjectRef = NewSerializedObjectRef;
        CurrentTable = DT;
        FieldNames = NewFieldNames;
        but.padding = new RectOffset();
        but.fixedHeight = 20;
        but.fixedWidth = 20;
        but.margin = new RectOffset(10, 10, 10, 10);
    }

    public override void OnGUI(Rect rect)
    {
        SerializedObjectRef.Update();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button((Texture2D)Resources.Load("Remove"), but))
        {
            editorWindow.Close();
        }
        GUILayout.EndHorizontal();


        GUILayout.Label("Table Style Overrides", EditorStyles.boldLabel);
        GUILayout.Label("These settings are only applied to this instance");

        ScrollPos = GUILayout.BeginScrollView(ScrollPos);
        
        GUILayout.Space(20);
        GUILayout.Label("Column Colours", EditorStyles.boldLabel);
        for(int i = 0; i < FieldNames.Count; i++)
        {
            if (i == 0) { GUILayout.Space(5); }
            CurrentTable.NewColumnColors[i] = EditorGUILayout.ColorField(FieldNames[i].Name, CurrentTable.NewColumnColors[i]);
        }

        GUILayout.Space(20);
        GUILayout.Label("Column Widths", EditorStyles.boldLabel);
        for (int i = 0; i < FieldNames.Count; i++)
        {
            if (i == 0) { GUILayout.Space(5); }
            CurrentTable.NewColumnWidths[i] = EditorGUILayout.IntField(FieldNames[i].Name, CurrentTable.NewColumnWidths[i]);

            if(CurrentTable.NewColumnWidths[i] < 0)
            {
                CurrentTable.NewColumnWidths[i] = 0;
            }
        }

        GUILayout.EndScrollView();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Reset Colors"))
        {
            bool Accept = EditorUtility.DisplayDialog("Are you sure?", "Do you want to reset all colors to their default values?", "Reset", "Cancel");
            if (Accept)
            {
                DataTable TempInstance = (DataTable)ScriptableObject.CreateInstance(CurrentTable.GetType().Name);
                CurrentTable.NewColumnColors = TempInstance.NewColumnColors;
                ScriptableObject.DestroyImmediate(TempInstance);
            }
        }
        if (GUILayout.Button("Reset Widths"))
        {
            bool Accept = EditorUtility.DisplayDialog("Are you sure?", "Do you want to reset all widths to their default values?", "Reset", "Cancel");
            if (Accept)
            {
                DataTable TempInstance = (DataTable)ScriptableObject.CreateInstance(CurrentTable.GetType().Name);
                CurrentTable.NewColumnWidths = TempInstance.NewColumnWidths;
                ScriptableObject.DestroyImmediate(TempInstance);
            }
        }

        GUILayout.EndHorizontal();
        SerializedObjectRef.ApplyModifiedProperties();
        EditorUtility.SetDirty(CurrentTable);
    }

    public override void OnOpen()
    {
        
    }

    public override void OnClose()
    {
        //MainEditor.CloseStyleOverride();
    }
}
