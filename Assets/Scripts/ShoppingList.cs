using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShoppingListEntry : IEquatable<ShoppingListEntry>
{
    public ShoppingListEntry(string key, int quantity = 0)
	{
        _key = key;
        _quantity = quantity;
    }

    private string _key;
    public string Key   // property
    {
        get { return _key; }   // get method
        set { _key = value; }  // set method
    }

    private int _quantity;
    public int Quantity   // property
    {
        get { return _quantity; }   // get method
        set { _quantity = value; }  // set method
    }

    private bool _complete;
    public bool Complete   // property
    {
        get { return _complete; }   // get method
        set { _complete = value; }  // set method
    }
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        ShoppingListEntry objAsPart = obj as ShoppingListEntry;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }
    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public bool Equals(ShoppingListEntry other)
    {
        if (other == null) return false;
        return (this.Key.Equals(other.Key));
    }
};

public class ShoppingList : MonoBehaviour
{
    public static List<ShoppingListEntry> ShopList = new List<ShoppingListEntry>();

    public static UnityEvent<List<ShoppingListEntry>> OnShoppingListChange = new UnityEvent<List<ShoppingListEntry>>();

    public bool IsItemComplete(int index)
    {

        return false;
    }
    public static bool IsItemComplete(string key)
    {
        var entry = ShopList.Find(x => x.Key.Contains(key));
        if (entry == null)
            return false;

        // TEMP!
        return entry.Quantity > 0;
    }
    public static bool IsListComplete()
	{
        foreach(var item in ShopList)
		{

		}
        return false;
	}

    public static ShoppingListEntry GetEntry(string key)
    {
        return ShopList.Find(x => x.Key.Contains(key));
    }

    public static void GenerateList()
    {
        // TODO: This needs to be some kind of algorithm! 

        int amountOfItems = UnityEngine.Random.Range(4, 6);

        List<string> keyList = new List<string>(StoreCreator.StockedItems.Keys);
        if (keyList.Count == 0)
            return;

        for (int i = 0; i < amountOfItems; ++i)
        {
            int randomItem = UnityEngine.Random.Range(1, StoreCreator.StockedItems.Keys.Count);

            string randomKey = keyList[randomItem];
            var entry = GetEntry(randomKey);
            if (entry != null)
                continue;

            int storeQuantity = StoreCreator.StockedItems[randomKey];
            int maxAmountToBuy = Math.Min(10, storeQuantity);

            // Cheeky way to favor lower numbers... Cube it then double sqrt it!
            int amountToBuy = UnityEngine.Random.Range(1, maxAmountToBuy * maxAmountToBuy * maxAmountToBuy);
            amountToBuy = (int)MathF.Sqrt(MathF.Sqrt((float)amountToBuy));

            ShoppingListEntry newEntry = new ShoppingListEntry(randomKey, amountToBuy);
            ShopList.Add(newEntry);
        }

        OnShoppingListChange.Invoke(ShopList);
    }

    public static void ClearList()
    {
        ShopList.Clear();
        OnShoppingListChange.Invoke(ShopList);
    }
    void OnUpdateInventory(Dictionary<string, int> list)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        CartInventory.OnInventoryChange.AddListener(OnUpdateInventory);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
