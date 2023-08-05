using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurt : MonoBehaviour
{
    public static bool isHurt = false;
    private SpriteRenderer spriteRenderer;
    private int time = 0;
    private int lives = 5;
    private float startTime;
    Color red = new Color(1f, 0f, 0f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHurt)
        {
            spriteRenderer.color = red;
            time = 100;
        }
        if (time > 0)
        {
            ReturnToNormal(0.01f);
            time--;
        }
    }

    private void ReturnToNormal(float amount)
    {
        Color currentColor = spriteRenderer.color;
        Color newColor = new Color(currentColor.r, currentColor.g + amount, currentColor.b + amount, currentColor.a);
        spriteRenderer.color = newColor;
    }
}
