using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForObjects : MonoBehaviour
{
    public float sightRadius;
    public float sightAngle;
    private float xDiff = 0;
    private float yDiff = 0;

    private Vector2 position;
    GameObject background;
    private BackgroundGrid backgroundGrid;
    private List<GameObject> foods = new List<GameObject>();
    private List<GameObject> bodiesOfWater = new List<GameObject>();
    private List<GameObject> enemyAnts = new List<GameObject>();
    private Vector2 escapeRoute;
    private GameObject pos;
    void Start()
    {
        background = GameObject.Find("Background");
        backgroundGrid = background.GetComponent<BackgroundGrid>();
        pos = new GameObject();
    }


    public void CheckSurroundingArea()
    {
        Collider2D[] surroundingObjects = Physics2D.OverlapCircleAll(position, sightRadius);
        List<GameObject> foodObjects = new List<GameObject>();
        List<GameObject> waterObjects = new List<GameObject>();
        List<GameObject> enemyObjects = new List<GameObject>();
        foreach (Collider2D resource in surroundingObjects)
        {
            if (resource.gameObject.CompareTag("Food"))
            {
                foodObjects.Add(resource.gameObject);
            }
            if (resource.gameObject.CompareTag("Water"))
            {
                waterObjects.Add(resource.gameObject);
            }
            if (resource.gameObject.CompareTag("EnemyAnt"))
            {
                enemyObjects.Add(resource.gameObject);
            }
        }
        
        foods = foodObjects;
        bodiesOfWater = waterObjects;
        enemyAnts = enemyObjects;
    }
    public GameObject AssessFoodPosition()
    {
        position = new Vector2(transform.position.x, transform.position.y);
        foreach(GameObject food in foods)
        {
            if (CheckPathPossible(food.transform)&&food.GetComponent<Food>().GetSoughtAfter() == false)
            {
                return food;
            }
        }
        return null;
    }
    public GameObject AssessWaterPosition()
    {
        position = new Vector2(transform.position.x, transform.position.y);
        foreach (GameObject body in bodiesOfWater)
        {
            if (CheckPathPossible(body.transform))
            { 
                return body;
            }
        }
        return null;
    }
    public GameObject AssessEnemyPositions()
    {
        
        float minDistance = 100;
        GameObject closest = null;
        foreach(GameObject enemyAnt in enemyAnts)
        {
            if (Vector2.Distance(position, enemyAnt.transform.position) < minDistance)
            {
                minDistance = Vector2.Distance(position, enemyAnt.transform.position);
                closest = enemyAnt;
            }
        }
        if (closest)
        {
            escapeRoute = GivePossibleEscapePath(closest);
            return closest;
        }
        else return null;
       
    }
    private Vector2 GivePossibleEscapePath(GameObject enemyAnt)
    {
        Transform _transform = pos.transform;
        Vector2 oppositeDirection = transform.position - enemyAnt.transform.position;
        Vector2 oppositeToRight = (oppositeDirection - Vector2.Perpendicular(oppositeDirection)) / 2;
        Vector2 oppositeToLeft = (oppositeDirection + Vector2.Perpendicular(oppositeDirection)) / 2;
        Vector2 right = -Vector2.Perpendicular(oppositeDirection);
        Vector2 left = Vector2.Perpendicular(oppositeDirection);
        _transform.position = new Vector2(transform.position.x, transform.position.y) + oppositeDirection*2;
        if (CheckPathPossible(_transform)) return oppositeDirection;
        _transform.position = new Vector2(transform.position.x, transform.position.y) + oppositeToRight*2;
        if (CheckPathPossible(_transform)) return oppositeToRight;
        _transform.position = new Vector2(transform.position.x, transform.position.y) + oppositeToLeft*2;
        if (CheckPathPossible(_transform)) return oppositeToLeft;
        _transform.position = new Vector2(transform.position.x, transform.position.y) + right*2;
        if (CheckPathPossible(_transform)) return right;
        _transform.position = new Vector2(transform.position.x, transform.position.y) + left*2;
        if (CheckPathPossible(_transform)) return left;
        else return Vector2.zero;
    }
    private bool CheckPathPossible(Transform _objectPos)
    {
        Vector2[] midPoints = new Vector2[9];

        AssignDifferences(_objectPos);
        bool pathPossible = true;
        Vector2 tempTransform = new Vector2(transform.position.x, transform.position.y);
        for (int i = 0; i < 4; i++)
        {

            tempTransform += new Vector2( xDiff / 8, yDiff / 8);
            midPoints[i] = tempTransform;
        }
        int section = 0;
        foreach(Vector2 transform in midPoints)
        {
   
            (int,int) pixelPos =  backgroundGrid.CalculatePixelPos(new Vector2(midPoints[section].x, midPoints[section].y));
            pathPossible = pathPossible && !backgroundGrid.EvaluateIfPixelIsBlockedArea(pixelPos);
            section++;
        }
        return pathPossible;

    }
    private void AssignDifferences(Transform _objectPos)
    {
        Vector2 direction = _objectPos.position - transform.position;
        xDiff = direction.x;
        yDiff = direction.y;
    }
   
    public Vector2 GetEscapeRoute()
    {
        return escapeRoute;
    }
}

