using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EvaluateEnvironment : MonoBehaviour
{
    private bool[] blockingPositions = new bool[3];
    private Sprite backgroundSprite;
    private BackgroundGrid backgroundGrid;
    private GameObject background;

    private Color waterColour = new Color(0.612f,0.859f, 0.906f, 1.00f);
    private Color groundColour = new Color(0.741f, 0.510f, 0.388f, 1.000f);
    private int width;
    private int height;
    private int verticalOffset;
    private int pixelsInFront = 40;
    private Dictionary<(int, int), bool> pixels = new Dictionary<(int, int), bool>();

    private void Start()
    {
        background = GameObject.Find("Background");
        backgroundSprite = background.GetComponent<SpriteRenderer>().sprite;
        backgroundGrid = background.GetComponent<BackgroundGrid>();
        width =(int)backgroundSprite.rect.width;
        height = (int)backgroundSprite.rect.height;
        verticalOffset = (int)(background.transform.position.y * 100)/2;
        
    }
    private void Update()
    {
        CheckAntVision();  
    }
    private void CheckAntVision()
    {
        (int, int)[] pixelPositions = PopulatePixelPositions();
        UpdateBlockingPositions(pixelPositions);
    }
    private void UpdateBlockingPositions((int,int)[] positionsToCheck)
    {
        int elem = 0;
        foreach((int,int)_position in positionsToCheck)
        {
            blockingPositions[elem] = backgroundGrid.EvaluateIfPixelIsBlockedArea(_position);
            elem++;
        }
    }
    private (int,int)[] PopulatePixelPositions()
    {
        Vector2 upRight = new Vector2((transform.up.x + transform.right.x) / 2, (transform.up.y + transform.right.y) / 2);
        Vector2 upLeft = new Vector2((transform.up.x + -transform.right.x) / 2, (transform.up.y + -transform.right.y) / 2);
        Vector2[] positionsToCheck = new Vector2[] {new Vector2((transform.up).x, (transform.up).y).normalized,
        upRight,upLeft};
        (int, int)[] pixelPositions = new (int, int)[3];
        int elem = 0;
        foreach (Vector2 _position in positionsToCheck)
        {
            var (x, y) = CalculatePixelPos(new Vector2(transform.position.x, transform.position.y), (int)width, (int)height, _position.x, _position.y);
            pixelPositions[elem].Item1 = x;
            pixelPositions[elem].Item2 = y;
            elem++;
        }
        return pixelPositions;
    }

    private (int x, int y) CalculatePixelPos(Vector2 _worldPos, int width, int height , float xOffset, float yOffset)
    {
        var xPos = 0;
        var yPos = 0;
        if (_worldPos.x > 0)
        {
            xPos = (width / 2) + (int)(_worldPos.x * 100 / 2) + (int)(xOffset * pixelsInFront);
        }
        else
        {
            xPos = (width / 2) - (int)(Mathf.Abs(_worldPos.x) * 100 / 2) + (int)(xOffset * pixelsInFront);
        }
        if (_worldPos.y > 0)
        {
            yPos = (height / 2) + (int)(_worldPos.y * 100 / 2) + (int)(yOffset * pixelsInFront) - verticalOffset;
        }
        else
        {
            yPos = (height / 2) - (int)(Mathf.Abs(_worldPos.y) * 100 / 2) + (int)(yOffset * pixelsInFront) - verticalOffset;
        }
        return (xPos, yPos);
    }

    public bool[] GetBlockingPositions()
    {
        return blockingPositions;
    }
 
    public void SetInFrontPixels(int _inFrontPixels)
    {
        pixelsInFront = _inFrontPixels;
    }
  

}
