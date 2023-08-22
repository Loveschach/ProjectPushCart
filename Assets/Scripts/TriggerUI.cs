using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriggerUI : MonoBehaviour
{
    public TMP_Text score;
    public TMP_Text time;
    public TMP_Text inventory;
    public TMP_Text shoppingList;
    public float GAME_LENGTH = 180;
    int currentScore = 0;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        TriggerManager.hitFoodTrigger.AddListener(UpdateScore);
        CartInventory.OnInventoryChange.AddListener(UpdateInventory);
        ShoppingList.OnShoppingListChange.AddListener(UpdateShoppingList);

        if (score != null)
        {
            score.text = "Score: 0";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (time != null)
        {
            if ((GAME_LENGTH - (Time.time - startTime)) <= 0)
            {
                ResetScore();
                startTime = Time.time;
            }
            time.text = "Time: " + Mathf.Round((GAME_LENGTH - (Time.time - startTime)));
        }
    }

    void ResetScore() 
    {
        if (score != null)
        {
            currentScore = 0;
            score.text = "Score: " + currentScore;
        }
    }

    void UpdateScore()
    {
        if (score != null)
        {
            currentScore++;
            score.text = "Score: " + currentScore;
        }
	}
    void UpdateInventory(Dictionary<string, int> cartInventory)
    {
        if (inventory != null)
        {
            inventory.text = "Inventory\n---------\n";

            DataTable_StoreInventory storeInventory = StoreCreator.GetStoreInventory();

            foreach (var productData in cartInventory)
            {
                var product = storeInventory.GetRow<DataTableRow_StoreInventory>(productData.Key);
                inventory.text += product.Name + ": " + productData.Value + "\n";
            }
        }
    }
    void UpdateShoppingList(Dictionary<string, int> list)
    {
        if (shoppingList != null)
        {
            shoppingList.text = "Shopping List\n--------------\n";
            
            DataTable_StoreInventory storeInventory = StoreCreator.GetStoreInventory();

            foreach (var productData in list)
            {
                var product = storeInventory.GetRow<DataTableRow_StoreInventory>(productData.Key);
                shoppingList.text += product.Name + ": " + productData.Value + "\n";
            }
        }
    }
}
