using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProductData : MonoBehaviour
{
    private string _key { get; set; }
    private int _amount = 0;

    public string Key   // property
    {
        get { return _key; }   // get method
        set { _key = value; }  // set method
    }
    public int Amount   // property
    {
        get { return _amount; }   // get method
        set { _amount = value; }  // set method
    }

    public void SetData(string key, int amount = 1)
    {
        _key = key;
        _amount = amount;
    }
}

public class CartInventory : MonoBehaviour
{
    public static UnityEvent<Dictionary<string, int>> OnInventoryChange = new UnityEvent<Dictionary<string, int>>();

    private Dictionary<string, int> _inventory = new Dictionary<string, int>();

    public Vector3 _lookShelfPoint = new Vector3(0.0f, 0.5f, 0.0f);
    public Vector3 GetCartShelfPoint()
    {
        return transform.TransformPoint(_lookShelfPoint);
    }

    public int GetProductAmount(string key)
    {
        if (_inventory.ContainsKey(key))
        {
            return _inventory[key];
        }

        return -1;
    }
    public Dictionary<string, int> GetInventory()
    {
        return _inventory;
    }

    public ICollection<string> GetProductList()
    {
        return _inventory.Keys;
    }

    public void AddProduct(string key, int amount)
    {
        bool productInInventory = _inventory.ContainsKey(key);

        if (productInInventory)
        {
            _inventory[key] += amount;
        }
        else
        {
            _inventory.Add(key, amount);
        }

        OnInventoryChange.Invoke(_inventory);
    }

    public void RemoveProduct(string key, int amount)
    {
        if (_inventory.ContainsKey(key))
        {
            _inventory[key] -= amount;
        }


        OnInventoryChange.Invoke(_inventory);
    }

    public void ClearInventory()
	{
        _inventory.Clear();

        OnInventoryChange.Invoke(_inventory);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnDrawGizmosSelected()
	{
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetCartShelfPoint(), 0.05f);
    }
}
