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
    public int _aisleWidth = 30;
    public AisleType _aisleType = AisleType.Invalid;

    public static Color[] DebugAisleColors = 
    { 
        new Color(1,0,0,0.25f),
        new Color(1,1,0,0.25f),
        new Color(0,1,1,0.25f),
        new Color(0,0,1,0.25f),
        new Color(1,0,1,0.25f),
    };

    public GameObject _shelfPrefab;
    private StoreShelfConfig _storeShelfConfig;
    private int _shelfCount = 0;

    List<GameObject> _leftShelves = new List<GameObject>();
    List<GameObject> _rightShelves = new List<GameObject>();

    private void OnValidate()
    {
        _storeShelfConfig = _shelfPrefab.GetComponent<StoreShelfConfig>();

        _shelfCount = _length / _storeShelfConfig._unitTotalLength;
        if (_length % _storeShelfConfig._unitTotalLength != 0)
        {
            // To account for "rounding up" we add one shelf if this isnt a clean division
            _shelfCount++;
        }
    }

	// Start is called before the first frame update
	void Start()
    {
        StoreShelf storeShelf = _shelfPrefab.GetComponent<StoreShelf>();

        // Reset length to be round
        //_length = _shelfCount * _storeShelfConfig._unitTotalLength;

        Vector3 pos = transform.position;
        for (int i = 0; i < _shelfCount; ++i)
        {
            GameObject LeftShelf = Instantiate(_shelfPrefab, pos, transform.rotation);
            _leftShelves.Add(LeftShelf);

            var rightSidePos = GetRightShelfPos(pos);

            GameObject RightShelf = Instantiate(_shelfPrefab, rightSidePos, transform.rotation);
            RightShelf.transform.localScale = new Vector3(RightShelf.transform.localScale.x, RightShelf.transform.localScale.y, -RightShelf.transform.localScale.z);
            _rightShelves.Add(RightShelf);

            if (i == 0 || i == (_shelfCount - 1))
            {
                Vector3 signPos = pos + ((rightSidePos - pos) * 0.5f);
                signPos += Vector3.up * 2.3f;
                float frontOfAisleDirection = i == 0 ? 1 : -1;
                signPos += transform.right * 0.5f * frontOfAisleDirection;
                GameObject aisleSign1 = Instantiate(StoreCreator.GetAisleSignPrefab(), signPos, transform.rotation);
                var aisleSignComponent = aisleSign1.GetComponent<AisleSign>();
                aisleSignComponent.SetText(_aisleType);
            }

            pos = GetAdjacentShelfPos(pos);
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

    Vector3 GetRightShelfPos(Vector3 leftShelfPos)
    {
        return leftShelfPos + (transform.forward * _aisleWidth * StoreCreator.GridScale);
    }

    Vector3 GetAdjacentShelfPos(Vector3 shelfPos)
    {
        return shelfPos + transform.right * -_storeShelfConfig._unitTotalLength * StoreCreator.GridScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            var shelfMesh = _shelfPrefab.GetComponentInChildren<MeshFilter>();

            var shelfPos = transform.position;
            Gizmos.color = new Color(0.0f, 0.0f, 0.0f, 0.25f);
            for (int i = 0; i < _shelfCount; ++i)
            {
                Gizmos.DrawMesh(shelfMesh.sharedMesh, shelfPos, transform.rotation);

                var rightShelfRotation = transform.rotation * Quaternion.AngleAxis(180, transform.up);
                Gizmos.DrawMesh(shelfMesh.sharedMesh, GetRightShelfPos(shelfPos), rightShelfRotation);

                shelfPos = GetAdjacentShelfPos(shelfPos);
            }
        }
    }
}
