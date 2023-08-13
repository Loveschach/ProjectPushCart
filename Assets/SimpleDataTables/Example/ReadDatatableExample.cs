using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadDatatableExample : MonoBehaviour
{
    public DataTable ExampleTable;

    void Start()
    {
        //Find a single row using its index
        DataTableRow_ExamplePersonTable GetByIndex = ExampleTable.GetRow<DataTableRow_ExamplePersonTable>(0);
        //Find a single row using its key
        DataTableRow_ExamplePersonTable GetByKey = ExampleTable.GetRow<DataTableRow_ExamplePersonTable>("Jim");


        //Return all rows with a certain key
        DataTableRow_ExamplePersonTable[] Jim = ExampleTable.GetAllRowsWithKey<DataTableRow_ExamplePersonTable>("Jim");
        foreach(DataTableRow_ExamplePersonTable i in Jim)
        {
            Debug.Log("A Jim is " + i.Age + " Years Old");
        }

        //Random Row
        DataTableRow_ExamplePersonTable RansomRow = ExampleTable.GetRandomRow<DataTableRow_ExamplePersonTable>();
        Debug.Log("Ransom Row is " + RansomRow.Key);

        //Get keys
        string[] AllKey = ExampleTable.GetAllKeys();
        string[] AllKeyUnique = ExampleTable.GetAllKeysUnique();

        Debug.Log("Keys: " + string.Join(", ", AllKey));
        Debug.Log("Unique Keys: " + string.Join(", ", AllKeyUnique));
    }
}
