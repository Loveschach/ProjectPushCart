using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using CustomEditorTools;
using System;

[CustomEditor(typeof(DataTable),true)]
public class DataTableEditor : Editor
{
    private BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private DataTable Selected;
    private int SelectedRow = -1;
    private Vector2 ScrollPos;
    private List<FieldInfo> RowAttributes = new List<FieldInfo>();

    private GUIStyle OddRow_Style = new GUIStyle();
    private GUIStyle EvenRow_Style = new GUIStyle();
    private GUIStyle SelectedRow_Style = new GUIStyle();

    private GUIStyle SelectRowBtn_Style = new GUIStyle();
    private GUIStyle RowEditBtn_Style = new GUIStyle();
    private GUIStyle ColumnHeader_Style = new GUIStyle();

    SimpleDataTableSettings Settings;

    private void OnEnable()
    {
        SelectedRow = -1;
        Selected = (DataTable)target;
        Settings = (SimpleDataTableSettings)Resources.Load("SimpleDataTableSettings");

        FieldInfo[] RowAttributesTemp = Selected.GetVariableList(BindingFlags);
        RowAttributes.Add(RowAttributesTemp[RowAttributesTemp.Length - 1]);

        //Prevent Out oif bounds errors when changing the number of columns in a row
        if (RowAttributesTemp.Length != Selected.NewColumnColors.Length)
        {
            DataTable TempInstance = (DataTable)CreateInstance(Selected.GetType().Name);
            Selected.NewColumnColors = TempInstance.NewColumnColors;
            Selected.NewColumnWidths = TempInstance.NewColumnWidths;
            DestroyImmediate(TempInstance);
        }
        
        for (int i = 0; i < RowAttributesTemp.Length-1; i++)
        {
            RowAttributes.Add(RowAttributesTemp[i]);
        }
        #region Style Setup
        OddRow_Style.normal.background = CET.MakeEditorBackgroundColor(Settings.CellColor);
        EvenRow_Style.normal.background = CET.MakeEditorBackgroundColor(Settings.AltCellColor);
        SelectedRow_Style.normal.background = CET.MakeEditorBackgroundColor(Settings.SelectedRowColor);


        RowEditBtn_Style.padding = new RectOffset();
        RowEditBtn_Style.fixedHeight = 20;
        RowEditBtn_Style.fixedWidth = 20;
        RowEditBtn_Style.margin = new RectOffset(0, 5, 0, 0);
        #endregion
    }

    public override void OnInspectorGUI()
    {
        // Hack color fix
        Color CellColor = new Color(0.15f, 0.15f, 0.15f, 1);
        Color AltCellColor = new Color(0.1f, 0.1f, 0.1f, 1);

        serializedObject.Update();
        OddRow_Style.normal.background = CET.MakeEditorBackgroundColor(CellColor);
        EvenRow_Style.normal.background = CET.MakeEditorBackgroundColor(AltCellColor);
        SelectedRow_Style.normal.background = CET.MakeEditorBackgroundColor(Settings.SelectedRowColor);
        SelectRowBtn_Style = new GUIStyle("button");
        int CurrentWidth;

        #region Draw Table View
        ScrollPos = GUILayout.BeginScrollView(ScrollPos);
        #region Draw Column Names
        GUILayout.BeginHorizontal();
        GUILayout.Space(30 + (SelectRowBtn_Style.margin.right * 2));

        for (int i = 0; i < RowAttributes.Count; i++)
        {
            CurrentWidth = Selected.NewColumnWidths[i];
            ColumnHeader_Style.normal.background = CET.MakeEditorBackgroundColor(Selected.NewColumnColors[i]);

            GUILayout.BeginHorizontal(ColumnHeader_Style, GUILayout.Width(CurrentWidth));
            GUILayout.Label(RowAttributes[i].Name, EditorStyles.boldLabel, GUILayout.Width(CurrentWidth));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndHorizontal();
        #endregion
        
        #region Draw Rows
        //Loop Through all rows
        for (int row = 0; row < Selected.Rows.Count; row++)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginHorizontal(GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight - 2));
                if (GUILayout.Button(""+row, SelectRowBtn_Style, GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight-2)))
                {
                    SelectedRow = row;
                }
                GUILayout.EndHorizontal();

