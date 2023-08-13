using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(menuName = "Simple Data Tables/Settings")]
public class SimpleDataTableSettings : ScriptableObject
{
    [SerializeField] public List<Color> DefaultColours;
    [SerializeField] public List<int> DefaultSize;
    public List<string> DataTypes = new List<string>(){
        "Key",
        "int",
        "string",
        "float",
        "bool",
        "double",
        "long",
        "Vector2",
        "Vector2Int",
        "Vector3",
        "Vector3Int",
        "GameObject",
        "Color",
        "MonoBehaviour",
        "Transform",
        "Texture2D",
        "AnimationCurve",
        "Rect",
        "Bounds",
        "BoundsInt",
        "Custom"
    };
    public int DefaultDataType = 0;
    public bool AutomaticlyApplyColours = true;
    public bool AutomaticlyApplyWidths = true;

    [SerializeField] public Color CellColor = new Color(0.15f, 0.15f, 0.15f, 1);
    [SerializeField] public Color AltCellColor = new Color(0.1f, 0.1f, 0.1f, 1);
    [SerializeField] public Color SelectedRowColor = new Color(0.2f, 0.592f, 1f, 1);

    public string[] GetUserDataType ()
    {
        List<string> temp = new List<string>(DataTypes);
        temp.RemoveAt(0);
        return temp.ToArray(); 
    }
    public int GetDataTypeIndex(string Type)
    {
        return DataTypes.IndexOf(Type);
    }
}