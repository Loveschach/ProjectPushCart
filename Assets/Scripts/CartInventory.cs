using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private IDictionary<string, int> _inventory;

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
    public IDictionary<string, int> GetInventory()
    {
        return _inventory;
    }

    public ICollection<string> GetProductList()
    {
        return _inventory.Keys;
    }

    public void AddProduct(string key, int amount)
    {
        if (_inventory.ContainsKey(key))
        {
            _inventory[key] += amount;
        }
        else
        {
            _inventory.Add(key, amount);
        }
    }

    public void RemoveProduct(string key, int amount)
    {
        if (_inventory.ContainsKey(key))
        {
            _inventory[key] -= amount;
        }
    }

    public void ClearInventory()
	{
        _inventory.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetCartShelfPoint(), 0.1f);
    }
}
