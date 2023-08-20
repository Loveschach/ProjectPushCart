using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AisleSign : MonoBehaviour
{
    public TMP_Text _frontText;
    public TMP_Text _backText;

    public void SetText(AisleType _aisleType)
	{
        _frontText.text = _backText.text = _aisleType.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
