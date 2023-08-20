using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProductTypes
{
    Invalid,
    MilkPint,
    MilkQuart,
    MilkGallon,
    BreadLoaf,
    PaperTowelSingle,
    ToiletPaperPack,
    SprayBottle,
    SodaSixPack,
    SodaSingle,
    Cereal,
    CannedGood,
}

public class StoreCreator : MonoBehaviour
{
    static public float GridScale = 0.0625f;
    static public float GridGap = 0.0f;
    static public float ProductScale = 0.9f;
    
    public DataTable_StoreInventory Inventory;
    public DataTable_ProductTypeTable TypeDefinitions;

    public static DataTable_StoreInventory GetStoreInventory()
	{
        var storeCreatorObject = FindFirstObjectByType<StoreCreator>();
        return storeCreatorObject.Inventory;
    }
    public static DataTable_ProductTypeTable GetTypeDefinitions()
    {
        var storeCreatorObject = FindFirstObjectByType<StoreCreator>();
        return storeCreatorObject.TypeDefinitions;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnValidate()
    {
        GridScale = MathF.Max(GridScale, 0.001f);
        ProductScale = MathF.Max(ProductScale, 0.01f);
    }
}
