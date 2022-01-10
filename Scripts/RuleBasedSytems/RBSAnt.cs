using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RBSAnt : MonoBehaviour
{
    [SerializeField] private float energy;
    [SerializeField] private float hydration;


    [SerializeField] private Queen queen;

    private GameObject heldObj;
    private bool atWater = false;
    private bool regaining = false;
    private bool dead = false;

    //Components

    [SerializeField] private Slider energySlider;
    [SerializeField] private Slider hydrationSlider;



    
    private void FixedUpdate()
    {
        if (energy <= 0 || hydration <= 0) dead = true;
        if (!regaining && hydration > 0) hydration -= 0.02f;
        if (energy > 0) energy -= (0.01f);
        
        SetEnergySlider();
        SetHydrationSlider();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Food") && heldObj == null && !collision.gameObject.GetComponent<Food>().GetHeld())
        {
            heldObj = collision.transform.gameObject;
            collision.gameObject.GetComponent<Food>().SetHeld(true);
            
        }
        if (collision.transform.CompareTag("Water"))
        {
            atWater = true;
        }
        if (collision.transform.CompareTag("EnemyAnt"))
        {
            dead = true;
        }
    }
    public void Eat()
    {
        heldObj.GetComponent<Food>().SetEaten(true, gameObject);
        heldObj.GetComponent<Food>().EatAnimation(this);
        heldObj = null;
        energy = Mathf.Clamp(energy + 90, 0, 100);
    }
    public void HoldObject()
    {
        if (heldObj.transform.parent != transform)
        {
            heldObj.transform.parent = transform;
            heldObj.transform.position = transform.position + (transform.up * 0.13f);
        }
    }
    public void Die()
    {

        StartCoroutine(FadeTo(0.0f, 2.5f));
    }
    public void Drink()
    {
        if (!regaining)
        {
            regaining = true;
        }
        if (hydration < 100)
        {
            hydration += 0.5f;
            return;
        }
        else if (hydration >= 100)
        {
            regaining = false;
            atWater = false; 

        }
    }
    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = GetComponent<Renderer>().material.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }
        Destroy(gameObject);
    }
    public void SetHeldObject(GameObject _heldObject)
    {
        heldObj = _heldObject;
    }
    private void SetHydrationSlider()
    {
        hydrationSlider.value = hydration;
    }
    private void SetEnergySlider()
    {
        energySlider.value = energy;
    }

    public float GetEnergy()
    {
        return energy;
    }
    public float GetHydration()
    {
        return hydration;
    }
    public bool GetHoldingResource()
    {
        if (heldObj == null) return false;
        else return true;
    }
    public bool GetAtWater()
    {
        return atWater;
    }
    public bool GetDead()
    {
        return dead;
    }

}
