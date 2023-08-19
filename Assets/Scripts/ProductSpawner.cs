using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Splines;

// This is a simple 
public class ProductGrabTrigger : MonoBehaviour
{
	private GameObject _closestProduct = null;
	private Vector3 _projectedCartPosition = Vector3.zero;

	private List<GameObject> _products;
	private DataTableRow_StoreInventory _productData;
	private DataTableRow_ProductTypeTable _typeDefinition;
	private bool _middleShelf = true;

	public void InitTriggerData(List<GameObject> products, DataTableRow_StoreInventory productData, DataTableRow_ProductTypeTable typeDefinition, bool middleShelf = true)
	{
		_products = products;
		_productData = productData;
		_typeDefinition = typeDefinition;
		_middleShelf = middleShelf;
	}

	public Vector3 GetShelfLine()
	{
		return _products[_products.Count - 1].transform.position - _products[0].transform.position;
	}

	public void UpdatePlayerInTrigger(Collider other)
	{
		var cartInventory = other.GetComponent<CartInventory>();
		var cartController = other.GetComponent<CartController>();
		var productData = GetComponent<ProductData>();
		float productWidth = _typeDefinition.WidthUnits * ProductSpawner._gridScale;

		// Get the cart point from the player cart
		Vector3 cartPoint = cartInventory.GetCartShelfPoint();

		// Project the cart point for inventory onto the shelf line
		Vector3 A = _products[0].transform.position;
		Vector3 AP = cartPoint - A;
		Vector3 AB = GetShelfLine();
		_projectedCartPosition = A + Vector3.Dot(AP, AB) / Vector3.Dot(AB, AB) * AB;

		// Vector to projected point along the shelf line
		Vector3 toProjectedPort = _projectedCartPosition - A;

		// Make sure the projected point is actually within the line
		float checkBehind = Vector3.Dot(toProjectedPort, -transform.right);
		if (checkBehind > 0.0f && (toProjectedPort.magnitude < (GetShelfLine().magnitude + productWidth)))
		{
			// Use the "size" of this product (width * scale + gap) to know which product is closest by using the magnitude!
			int closestProductIndex = (int)MathF.Floor(toProjectedPort.magnitude / (productWidth + ProductSpawner._gridGap));
			closestProductIndex = Math.Min(closestProductIndex, _products.Count - 1);

			_closestProduct = _products[closestProductIndex];

			var productRenderer = _closestProduct.GetComponent<Renderer>();
			if (productRenderer.enabled && cartController.Grabbing)
			{
				productRenderer.enabled = false;
				cartInventory.AddProduct(productData.Key, productData.Amount);
			}
		}
		else
		{
			_closestProduct = null;
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void OnDrawGizmos()
	{
		if (_closestProduct)
		{
			Gizmos.color = new Color(0.9f, 0.0f, 0.25f, 1.0f);

			Vector3 gridPos = _closestProduct.transform.position;
			gridPos.y += ProductSpawner._gridScale * 0.5f;
			Gizmos.DrawWireCube(gridPos, new Vector3(ProductSpawner._gridScale, ProductSpawner._gridScale, ProductSpawner._gridScale));

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(_projectedCartPosition, 0.1f);

			Gizmos.DrawLine(_products[_products.Count - 1].transform.position, _products[0].transform.position);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		UpdatePlayerInTrigger(other);
	}

	private void OnTriggerEnter(Collider other)
	{
	}

	private void OnTriggerExit(Collider other)
	{
		_closestProduct = null;
	}
}

public class ProductSpawner : MonoBehaviour
{
	static public float _gridScale = 0.1f;
	static public float _gridGap = 0.01f;
	static public float _productScale = 0.1f;


	public Transform _endPos;
	//public GameObject _spline;
	//public SplineInstantiate _splineInstantiate;

	// TODO: Move this to a "store" object
	public DataTable_StoreInventory _inventory;
	public DataTable_ProductTypeTable _typeDefinitions;

	public ProductTypes[] _types;

	public int _amountOfRows = 1;
	public float _rotationOffsetMin = 0.0f;
	public float _rotationOffsetMax = 0.0f;

	public float _triggerDepth = 2.0f;
	public float _triggerHeight = 4.0f;

	private Vector3 _spawnDirection = Vector3.right;
	private float _spawnDistance = 0.0f;
	private int _shelfSpots = 0;

	public List<List<GameObject>> _productsSpawned = new List<List<GameObject>>();

	private void OnValidate()
	{
		_gridScale = MathF.Max(_gridScale, 0.1f);
		_productScale = MathF.Max(_productScale, 0.01f);
	}

	// Start is called before the first frame update
	void Start()
	{
		InitSpawnVars();
		SpawnProducts();
	}

	void InitSpawnVars()
	{
		_spawnDirection = _endPos.position - transform.position;
		_spawnDistance = _spawnDirection.magnitude;
		_spawnDirection.Normalize();
		_shelfSpots = (int)MathF.Round(_spawnDistance / (_gridScale + _gridGap));

		//var splineComponent = _spline.GetComponent<Spline>();
		//splineComponent.Evaluate(0.5f, out position, out target, out up);
	}

	void SpawnProducts()
	{
		// TODO: Might need to turn this into an array and/or a For instead of For Each for future perf!

		List<DataTableRow_StoreInventory> rows = new List<DataTableRow_StoreInventory>();
		var totalWeight = 0.0f;

		string[] keys = _inventory.GetAllKeys();
		foreach (var key in keys)
		{
			var row = _inventory.GetRow<DataTableRow_StoreInventory>(key);

			var shelfHasProductType = Array.Exists(_types, element => element == row.Type);
			if (shelfHasProductType)
			{
				rows.Add(row);
				totalWeight += row.Odds;
			}
		}

		// TODO: Might want to shuffle the rows!


		Vector3 productPos = transform.position;

		int productWidth = -1;
		int lastProductWidth = -1;

		foreach (var row in rows)
		{
			DataTableRow_ProductTypeTable typeDefinition;
			if( GetTypeDefinition(row.Type, out typeDefinition) )
			{
				List<GameObject> productsOfType = new List<GameObject>();

				lastProductWidth = productWidth;
				productWidth = typeDefinition.WidthUnits;

				float myWeight = row.Odds / totalWeight;
				int amountToSpawn = (int)Math.Round(myWeight * _shelfSpots);
				amountToSpawn /= productWidth;
				amountToSpawn = Math.Max(amountToSpawn, 1);

				for (int i = 0; i < amountToSpawn; i++)
				{
					float nextSpotDist = _gridGap;
					nextSpotDist += _gridScale * (float)productWidth * 0.5f;
					if (lastProductWidth != -1)
					{
						nextSpotDist += _gridScale * (float)lastProductWidth * 0.5f;
					}

					productPos += _spawnDirection * nextSpotDist;

					Quaternion productQuat = new Quaternion(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z, 1.0f);

					for (int j = 0; j < _amountOfRows; j++)
					{
						Vector3 thisProductPos = productPos;
						thisProductPos += transform.forward * typeDefinition.WidthUnits * -(_gridScale + _gridGap) * j;

						GameObject newProduct = Instantiate(typeDefinition.GameObject, thisProductPos, productQuat);
						newProduct.transform.localScale = new Vector3(_productScale, _productScale, _productScale);

						var productRenderer = newProduct.GetComponent<Renderer>();
						productRenderer.materials[1].SetColor("_Color", row.ProductColor);
						/*
						foreach (var mat in productRenderer.materials)
						{
							if(mat.name.Contains("Blue") || mat.name.Contains("White"))
							{
								mat.SetColor("_Color", row.ProductColor);
							}
						}
						*/
						//string result = productRenderer.materials[1].GetTag("label", true, "Nothing");

						ProductData productData = newProduct.AddComponent(typeof(ProductData)) as ProductData;
						productData.SetData(row.Key);

						if (j == 0)
						{
							productsOfType.Add(newProduct);
						}
					}

					lastProductWidth = productWidth;
				}

				_productsSpawned.Add(productsOfType);

				AddProductTrigger(productsOfType[0], productsOfType[productsOfType.Count - 1], productWidth);

				ProductGrabTrigger productGrabTrigger = productsOfType[0].AddComponent(typeof(ProductGrabTrigger)) as ProductGrabTrigger;
				productGrabTrigger.InitTriggerData(productsOfType, row, typeDefinition);

			}
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.8f, 0.4f, 0.2f, 1.0f);
		Gizmos.DrawCube(transform.position, new Vector3(0.01f, 0.01f, 0.01f));

		// Can we make this visable ONLY when the object is selected AND not have it hidden as you move it around (since then it's not selected)
		Gizmos.color = new Color(0.2f, 0.4f, 0.8f, 1.0f);
		Gizmos.DrawSphere(_endPos.position, 0.01f);
	}

	private void OnDrawGizmosSelected()
	{
		InitSpawnVars();

		Gizmos.color = new Color(0.9f, 0.0f, 0.25f, 1.0f);

		Vector3 sd = transform.InverseTransformDirection(_spawnDirection);
		Vector3 gridPos = Vector3.zero;
		gridPos.y += _gridScale * 0.5f;
		gridPos += sd * _gridScale * 0.5f;

		for (int i = 0; i < _shelfSpots; i++)
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube(gridPos, new Vector3(_gridScale, _gridScale, _gridScale));

			float nextSpotDist = _gridScale + _gridGap;
			gridPos += sd * nextSpotDist;
		}
	}

	private void AddProductTrigger(GameObject firstProduct, GameObject lastProduct, float productWidth)
	{
		BoxCollider boxCollider = firstProduct.AddComponent(typeof(BoxCollider)) as BoxCollider;

		Vector3 midPoint = firstProduct.transform.position + lastProduct.transform.position;
		midPoint *= 0.5f;
		midPoint += firstProduct.transform.forward * 0.5f * _triggerDepth;

		float width = Vector3.Distance(firstProduct.transform.position, lastProduct.transform.position);
		width += _gridScale * productWidth;

		boxCollider.center = firstProduct.transform.InverseTransformPoint(midPoint);
		boxCollider.size = new Vector3(width, _triggerHeight, _triggerDepth);
		boxCollider.size /= _productScale;
		boxCollider.isTrigger = true;
	}

	private bool GetTypeDefinition(ProductTypes type, out DataTableRow_ProductTypeTable typeDefinition)
	{
		// TODO: Can turn the table rows into a map for speed up!
		
		string[] keys = _typeDefinitions.GetAllKeys();
		foreach (var key in keys)
		{
			var row = _typeDefinitions.GetRow<DataTableRow_ProductTypeTable>(key);

			if (row.Type == type)
			{
				typeDefinition = row;
				return true;
			}
		}

		// TODO: How do I add an error here?

		typeDefinition = new DataTableRow_ProductTypeTable();
		return false;
	}
}
