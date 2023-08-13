
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;
using UnityEditor;
using Path = System.IO.Path;
using CustomEditorTools;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Simple Data Tables/New Table Template")]
public class DataTableTemplate : ScriptableObject
{
    public List<Attribute> columns = new List<Attribute>(0); 
    public string columnsSetName = "";                       //Default attribute Name
    private void OnEnable()
    {
        if(!columns.Exists(i => i.Name.Equals("Key")))
        {
            SimpleDataTableSettings Settings = (SimpleDataTableSettings)Resources.Load("SimpleDataTableSettings");
            columns.Add(new Attribute("Key", 0, "Key", Settings.DefaultColours[0], Settings.DefaultSize[0]));
        }  
    }
}

[CustomEditor(typeof(DataTableTemplate))]
public class CreateTableTypeEditor : Editor
{
    DataTableTemplate MyTarget;
    GUIStyle Attribute_Style = new GUIStyle();
    SimpleDataTableSettings Settings;
   
    private void OnEnable()
    {
        MyTarget = (DataTableTemplate)target;
        Attribute_Style.normal.background = CET.MakeEditorBackgroundColor(new Color(0.1f, 0.1f, 0.1f));
        Attribute_Style.margin = new RectOffset(0, 0, 5, 0);
        Attribute_Style.padding = new RectOffset(0, 0, 5, 5);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUIUtility.labelWidth = 70;
        Settings = (SimpleDataTableSettings)Resources.Load("SimpleDataTableSettings");
        GUIStyle but = new GUIStyle();
        but.padding = new RectOffset();
        but.fixedHeight = 20;
        but.fixedWidth = 20;
        but.margin = new RectOffset(0, 5, 0, 0);
        
        for (int i = 0; i < MyTarget.columns.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(Attribute_Style);

            if (i == 0) { GUI.enabled = false; }
            MyTarget.columns[i].Name = EditorGUILayout.TextField("Name", MyTarget.columns[i].Name);

            string[] ggg = i != 0 ? Settings.GetUserDataType() : Settings.DataTypes.ToArray();
            MyTarget.columns[i].selected = EditorGUILayout.Popup("Date Type", MyTarget.columns[i].selected, ggg);
            MyTarget.columns[i].DataType = ggg[MyTarget.columns[i].selected];
            GUI.enabled = true;

            int ID = Settings.GetDataTypeIndex(MyTarget.columns[i].DataType);
         
            if (MyTarget.columns[i].DataType.Equals("Custom"))
            {
                MyTarget.columns[i].CustomDataType = EditorGUILayout.TextField("Custom Class", MyTarget.columns[i].CustomDataType);
            }

            #region Colors
            if (Settings.AutomaticlyApplyColours)
            {
                MyTarget.columns[i].ColumnColor = Settings.DefaultColours[ID];
            }
            GUI.enabled = !Settings.AutomaticlyApplyColours;
            MyTarget.columns[i].ColumnColor = EditorGUILayout.ColorField("Color", MyTarget.columns[i].ColumnColor);
            GUI.enabled = true;
            #endregion

            #region Widths
            if (Settings.AutomaticlyApplyWidths)
            {
                MyTarget.columns[i].ColumnWidth = Settings.DefaultSize[ID];
            }
            GUI.enabled = !Settings.AutomaticlyApplyWidths;
            MyTarget.columns[i].ColumnWidth = EditorGUILayout.IntField("Width", MyTarget.columns[i].ColumnWidth);
            GUI.enabled = true;
            #endregion

            if (MyTarget.columns[i].ColumnWidth < 0) { MyTarget.columns[ID].ColumnWidth = 0; }

            GUILayout.BeginHorizontal(GUILayout.Width(70));
            if(i > 1)
            {
                if (GUILayout.Button(Resources.Load<Texture2D>("Up"), but)) //Move Up
                {
                    move(-1, i);
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
            }
        
            if (i != MyTarget.columns.Count -1 && i != 0 && GUILayout.Button(Resources.Load<Texture2D>("Down"), but))  //Move Down
            {
                move(1, i);
            }
            if(i == MyTarget.columns.Count - 1)
            {
                GUILayout.FlexibleSpace();
            }
            if (i != 0 && GUILayout.Button(Resources.Load<Texture2D>("Remove"), but)) //Remove a Row
            {
                MyTarget.columns.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            if (i == 0) { GUILayout.Space(30); }
        }
        GUILayout.Space(15);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Column"))
        {
            MyTarget.columns.Add(new Attribute(
                Settings.DataTypes[Settings.DefaultDataType], 
                Settings.DefaultDataType, "Column" + MyTarget.columns.Count,
                Settings.DefaultColours[Settings.DefaultDataType + 1], 
                Settings.DefaultSize[Settings.DefaultDataType + 1]));
        }
       
        if (GUILayout.Button("Reset Width Style"))
        {
            if (EditorUtility.DisplayDialogComplex("Reset Width Style", "Are you sure you want to reset widths to their default styles", "Reset Styles Now", "Cancel", "") == 0)
            {
                for (int i = 0; i < MyTarget.columns.Count; i++)
                {
                    MyTarget.columns[i].ColumnWidth = Settings.DefaultSize[MyTarget.columns[i].selected];
                }
            }
        }
        if (GUILayout.Button("Reset Color Style"))
        {
            if(EditorUtility.DisplayDialogComplex("Reset Color Style", "Are you sure you want to reset colors to their default styles", "Reset Styles Now", "Cancel", "") == 0)
            {
                for (int i = 0; i < MyTarget.columns.Count; i++)
                {
                    MyTarget.columns[i].ColumnColor = Settings.DefaultColours[MyTarget.columns[i].selected];
                }
            }
        }
        GUILayout.EndHorizontal();
        CET.HorizontalLine();
        GUILayout.Space(15);
        GUILayout.BeginHorizontal();
        GUILayout.Label("New Table Name: ", GUILayout.Width(100));
        MyTarget.columnsSetName = EditorGUILayout.TextField(MyTarget.columnsSetName);
        GUILayout.EndHorizontal();
        GUILayout.Space(15);

        EditorUtility.SetDirty(MyTarget);
        serializedObject.ApplyModifiedProperties();
        #region Generate Table
        if (GUILayout.Button("Generate Table"))
        {
            string valid = isValid();
            if(!valid.Equals("Valid"))
            {
                EditorUtility.DisplayDialog("Error Invalid Name", valid, "Ok");
            }
            else
            {
                string NewPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MyTarget.GetInstanceID())) + "/DataTable_" + MyTarget.columnsSetName + ".cs";

                if (!File.Exists(NewPath)){
                    GenerateNewDataTable(NewPath);
                }
                else //Found a file with the same name
                {
                    int option = EditorUtility.DisplayDialogComplex("Are you sure you want to create this file?", "Another file was found with the same Name. This will override / update this?", "Create", "Don't Create", "");
                    if(option == 0)
                    {
                        GenerateNewDataTable(NewPath);
                    }
                }
            }
            string AssetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MyTarget));
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(MyTarget), MyTarget.columnsSetName + "(Template)");
            AssetDatabase.Refresh();
        }

        #endregion
    }
    public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
    {
        try
        {
            Texture2D IconLoad = Resources.Load<Texture2D>("TableIconSettings");
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
    public void GenerateNewDataTable (string Path)
    {
        using (StreamWriter script = new StreamWriter(Path))
        {   
            TextAsset BoilerPlate = (TextAsset)Resources.Load("Boilerplate");
            string BoilerPlateText = BoilerPlate.text;
            BoilerPlateText = BoilerPlateText.Replace("BoilerPlateNameOfTable", MyTarget.columnsSetName);

            #region Generate Colors
            string NewColors = "";
            Color CurrentColor;
            for (int i = 0; i < MyTarget.columns.Count; i++)
            {
                CurrentColor = MyTarget.columns[i].ColumnColor;
                NewColors += "new Color(" + CurrentColor.r + "f, " + CurrentColor.g + "f, " + CurrentColor.b + "f)";
                if (i != MyTarget.columns.Count - 1) { NewColors += ", "; }
            }
            BoilerPlateText = BoilerPlateText.Replace("BoilerPlateColors", NewColors);
            #endregion

            #region Generate Widths
            string NewWidths = "";
            for (int i = 0; i < MyTarget.columns.Count; i++)
            {
                NewWidths += MyTarget.columns[i].ColumnWidth;
                if (i != MyTarget.columns.Count - 1) { NewWidths += ", "; }
            }
            BoilerPlateText = BoilerPlateText.Replace("BoilerPlateWidths", NewWidths);
            #endregion
            
            #region Generate Row Attributes
            string NewRowData = "";
            for (int i = 1; i < MyTarget.columns.Count; i++)
            {
                string DataType = (MyTarget.columns[i].DataType.Equals("Custom")) ? MyTarget.columns[i].CustomDataType : MyTarget.columns[i].DataType;
                NewRowData += "\t[SerializeField] public " + DataType + " " + MyTarget.columns[i].Name.Trim() + ";" + Environment.NewLine;
            }
            BoilerPlateText = BoilerPlateText.Replace("BoilerPlateRowAttributes", NewRowData);
            #endregion

            script.Write(BoilerPlateText);
            
            EditorUtility.DisplayDialog("Success", "A new data table was created", "Ok");
        }
    }

    //Ensures that an attribute Name is valid
    public string isValid ()
    {
        foreach(Attribute i in MyTarget.columns)
        {
            if(string.IsNullOrEmpty(i.Name))
            {
                return "Attribute Names cannot be null or empty.";
            }
            if(i.Name.Contains("#"))
            {
                return "Attribute Names cannot contain '#'.";
            }
            if (i.Name.Contains(" "))
            {
                return "Attribute Names cannot contain spaces.";
            }
            if (char.IsDigit(i.Name[0]))
            {
                return "Attribute Names cannot begin with a number.";
            }
        }
        return "Valid";
    }

    //Used to rearrange the attributes 
    public void move(int direction, int index)
    {
        try
        {
            Attribute temp = MyTarget.columns[index];
            MyTarget.columns[index] = MyTarget.columns[index + direction];
            MyTarget.columns[index + direction] = temp;
        }
        catch (Exception e) { e.ToString(); }
    }

}

//Stores the infromation about a single column in table
[System.Serializable]
public class Attribute
{
    [SerializeField] public string DataType;                        //The column type
    [SerializeField] public string CustomDataType;                  //Used if DataType is set to AvailableDataTypes.Custom
    [SerializeField] public string Name;                            //The column Name
    [SerializeField] public Color ColumnColor = new Color(1,1,1);   //The column colour
    [SerializeField] public int ColumnWidth = 150;
    public int selected = 0;

    //Column Constructor
    public Attribute(string NewDataTypeText, int NewDataTypeIndex, string NewName, Color NewColor, int NewWidth)      
    {
        DataType = NewDataTypeText;
        selected = NewDataTypeIndex;
        Name = NewName;
        ColumnColor = NewColor;
        ColumnWidth = NewWidth;
    }
}