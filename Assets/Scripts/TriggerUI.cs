using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriggerUI : MonoBehaviour
{
    public TMP_Text score;
    public TMP_Text time;
    public TMP_Text inventory;
    public float GAME_LENGTH = 180;
    int currentScore = 0;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        score.text = "Score: 0";
        TriggerManager.hitFoodTrigger.AddListener( UpdateScore );

        CartInventory.OnInventoryChange.AddListener( UpdateInventory );
    }

    // Update is called once per frame
    void Update()
    {
        if( ( GAME_LENGTH - ( Time.time - startTime ) ) <= 0 ) {
            ResetScore();
            startTime = Time.time;
        }
        time.text = "Time: " + Mathf.Round( ( GAME_LENGTH - ( Time.time - startTime ) ) );
    }

    void ResetScore() {
        currentScore = 0;
        score.text = "Score: " + currentScore;
    }

    void UpdateScore() {
        currentScore++;
        score.text = "Score: " + currentScore;
	}
    void UpdateInventory(Dictionary<string, int> cartInventory)
    {
        inventory.text = "Inventory\n---------\n";

        var HackSpawner = GameObject.FindFirstObjectByType<ProductSpawner>();
        DataTable_StoreInventory storeInventory = HackSpawner._inventory;

        foreach (var productData in cartInventory)
        {
            var product = storeInventory.GetRow<DataTableRow_StoreInventory>(productData.Key);
            inventory.text += product.Name + ": " + productData.Value + "\n";
        }
    }
}
