
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ant : MonoBehaviour
{
    //Stats/Properties

    [SerializeField] private float energy;
    [SerializeField] private float hydration;
    private bool returningToQueen = false;
    private bool movingToResource = false;
    private bool regaining = false;
    private GameObject heldObj;
    private GameObject enemyAnt;
    private bool dead = false;

    private AntState antState;
    private AntMovement antMovement;
    private LookForObjects lookForResources;
    private Queen queen;


    //Components

    [SerializeField] private Slider energySlider;
    [SerializeField] private Slider hydrationSlider;


    public enum AntState
    {
        Wandering,
        MovingToResource,
        Drinking,
        HoldingFood,
        Dead,
        Fleeing
    }

    private void Start()
    {
        queen = GameObject.Find("Queen").GetComponent<Queen>();
        antState = AntState.Wandering;
        antMovement = GetComponent<AntMovement>();
        lookForResources = GetComponent<LookForObjects>();
    }
    private void FixedUpdate()
    {
        if (!regaining && hydration > 0)hydration -= 0.02f;
        if (!regaining && energy > 0) energy -= 0.01f;
        SetEnergySlider();
        SetHydrationSlider();
    }
    void Update()
    {
        if (energy <= 0 || hydration <= 0) Die();
        lookForResources.CheckSurroundingArea();
        enemyAnt = lookForResources.AssessEnemyPositions();
        EvaluateState();
    }

    private void EvaluateState()
    {
        switch (antState)
        {
            case AntState.Wandering:
                Wander();
                break;
            case AntState.MovingToResource:
                if (!movingToResource) MoveToObject();
                break;
            case AntState.Drinking:
                Drink();
                break;
            case AntState.HoldingFood:
                if (!returningToQueen)ReturnToQueen();
                break;
            case AntState.Dead:
                Die();
                break;
            case AntState.Fleeing:
                Flee();
                break;
            default:
                break;
        }
    }
    private void Flee()
    {
        if (enemyAnt == null) 
        { 
            antState = AntState.Wandering; 
            antMovement.SetMovementState(AntMovement.MovementState.RandomMovement); 
        }
        else antMovement.SetMovementState(AntMovement.MovementState.Fleeing);
    }
    private void CheckForEnemy()
    {
        if (enemyAnt != null)
        {
            antState = AntState.Fleeing;
            return;
        }
    }
    private void Wander()
    {
        CheckForEnemy();
        EvaluateHealthLevels();
    }
    private void ChangeMovementState(GameObject _obj)
    {
        if (_obj != null)
        {
            antState = AntState.MovingToResource;
            antMovement.SetResourceToMoveTowards(_obj);
        }
    }
    private void EvaluateHealthLevels()
    {

        if (!regaining)
        {
            if (hydration < 70)
            {
                GameObject _water = lookForResources.AssessWaterPosition();
                ChangeMovementState(_water);
                return;
            }
            if (energy < 100)
            {
                GameObject _food = lookForResources.AssessFoodPosition();
                ChangeMovementState(_food);

                return;
            }

        }
    }
    private void MoveToObject()
    {
        antMovement.SetMovementState(AntMovement.MovementState.MovingTowardsResource);
        movingToResource = true;
    }
    private void ReturnToQueen()
    {
        CheckForEnemy();
        returningToQueen = true;
        antMovement.SetMovementState(AntMovement.MovementState.ReturningToQueen);
    }
    private void Drink()
    {
        CheckForEnemy();
        if (!regaining)
        {
            antMovement.SetMovementState(AntMovement.MovementState.Stationary);
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
            antState = AntState.Wandering;
            antMovement.SetMovementState(AntMovement.MovementState.RandomMovement);
            antMovement.SetVelocity(Vector2.zero);
        }
    }
    private void HoldObject()
    {
        heldObj.transform.parent = transform;
        heldObj.transform.position = transform.position + (transform.up * 0.13f);
        if (queen.GetEnergy() < energy)
        {
            ReturnToQueen();
        }
        else
        {
            heldObj.GetComponent<Food>().SetEaten(true, gameObject);
            heldObj.GetComponent<Food>().EatAnimation(this);  
            energy = Mathf.Clamp(energy + 90, 0, 100);
        }
    }
    private void Die()
    {
        dead = true;
        antMovement.SetMovementState(AntMovement.MovementState.Stationary);
        if (antMovement.animator.speed > 0.01f) antMovement.animator.speed -= 0.02f;
        StartCoroutine(FadeTo(0.0f, 2.5f));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Food") && heldObj == null && 
            !collision.gameObject.GetComponent<Food>().GetHeld() )
        {
            antState = AntState.HoldingFood;
            collision.GetComponent<Food>().SetSoughtAfter(true);
            heldObj = collision.transform.gameObject;
            HoldObject();
            collision.gameObject.GetComponent<Food>().SetHeld(true);
            movingToResource = false;
        }
        if (collision.transform.CompareTag("Water") && antState == AntState.MovingToResource)
        {
            antState = AntState.Drinking;
            movingToResource = false;
        }
        if (collision.transform.CompareTag("EnemyAnt"))
        {
            antState = AntState.Dead;
        }
    }
    private void SetHydrationSlider()
    {
        hydrationSlider.value = hydration;
    }
    private void SetEnergySlider()
    {
        energySlider.value = energy;
    }

    public void SetHeldObject(GameObject _heldObject)
    {
        heldObj = _heldObject;
        if (heldObj == null)
        {
            antState = AntState.Wandering;
            antMovement.SetMovementState(AntMovement.MovementState.RandomMovement);
        }
    }
    public bool GetDead()
    {
        return dead;
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
    public float GetHydration()
    {
        return hydration;
    }
    public float GetEnergy()
    {
        return energy;
    }
}


