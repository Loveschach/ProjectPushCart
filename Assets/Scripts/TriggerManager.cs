using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class TriggerManager : MonoBehaviour
{
    public static UnityEvent hitFoodTrigger = new UnityEvent();

    public FoodTrigger[] managedTriggers;
    int currentTrigger = 0;
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState( System.DateTime.Now.Millisecond );
        managedTriggers = managedTriggers.OrderBy( x => Random.Range( 0, managedTriggers.Length ) ).ToArray();
        ChooseNextTrigger();
        hitFoodTrigger.AddListener( ChooseNextTrigger );
    }
    
    void ChooseNextTrigger() {
        FoodTrigger nextTrigger = managedTriggers[currentTrigger];
        nextTrigger.SetActive( true );
        currentTrigger++;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