                for (int i = 0; i < RowAttributes.Count; i++)
                {
                    CurrentWidth = Selected.NewColumnWidths[i];

                    //= CET.MakeEditorBackgroundColor(AltCellColor);

                    if (SelectedRow == row)
                    {
                        GUILayout.BeginHorizontal(SelectedRow_Style, GUILayout.Width(CurrentWidth));
                    }
                    else
                    {
                        //ColumnHeader_Style.normal.background = CET.MakeEditorBackgroundColor(Selected.NewColumnColors[i]);

                        GUILayout.BeginHorizontal((row % 2 == 0) ? EvenRow_Style : OddRow_Style, GUILayout.Width(CurrentWidth));
                    }
                    GUILayout.Label(TypeToStringValue(serializedObject.FindProperty("Rows").GetArrayElementAtIndex(row).FindPropertyRelative(RowAttributes[i].Name)), EditorStyles.boldLabel, GUILayout.Width(CurrentWidth));
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(5);

        #endregion
        GUILayout.EndScrollView();
        #endregion

        #region Rearrange, Remove and Add Buttons

        CET.HorizontalLine();
        if (GUILayout.Button("Add"))
        {
            Selected.Add("Key_" + Selected.Rows.Count);
        }
        CET.HorizontalLine();
        #endregion

        #region Selected Row PropertyField
        if (Selected.Rows.Count > 0 && SelectedRow != -1) //If there is at least one row
        {
            EditorGUILayout.BeginHorizontal();
            if (SelectedRow != 0 && GUILayout.Button(Resources.Load<Texture2D>("Up"), RowEditBtn_Style))
            {
                SelectedRow = Selected.Nudge(-1, SelectedRow);
            }
            if (SelectedRow < Selected.Rows.Count-1 && GUILayout.Button(Resources.Load<Texture2D>("Down"), RowEditBtn_Style))
            {
                SelectedRow = Selected.Nudge(1, SelectedRow);
            }
            if (GUILayout.Button(Resources.Load<Texture2D>("Remove"), RowEditBtn_Style))
            {
                if (Selected.Rows.Count > 0)
                {
                    Selected.Remove(SelectedRow);
                }
                SelectedRow = 0;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            for (int j = 0; j < RowAttributes.Count; j++) //All row attributes
            {
                if (j == 1) { EditorGUILayout.Space(); }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Rows").GetArrayElementAtIndex(SelectedRow).FindPropertyRelative(RowAttributes[j].Name));
            }
        }
        else
        {
            SelectedRow = -1;
            GUILayout.Label("No Row Selected");
        }
        CET.HorizontalLine();
        #endregion

        #region Edit Styles
        Rect StyleArea = new Rect((EditorGUIUtility.currentViewWidth / 2)-150 ,30, 0,0);
        if (GUILayout.Button("Open Instance Style Option"))
        {
            PopupWindow.Show(StyleArea, new StyleOverridePopup(serializedObject, RowAttributes, Selected));
        }
        #endregion

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(Selected);
    }

    private string TypeToStringValue(SerializedProperty sp)
    {
        switch (sp.type)
        {
            case "bool": return sp.boolValue.ToString();
            case "int": return sp.intValue.ToString();
            case "string": return sp.stringValue;
            case "float": return sp.floatValue.ToString();
            case "double": return sp.doubleValue.ToString();
            case "long": return sp.longValue.ToString();
            case "Vector2": return sp.vector2Value.ToString();
            case "Vector2Int": return sp.vector2IntValue.ToString();
            case "Vector3": return sp.vector3Value.ToString();
            case "Vector3Int": return sp.vector3IntValue.ToString();
            case "Color": return sp.colorValue.ToString();
            case "Rect": return sp.rectValue.ToString();
            case "Bounds": return sp.boundsValue.ToString();
            case "BoundsInt": return sp.boundsIntValue.ToString();
            case "AnimationCurve": return sp.animationCurveValue.ToString();
            case "Enum": return sp.enumNames[sp.enumValueFlag].ToString();
            //case "PPtr<$Texture2D>": return sp.objectReferenceValue.ToString();
        }
        return (sp.type);
    }
    public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
    {
        try
        {
            Texture2D IconLoad = Resources.Load<Texture2D>("TableIcon");
            Texture2D FinalIconImage = new Texture2D(width, height);
            EditorUtility.CopySerialized(AssetPreview.GetAssetPreview(IconLoad), FinalIconImage);
            return FinalIconImage;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return null;
    }
}