using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] GameObject background;
    private BackgroundGrid backgroundGrid;
    private float randomX;
    private float randomY;
    private int counter;
    [SerializeField] GameObject foodPrefab;

    void Start()
    {
        backgroundGrid = GameObject.Find("Background").GetComponent<BackgroundGrid>();
 
    }


    void FixedUpdate()
    {
        counter++;
        if (counter % 150 == 0)
        {
            RandomFoodSpawn();
        }
    }
    private void RandomFoodSpawn()
    {
        bool canSpawn = false;
        do
        {
            canSpawn = EvaluateSpawnPosition();
        }
        while (canSpawn == false);
        GameObject foodObj = Instantiate(foodPrefab);
        foodObj.transform.position += new Vector3(randomX, randomY,0);
    }

    private bool EvaluateSpawnPosition()
    {
        bool canSpawn = true;
        randomX = Random.Range(-9.4f,9.4f);
        randomY = Random.Range(-4.7f,5.7f);
        (int, int)[] surroundingPoints = LocateSurroundingPoints(randomX, randomY);
        canSpawn = canSpawn && !backgroundGrid.EvaluateIfPixelIsBlockedArea(surroundingPoints[0], true);
        canSpawn = canSpawn && !backgroundGrid.EvaluateIfPixelIsBlockedArea(surroundingPoints[1], true);
        canSpawn = canSpawn && !backgroundGrid.EvaluateIfPixelIsBlockedArea(surroundingPoints[2], true);
        canSpawn = canSpawn && !backgroundGrid.EvaluateIfPixelIsBlockedArea(surroundingPoints[3], true);
        return canSpawn;
    }
    private (int,int)[] LocateSurroundingPoints(float randX, float randY)
    {
        (int,int)[] surroundingPoints = new (int,int)[4];
        (int, int) pixelPos = backgroundGrid.CalculatePixelPos(new Vector2(randX, randY));
        surroundingPoints[0] = (pixelPos.Item1 + 5, pixelPos.Item2);
        surroundingPoints[1] = (pixelPos.Item1 - 5, pixelPos.Item2);
        surroundingPoints[2] = (pixelPos.Item1, pixelPos.Item2 + 5);
        surroundingPoints[3] = (pixelPos.Item1, pixelPos.Item2 - 5);
        return surroundingPoints;

    }
}
