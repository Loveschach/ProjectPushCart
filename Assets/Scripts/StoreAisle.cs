using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AisleType
{
        Invalid = -1,
        Baking,
        Beverages,
        BoxedDinners,
        Bread,
        CannedGoods,
        Cereal,
        Cleaning,
        Milk,
        Produce,
        Snacks,
};

public class StoreAisle : MonoBehaviour
{
    public int _length = 1;
    public int _width = 1;
    public AisleType _aisleType = AisleType.Invalid;

    public GameObject _shelfPrefab;

    List<GameObject> _leftShelves = new List<GameObject>();
    List<GameObject> _rightShelves = new List<GameObject>();

    //private 

    // Start is called before the first frame update
    void Start()
    {
        StoreShelf storeShelf = _shelfPrefab.GetComponent<StoreShelf>();
        StoreShelfConfig storeShelfConfig = _shelfPrefab.GetComponent<StoreShelfConfig>();

        int shelfCount = _length / storeShelfConfig._unitTotalLength;
        if(_length % storeShelfConfig._unitTotalLength != 0)
		{
            // To account for "rounding up" we add one shelf if this isnt a clean division
            shelfCount++;
        }

        // Reset length to be round
        _length = shelfCount * storeShelfConfig._unitTotalLength;

        Vector3 pos = transform.position;
        for (int i = 0; i < shelfCount; ++i)
        {
            GameObject LeftShelf = Instantiate(_shelfPrefab, pos, transform.rotation);
            _leftShelves.Add(LeftShelf);

            var rightSidePos = pos + (transform.forward * _width * StoreCreator.GridScale);
            var rightSideRotation = transform.rotation * Quaternion.AngleAxis(180, transform.up);
            GameObject RightShelf = Instantiate(_shelfPrefab, rightSidePos, transform.rotation);
            RightShelf.transform.localScale = new Vector3(RightShelf.transform.localScale.x, RightShelf.transform.localScale.y, -RightShelf.transform.localScale.z);
            _rightShelves.Add(RightShelf);

            pos += transform.right * -storeShelfConfig._unitTotalLength * StoreCreator.GridScale;
        }

        CreateProducts(_leftShelves[0], storeShelf.ShelfData);
        CreateProducts(_rightShelves[0], storeShelf.ShelfData);

    }

    void CreateProducts(GameObject Shelf, List<StoreShelfData> ShelfData)
	{

        foreach (var shelfData in ShelfData)
        {
            // TODO: Clone support later...
            if (shelfData.IsClone)
                continue;
            //if (shelfData.Enum != ShelfType.Middle)
            //    continue;

            var gameObject = new GameObject();
            var newComponent = gameObject.AddComponent<ProductSpawner>();
            newComponent.transform.position = Shelf.transform.TransformPoint(shelfData.Position);
            newComponent.transform.rotation = Shelf.transform.rotation;
            newComponent.Contruct(Shelf, _aisleType, shelfData.Enum, _length, shelfData.UnitDepth, shelfData.UnitHeight);

            //_productRows[0].Add(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnDrawGizmos()
	{
        

    }
}
