using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Queen : MonoBehaviour
{
    private float energy = 100f;
    [SerializeField] private Slider energySlider;

    void FixedUpdate()
    {   
        if (energy > 0)
        {
            energy -= 0.01f;
            energySlider.value = energy;
        }
    }
    public void Eat()
    {
        energy = Mathf.Clamp(energy + 10, 0, 100);
    }
    public float GetEnergy()
    {
        return energy;
    }
    
}
