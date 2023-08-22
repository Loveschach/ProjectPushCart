using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShoppingList : MonoBehaviour
{
    static public Dictionary<string, int> ShopList = new Dictionary<string, int>();

    public static UnityEvent<Dictionary<string, int>> OnShoppingListChange = new UnityEvent<Dictionary<string, int>>();

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
            if (ShopList.ContainsKey(randomKey))
                continue;

            int storeQuantity = StoreCreator.StockedItems[randomKey];
            int maxAmountToBuy = Math.Min(10, storeQuantity);

            int amountToBuy = UnityEngine.Random.Range(1, maxAmountToBuy * maxAmountToBuy);
            amountToBuy = (int)MathF.Sqrt((float)amountToBuy);

            ShopList.Add(randomKey, amountToBuy);
        }

        OnShoppingListChange.Invoke(ShopList);
    }

    public static void ClearList()
    {
        ShopList.Clear();
        OnShoppingListChange.Invoke(ShopList);
    }

        // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
