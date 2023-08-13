using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

[System.Serializable]
public abstract class DataTable : ScriptableObject
{
    [field: NonSerialized] public virtual Color[]  NewColumnColors { get; set; }
    [field: NonSerialized] public virtual int[]    NewColumnWidths { get; set; }

    [SerializeReference] public List<DataTableRow> Rows = new List<DataTableRow>();

    public int Nudge(int Direction, int Index)
    {
        DataTableRow temp = Rows[Index];
        Rows[Index] = Rows[Index + Direction];
        Rows[Index + Direction] = temp;
        return Index + Direction;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Key"></param>
    /// <returns></returns>
    private int FindRowIndex(string Key)
    {
        return Rows.FindIndex(x => x.Key.Equals(Key));
    }
    /// <summary>
    /// Adds a new row to the bottom of a data table with default values.
    /// </summary>
    public virtual void Add(string Key){}
    /// <summary>
    /// Removes a row from a table at a given index.
    /// </summary>
    public void Remove(int Index)
    {
        Rows.RemoveAt(Index);
    }
    /// <summary>
    /// Removes a row with a given key from the table. If ‘MultipleRows’ is true all rows with the key will be removed. If ‘MultipleRows’ is false only the first match will be removed.
    /// </summary>
    public void Remove(string Key, bool MultipleRows)
    {
        if(MultipleRows)
        {
            Rows.RemoveAll(x => x.Key.Equals(Key));
        }
        else
        {
            Remove(FindRowIndex(Key));
        }
    }
    /// <summary>
    /// Returns the number of rows in a data table.
    /// </summary>
    public int Size() => Rows.Count;
    /// <summary>
    /// Returns a list of all variables in a data table. Used for editor code
    /// </summary>
    public virtual FieldInfo[] GetVariableList(BindingFlags bindingFlags)
    {
        return typeof(DataTable).GetFields(bindingFlags);
    }
    /// <summary>
    /// Returns a row from a table at a specific index.
    /// </summary>
    public T GetRow<T>(int Index) where T : DataTableRow
    {
        return (T)Rows[Index];
    }
    /// <summary>
    /// Returns the first row in the table which has a specific key.
    /// </summary>
    public T GetRow<T>(string Key) where T : DataTableRow
    {
        return (T)Rows[FindRowIndex(Key)];
    }
    /// <summary>
    /// Returns a random row from a data table.
    /// </summary>
    public T GetRandomRow<T>() where T : DataTableRow
    {
        return (T)Rows[UnityEngine.Random.Range(0, Rows.Count)];
    }
    /// <summary>
    /// Returns an array of all rows.
    /// </summary>
    public T[] GetAllRows<T>() where T : DataTableRow
    {
        DataTableRow[] BaseArray = Rows.ToArray();
        T[] CastArray = Array.ConvertAll(BaseArray, x => (T)x);
        return (T[])CastArray;
    }
    /// <summary>
    /// Returns an array of all rows with a specific key.
    /// </summary>
    public T[] GetAllRowsWithKey<T>(string Key) where T : DataTableRow
    {
        var KeyRows = from DataTableRow i in Rows
                      where i.Key.Equals(Key)
                      select i;
        DataTableRow[] A = new List<DataTableRow>(KeyRows).ToArray();

        return Array.ConvertAll(A, x => (T)x);
    }
    /// <summary>
    /// Returns true if there is at least one row with the passed in key.
    /// </summary>
    public bool HasKey (string Key)
    {
        return Rows.Any(i => i.Key.Equals(Key));

       /*
        Rows.Find(i => i.Key)
        for (int i = 0; i < Rows.Count; i++)
        {
            if (Rows[i].Key.Equals(Key))
            {
                return true;
            }
        }
        return false;
        */
    }
    /// <summary>
    /// Returns a list of all keys present in the table with duplicates included.
    /// </summary>
    public string[] GetAllKeys ()
    {
        string[] Keys = new string[Rows.Count];
        for (int i = 0; i < Rows.Count; i++)
        {
            Keys[i] = Rows[i].Key;
        }
        return Keys;
    }
    /// <summary>
    /// Returns a list of all keys present in the table with duplicates removed.
    /// </summary>
    public string[] GetAllKeysUnique()
    {
        string[] Temp = GetAllKeys();
        return Temp.Distinct().ToArray();
    }
}


[System.Serializable]
public class DataTableRow
{
    [SerializeReference] public string Key;
}