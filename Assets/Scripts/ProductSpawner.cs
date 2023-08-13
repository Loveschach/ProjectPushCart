using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductSpawner : MonoBehaviour
{
    public Transform _endPos;

    // TODO: Move this to a "store" object
    public DataTable_StoreInventory _inventory;
    public DataTable_ProductTypeTable _typeDefinitions;

    public ProductTypes[] _types;

    public float _rotationOffsetMin = 0.0f;
    public float _rotationOffsetMax = 0.0f;

    private Vector3 _spawnDirection = Vector3.right;
    private float _spawnDistance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _spawnDirection = _endPos.position - transform.position;
        _spawnDistance = _spawnDirection.magnitude;
        _spawnDirection.Normalize();

        SpawnProducts();
    }
    void SpawnProducts()
    {
        string[] keys = _inventory.GetAllKeys();
        var totalWeight = 0.0f;

        foreach (var key in keys)
        {
            var row = _inventory.GetRow<DataTableRow_StoreInventory>(key);

            var shelfHasProductType = Array.Exists(_types, element => element == row.Type);
            if (shelfHasProductType)
            {
                totalWeight += row.Odds;
            }
        }


        foreach (var key in keys)
        {
            var row = _inventory.GetRow<DataTableRow_StoreInventory>(key);

            //row.Type;
            
            //Instantiate(, new Vector3(x, y, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.4f, 0.2f, 1.0f);
        Gizmos.DrawCube(transform.position, new Vector3(0.1f, 0.1f, 0.1f));

        // Can we make this visable ONLY when the object is selected AND not have it hidden as you move it around (since then it's not selected)
        Gizmos.color = new Color(0.2f, 0.4f, 0.8f, 1.0f);
        Gizmos.DrawSphere(_endPos.position, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        //TODO: Draw the shelf grid
    }
}
