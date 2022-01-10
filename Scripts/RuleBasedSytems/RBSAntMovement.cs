using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBSAntMovement : MonoBehaviour
{
    public float wanderStrength = 1f;
    public float steerStrength = 2f;
    public float speed = 2f;
    private float variedSpeed;
    private float minVariedSpeed = 2f;
    private float maxVariedSpeed = 15f;
    private EvaluateEnvironment environment;
    private BackgroundGrid backgroundGrid;
    private Vector2 velocity;
    private Vector2 position;
    private Vector2 desiredDirection;
    private bool shouldTurnAround = false;
    private GameObject resource;
    private bool shouldStray = false;
    private int strayCounter = 0;
    private GameObject queen;
    public Animator animator;
    private float velocityMult = 1f;
    private LookForObjects lookForObjects;
    private bool returningToQueen;

 
    private void Start()
    {
        position = new Vector2(transform.position.x, transform.position.y);
        environment = GetComponent<EvaluateEnvironment>();
        backgroundGrid = GameObject.Find("Background").GetComponent<BackgroundGrid>();
        queen = GameObject.Find("InFrontQueen");
        lookForObjects = GetComponent<LookForObjects>();
    }
 

    public void Flee()
    {
        desiredDirection = lookForObjects.GetEscapeRoute();
          
        if (desiredDirection == Vector2.zero)
        {
            velocity *= 0.0001f;
        }
        else
        {
            velocity = (velocity * 6f + desiredDirection).normalized;
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            position += velocity * Time.deltaTime;
            transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle - 90));
            backgroundGrid.DarkenPixel(gameObject, transform.position,0.04f);
        }
        animator.speed = velocity.magnitude;
    }
    
    
    public void MoveTowardsResource()
    {
        velocityMult = Mathf.Clamp(Vector2.Distance(resource.transform.position, transform.position), 0.3f, 1.2f);
        desiredDirection = resource.transform.position - transform.position;
        velocity = (velocity * 1.1f + desiredDirection).normalized * velocityMult;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        position += velocity * Time.deltaTime;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle - 90));
        backgroundGrid.DarkenPixel(gameObject, transform.position,0.04f);
    }
    public void ReturnToQueen()
    {
        strayCounter++;
        Vector2 assess = EvaluateBlockingPositions();
        if (assess != new Vector2(0, 0) || shouldStray)
        {
            shouldStray = true;
            Movement(true);
        }
        else
        {
            desiredDirection = (queen.transform.position - transform.position).normalized;
            Movement(false);
        }

        if (strayCounter > 100)
        {
            shouldStray = false;
            strayCounter = 0;
        }
    }
    public void Movement(bool isRandomMovement)
    {
        if (isRandomMovement)
        {
            desiredDirection = CalculateAvoidanceDirection(isRandomMovement);
            ModerateSpeed();
        }
        Vector2 desiredVelocity = desiredDirection * variedSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength) / 1;
        velocity = Vector2.ClampMagnitude(velocity + (acceleration) * Time.deltaTime, variedSpeed);
        position += velocity * Time.deltaTime;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle - 90));
        backgroundGrid.DarkenPixel(gameObject, transform.position,0.04f);
        SetAntCollisionSight(velocity);
    }
    private Vector2 CalculateAvoidanceDirection(bool isRandomMovement)
    {
        var tmpDir = EvaluateBlockingPositions();
        if (shouldTurnAround)
        {
            return tmpDir;
        }
        else if (isRandomMovement)
        {
            return (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized + tmpDir;
        }
        else return Vector2.zero;
    }
    private void ModerateSpeed()
    {
        variedSpeed = Random.Range(maxVariedSpeed, minVariedSpeed);
        if (variedSpeed < 1)
        {
            animator.speed = 0.5f;
        }
        else
        {
            animator.speed = variedSpeed / 8;
        }
        Decelerate();
    }
    private void Decelerate()
    {
        if (shouldTurnAround)
        {
            if (maxVariedSpeed > 0.5f)
            {
                maxVariedSpeed -= maxVariedSpeed / 2;
            }
            if (minVariedSpeed > 0.3f)
            {
                minVariedSpeed -= minVariedSpeed / 100;
            }
        }
    }
    private void SetAntCollisionSight(Vector2 _velocity)
    {
        if (_velocity.magnitude * 100 > 35)
        {
            environment.SetInFrontPixels((int)(velocity.magnitude * 100 / 1.3f));
            return;
        }
        environment.SetInFrontPixels(25);
    }
    private Vector2 EvaluateBlockingPositions()
    {
        bool[] blockingPositions = environment.GetBlockingPositions();
        bool up = blockingPositions[0];
        bool right = blockingPositions[1];
        bool left = blockingPositions[2];
        if (up || left || right) shouldTurnAround = true;
        else ResetMovementVariables();


        if (up && left && right) return -transform.up;
        if (up && !left && !right)
        {
            if (!returningToQueen)
            {
                return transform.right;
            }
            return AssessClosestToQueen();
        }
        if (right && !left) return -transform.right;
        if (left && !right) return transform.right;
        if (left && right && !up) return transform.right;
        return Vector2.zero;
    }
    public void SetResourceToMoveTowards(GameObject _resource)
    {
        resource = _resource;
    }
    private void ResetMovementVariables()
    {
        shouldTurnAround = false;
        maxVariedSpeed = 15f;
        minVariedSpeed = 2f;
    }
    private Vector2 AssessClosestToQueen()
    {
        if (Vector2.Distance(transform.position + transform.right, queen.transform.position)
            < Vector2.Distance(transform.position + -transform.right, queen.transform.position))
        {
            return transform.right;
        }
        return -transform.right;
    }
 
    public void SetVelocity(Vector2 _velocity)
    {
        velocity = _velocity;
    }
    public void SetReturningToQueen(bool _returningToQueen)
    {
        returningToQueen = _returningToQueen;
    }
}
