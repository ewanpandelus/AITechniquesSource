using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    private float antsToLift = 0;
    private List<GameObject> liftingAnts = new List<GameObject>();
    private List<Vector3> positionAdditions = new List<Vector3>();
    private List<Vector3> rotationAdditions = new List<Vector3>();
    private CircleCollider2D thisCollider = new CircleCollider2D();
    void Start()
    {
        thisCollider = GetComponent<CircleCollider2D>();
    }
    public void SetColliderEnabled(bool _val)
    {
        GetComponent<CircleCollider2D>().enabled = _val;
    }
    private void FixedUpdate()
    {
        if(liftingAnts.Count == antsToLift)
        {
            Vector3 addingPos = new Vector3();
           // Vector3 addingRot = new Vector3();
            foreach (Vector3 addPos in positionAdditions)
            {
                addingPos += addPos;
            }
            //foreach(Vector3 addRot in rotationAdditions)
          //  {
             //   addingRot += addRot;
          //  }
            addingPos /= positionAdditions.Count;
           // addingRot /= rotationAdditions.Count;
            this.transform.position += addingPos;
          //  transform.Rotate(addingRot);
            positionAdditions.Clear();
            //rotationAdditions.Clear();
        }
    }
    public void ClearPositionAdditions()
    {
        positionAdditions.Clear();
    }
    public void ClearRotationsAdditions()
    {
        rotationAdditions.Clear();
    }
    public void ClearLiftingAnts()
    {
        liftingAnts.Clear();
    }
    public bool IsLiftingAnt(GameObject _ant)
    {
        return liftingAnts.Contains(_ant);
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Red"))
        {
            if (thisCollider.bounds.Contains(collider.bounds.min)
              && thisCollider.bounds.Contains(collider.bounds.max)
              &&!liftingAnts.Contains(collider.gameObject))
            {
                liftingAnts.Add(collider.gameObject);

            }
            if(liftingAnts.Count == antsToLift)
            {
                foreach(GameObject ant in liftingAnts)
                {
                    ant.transform.parent = this.transform;
                
                }
             
            }
        }

        else if (collider.gameObject.CompareTag("Goal") && liftingAnts.Count == antsToLift)
        {
            try
            {
                foreach (GameObject ant in liftingAnts.ToArray())
                {
             
                }
            }
            catch (Exception _ex)
            {
                Debug.Log(_ex.ToString()); ;
            }


        }
    }
   
 

   
}
