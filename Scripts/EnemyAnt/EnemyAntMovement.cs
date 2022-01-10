using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAntMovement : MonoBehaviour
{
    public float wanderStrength = 1f;
    public float steerStrength = 2f;
    public float speed = 2f;
    private float variedSpeed;
    private float minVariedSpeed = 2.5f;
    private float maxVariedSpeed = 18f;
    private EvaluateEnvironment environment;
    private BackgroundGrid backgroundGrid;
    private Vector2 velocity;
    private Vector2 position;
    private Vector2 desiredDirection;
    private bool shouldTurnAround = false;
    private GameObject preyedAnt;
    private CheckForNearbyAnts checkForNearbyAnts;
    private float accelerationCounter = 0f;
    private bool chasing = false;
    private float speedChange = 1f;
    public Animator animator;
    private float velocityMult = 1f;
    private bool up = false;
 
    private void Start()
    {
        position = new Vector2(transform.position.x, transform.position.y);
        environment = GetComponent<EvaluateEnvironment>();
        checkForNearbyAnts = GetComponent<CheckForNearbyAnts>();
        backgroundGrid = GameObject.Find("Background").GetComponent<BackgroundGrid>();

    }
    private void FixedUpdate()
    {
        if (CheckTargetAntWithinRange()) MoveTowardsPreyedAnt();
        else
        {
            checkForNearbyAnts.CheckSurroundingArea();
            preyedAnt = checkForNearbyAnts.AssessAntPosition();
            Movement(true);
        }
    }

    private bool CheckTargetAntWithinRange()
    {
        if (preyedAnt != null && !preyedAnt.GetComponent<Ant>().GetDead())
        {
            if (Vector2.Distance(position, preyedAnt.transform.position) <= 1.25f) return true;
            else
            {
                ResetSpeedChange();
                return false;
            }

        }
        ResetSpeedChange();
        return false;
    }
    private void ChangeSpeed()
    {
        if (up && speedChange < 3.5) speedChange += 0.03f;
        else if (speedChange > 0.5) speedChange -= 0.01f;

    }
    private void MoveTowardsPreyedAnt()
    {
        accelerationCounter += Time.deltaTime;
        if (accelerationCounter > 3)
        {
            UpOrDown();
            ChangeSpeed();
        }
        velocityMult = Mathf.Clamp(Vector2.Distance(preyedAnt.transform.position, transform.position), 0.35f, 1.2f);
        desiredDirection = preyedAnt.transform.position - transform.position;
        if (Vector2.Distance(preyedAnt.transform.position, transform.position) < 1) velocity = (velocity + desiredDirection).normalized * velocityMult;
        else velocity = (velocity * 15f + desiredDirection).normalized * velocityMult;
        velocity *= speedChange;
        animator.speed = velocity.magnitude;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        position += velocity * Time.deltaTime;
        transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, angle - 90));
        backgroundGrid.DarkenPixel(gameObject, transform.position,0.1f);
    }
    private void UpOrDown()
    {
        if (!chasing)
        {
            chasing = true;
            if (Random.Range(0, 2) == 1) up = true;
            else up = false;
        }
    }

    private void ResetSpeedChange()
    {
        chasing = false;
        accelerationCounter = 0f;
        speedChange = 1f;
    }
    private void Movement(bool isRandomMovement)
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
        backgroundGrid.DarkenPixel(gameObject, transform.position,0.1f);
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
        if (up && !left && !right) return transform.right;
        if (right && !left) return -transform.right;
        if (left && !right) return transform.right;
        if (left && right && !up) return transform.right;
        return Vector2.zero;
    }
    public void SetPreyedAnt(GameObject _resource)
    {
        preyedAnt = _resource;
    }
    private void ResetMovementVariables()
    {
        shouldTurnAround = false;
        maxVariedSpeed = 18f;
        minVariedSpeed = 2.5f;
    }

    public GameObject GetPreyedAnt()
    {
        return preyedAnt;
    }
}
