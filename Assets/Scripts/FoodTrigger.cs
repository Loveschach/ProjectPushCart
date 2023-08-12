using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FoodTrigger : MonoBehaviour
{
    public Material inActiveMaterial;
    public Material activeMaterial;
    public Renderer cube;
    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetActive( bool active ) {
        cube.material = active ? activeMaterial : inActiveMaterial;
        isActive = active;
    }

	private void OnTriggerEnter( Collider other ) {
		if ( other.gameObject.tag == "Player" && isActive ) {
            SetActive( false );
            TriggerManager.hitFoodTrigger.Invoke();
        }
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
