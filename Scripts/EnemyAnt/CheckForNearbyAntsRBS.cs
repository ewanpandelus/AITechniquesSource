using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForNearbyAntsRBS : MonoBehaviour
{
    public float sightRadius;
    public float sightAngle;
    private float xDiff = 0;
    private float yDiff = 0;

    private Vector2 position;
    GameObject background;
    private BackgroundGrid backgroundGrid;
    private List<GameObject> nearbyAnts = new List<GameObject>();
    private EnemyAntMovementRBS enemyAnt;
    void Start()
    {
        background = GameObject.Find("Background");
        backgroundGrid = background.GetComponent<BackgroundGrid>();
        enemyAnt = GetComponent<EnemyAntMovementRBS>();
    }


    public void CheckSurroundingArea()
    {
        Collider2D[] surroundingAnts = Physics2D.OverlapCircleAll(position, sightRadius);
        List<GameObject> ants = new List<GameObject>();
        foreach (Collider2D ant in surroundingAnts)
        {

            if (enemyAnt.GetPreyedAnt() == ant.gameObject && ant.gameObject && ant.gameObject.GetComponent<RBSAnt>().GetDead()) enemyAnt.SetPreyedAnt(null);
            if (ant.gameObject.CompareTag("Ant") && !ant.gameObject.GetComponent<RBSAnt>().GetDead())
            {

                ants.Add(ant.gameObject);
            }
        }

        nearbyAnts = ants;

    }
    public GameObject AssessAntPosition()
    {
        position = new Vector2(transform.position.x, transform.position.y);
        foreach (GameObject ant in nearbyAnts)
        {
            if (CheckPathPossible(ant))
            {
                return ant;
            }
        }
        return null;
    }

    private bool CheckPathPossible(GameObject _resource)
    {
        Vector2[] midPoints = new Vector2[4];

        AssignDifferences(_resource);
        bool pathPossible = true;
        Vector2 tempTransform = new Vector2(transform.position.x, transform.position.y);
        for (int i = 0; i < 4; i++)
        {

            tempTransform += new Vector2(xDiff / 5, yDiff / 5);
            midPoints[i] = tempTransform;
        }
        int section = 0;
        foreach (Vector2 transform in midPoints)
        {

            (int, int) pixelPos = backgroundGrid.CalculatePixelPos(new Vector2(midPoints[section].x, midPoints[section].y));
            pathPossible = pathPossible && !backgroundGrid.EvaluateIfPixelIsBlockedArea(pixelPos);
            section++;
        }
        return pathPossible;

    }
    private void AssignDifferences(GameObject _resource)
    {
        Vector2 direction = _resource.transform.position - transform.position;
        xDiff = direction.x;
        yDiff = direction.y;

    }

}

