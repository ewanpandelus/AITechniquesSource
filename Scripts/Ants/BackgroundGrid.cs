using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGrid : MonoBehaviour
{
    private Dictionary<(int, int), bool> pixels = new Dictionary<(int, int), bool>();
    private Texture2D backgroundTexture;
    private Texture2D copiedTexture;
    private Sprite backgroundSprite;
    private int width;
    private int height;
    private float pixelDarken = 0.08f;
    [SerializeField] private Color caveWallColour = new Color(0.212f, 0.212f, 0.212f, 1.00f);
    private Color caveWallColour2 = new Color(0.129f, 0.184f, 0.200f, 1.000f);
    [SerializeField] private Color waterColour;
    private int verticalOffset;
    [SerializeField] GameObject background;
    void Start()
    {
        backgroundSprite = background.GetComponent<SpriteRenderer>().sprite;
        width = (int)backgroundSprite.rect.width;
        height = (int)backgroundSprite.rect.height;
        backgroundTexture = backgroundSprite.texture;
        copiedTexture = new Texture2D(width, height);
        copiedTexture.SetPixels(backgroundTexture.GetPixels());
        copiedTexture.Apply();
        verticalOffset = (int)(background.transform.position.y * 100) / 2;
        InitialisePixelDict();
    }
    private void InitialisePixelDict()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (EvaluateIfPixelIsBlockedArea((i, j)))
                {
                    pixels.Add((i, j), true);
                    continue;
                }
                pixels.Add((i, j), false);
            }
        }
    }
    public void DarkenPixel(GameObject ant, Vector2 _position, float distance)
    {
        Vector2 left = new Vector2(_position.x + (ant.transform.right.x * distance)
            + (ant.transform.up.x * -0.13f), _position.y + (ant.transform.right.y * distance) + (ant.transform.up.y * -0.13f));
        Vector2 right = new Vector2(_position.x - (ant.transform.right.x * distance)
         + (ant.transform.up.x * -0.13f), _position.y - (ant.transform.right.y * distance) + (ant.transform.up.y * -0.13f));
        (int, int) backLeftPixel = CalculatePixelPos(left);
        (int, int) backrightPixel = CalculatePixelPos(right);

        ApplyColouring(backLeftPixel);
        ApplyColouring(backrightPixel);

    }
    private void ApplyColouring((int, int) _pixel)
    {
        if (CheckIfPixelBeenColoured(_pixel))
        {
            return;
        }
        Color pixelColour = backgroundTexture.GetPixel(_pixel.Item1, _pixel.Item2);
        Color darkerPixel = pixelColour - new Color(pixelDarken, pixelDarken, pixelDarken);
        pixels[(_pixel.Item1, _pixel.Item2)] = true;
        backgroundTexture.SetPixel(_pixel.Item1, _pixel.Item2, darkerPixel);
        StartCoroutine(ChangeColour(darkerPixel, pixelColour, 30f, _pixel));
    }
    private bool CheckIfPixelBeenColoured((int, int) _pixelPos)
    {
        try
        {
            return pixels[_pixelPos];
        }
        catch
        {
            return true;
        }
        
    }
    public (int x, int y) CalculatePixelPos(Vector2 _worldPos)
    {
        var xPos = 0;
        var yPos = 0;
        if (_worldPos.x > 0)
        {
            xPos = ((int)width / 2) + (int)(Mathf.Abs(_worldPos.x) * 100 / 2);
        }
        else
        {
            xPos = ((int)width / 2) - (int)(Mathf.Abs(_worldPos.x) * 100 / 2);
        }
        if (_worldPos.y > 0)
        {
            yPos = ((int)height / 2) + (int)(Mathf.Abs(_worldPos.y) * 100 / 2) - verticalOffset;
        }
        else
        {
            yPos = ((int)height / 2) - (int)(Mathf.Abs(_worldPos.y) * 100 / 2) - verticalOffset;
        }
        return (xPos, yPos);
    }

    private void OnApplicationQuit()
    {
        backgroundTexture.SetPixels(copiedTexture.GetPixels());

    }
    private IEnumerator ChangeColour(Color startColour, Color endColour, float time, (int, int) _pixel)
    {

        float i = 0;
        float rate = 1 / time;
        while (i < 1)
        {
            i += Time.deltaTime * rate;
            Color color = Color.Lerp(startColour, endColour, i);
            backgroundTexture.SetPixel(_pixel.Item1, _pixel.Item2, color);
            yield return 0;
        }
        pixels[_pixel] = false;
    }
    public bool EvaluateIfPixelIsBlockedArea((int, int) _pos)
    {
        Color pixelColour = backgroundTexture.GetPixel(_pos.Item1, _pos.Item2);
        return AreColoursSame(pixelColour, caveWallColour) || AreColoursSame(pixelColour, caveWallColour2);
    }
    public bool EvaluateIfPixelIsBlockedArea((int, int) _pos, bool _isWater)
    {
        if (!_isWater) return false;
        Color pixelColour = backgroundTexture.GetPixel(_pos.Item1, _pos.Item2);
        return AreColoursSame(pixelColour, caveWallColour) ||
            AreColoursSame(pixelColour, caveWallColour2)||
            AreColoursSame(pixelColour, waterColour);
    }

    public bool AreColoursSame(Color colourOne, Color colourTwo)
    {
        float r1 = (float)System.Math.Round(colourOne.r, 1);
        float g1 = (float)System.Math.Round(colourOne.g, 1);
        float b1 = (float)System.Math.Round(colourOne.b, 1);
        float a1 = (float)System.Math.Round(colourOne.a, 1);
        float r2 = (float)System.Math.Round(colourTwo.r, 1);
        float g2 = (float)System.Math.Round(colourTwo.g, 1);
        float b2 = (float)System.Math.Round(colourTwo.b, 1);
        float a2 = (float)System.Math.Round(colourTwo.a, 1);
        return r1 == r2 && g1 == g2 && b1 == b2 && a1 == a2;
    }
    void Update()
    {
        backgroundTexture.Apply();
    }
}
