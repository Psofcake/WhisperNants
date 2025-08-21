using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class TestVolume : MonoBehaviour
{
    public float sa;
    public Color color;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Cyan();    
    }
    public void Black()
    {
        GetComponent<Volume>().sharedProfile.TryGet<ColorAdjustments>(out var colorAdjustments);
        colorAdjustments.saturation.value = sa; //sa = -100
    }

    public void Cyan()
    {
        GetComponent<Volume>().sharedProfile.TryGet<ColorAdjustments>(out var colorAdjustments);
        colorAdjustments.colorFilter.value = color; //Color.cyan;
    }
}
