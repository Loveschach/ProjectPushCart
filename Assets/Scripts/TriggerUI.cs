using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriggerUI : MonoBehaviour
{
    public TMP_Text score;
    public TMP_Text time;
    public float GAME_LENGTH = 180;
    int currentScore = 0;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        score.text = "Score: 0";
        TriggerManager.hitFoodTrigger.AddListener( UpdateScore );
    }

    // Update is called once per frame
    void Update()
    {
        time.text = "Time: " + Mathf.Round( ( GAME_LENGTH - ( Time.time - startTime ) ) );
    }

    void UpdateScore() {
        currentScore++;
        score.text = "Score: " + currentScore;
	}
}
