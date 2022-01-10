using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private bool soughtAfter = false;
    private bool eaten;
    private Vector3 targetDirection;
    private bool held = false;
    private void FixedUpdate()
    {
        if (eaten)
        {
            transform.position += targetDirection;
        }
    }
    public void SetSoughtAfter(bool _soughtAfter)
    {
        soughtAfter = _soughtAfter;
    }
    public bool GetSoughtAfter()
    {
        return soughtAfter;
    }
    public void SetHeld(bool _held)
    {
        held = _held;
    }
    public bool GetHeld()
    {
        return held;
    }
    public void SetEaten(bool _eaten)
    {
        eaten = _eaten;
        targetDirection = new Vector3(0, -0.005f, 0);
    }
    public void SetEaten(bool _eaten, GameObject _ant)
    {
        eaten = _eaten;
        targetDirection = (_ant.transform.position - transform.position).normalized * 0.005f;
    }
    public bool GetEaten()
    {
        return eaten;
    }
    public void EatAnimation(Ant _ant)
    {
        StartCoroutine(ScaleDownAnimation(1f, _ant));
    }
    public void EatAnimation(RBSAnt _ant)
    {
        StartCoroutine(ScaleDownAnimation(1f, _ant));
    }
    IEnumerator ScaleDownAnimation(float time, Ant _ant)
    {
        float i = 0;
        float rate = 1 / time;

        Vector3 fromScale = transform.localScale;
        Vector3 toScale = Vector3.zero;
        while (i < 1)
        {
            i += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(fromScale, toScale, i);
            yield return 0;
        }
        Destroy(gameObject);
        _ant.SetHeldObject(null);
    }
    IEnumerator ScaleDownAnimation(float time, RBSAnt _ant)
    {
        float i = 0;
        float rate = 1 / time;

        Vector3 fromScale = transform.localScale;
        Vector3 toScale = Vector3.zero;
        while (i < 1)
        {
            i += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(fromScale, toScale, i);
            yield return 0;
        }
        Destroy(gameObject);
        _ant.SetHeldObject(null);
    }
   
}

