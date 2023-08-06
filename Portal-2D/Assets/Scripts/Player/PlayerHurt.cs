using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to controll the appearance and behavior of the player while hurt
/// </summary>
public class PlayerHurt : MonoBehaviour
{
    /// <summary>
    /// Indicates whether the player is being hurt or not.
    /// </summary>
    public static bool isHurt = false;
    /// <summary>
    /// SpriteRenderer component - used to change the color of the player when hurt.
    /// </summary>
    private SpriteRenderer[] spriteRenderer;
    /// <summary>
    /// Time that the player is hurt for.
    /// </summary>
    private int recolorTime = 0;
    /// <summary>
    /// Number of lives the player has.
    /// </summary>
    private int lives = 5;
    /// <summary>
    /// Counts the time since the last hurt
    /// </summary>
    private int timeSinceLastHurt = 0;
    /// <summary>
    /// Color to be applied while the player is hurt
    /// </summary>
    Color red = new Color(1f, 0f, 0f, 1f);
    /// <summary>
    /// Color to be applied to the player when not hurt
    /// </summary>
    Color white = new Color(1f, 1f, 1f, 1f);


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        foreach (var item in spriteRenderer)
        {
            item.color = white;
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (isHurt)
        {
            lives--;
            timeSinceLastHurt = 0;
            foreach (var item in spriteRenderer)
            {
                item.color = red;
            }
            recolorTime = 100;
            isHurt = false;
        }
        else timeSinceLastHurt++;
        if (lives == 0)
        {
            lives = 5;
            PanelManager.Instance.RestartLevel();
            isHurt = false;
        }
        if (recolorTime > 0)
        {
            ReturnToNormal(0.01f);
            recolorTime--;
        }
        if (timeSinceLastHurt > 1000)
        {
            timeSinceLastHurt = 0;
            if (lives < 5) lives++;
        }
    }

    /// <summary>
    /// Gradually returns the player's color to normal
    /// </summary>
    /// <param name="amount">amount of color to be brung back</param>
    private void ReturnToNormal(float amount)
    {
        Color currentColor = spriteRenderer[0].color;
        Color newColor = new Color(currentColor.r, currentColor.g + amount, currentColor.b + amount, currentColor.a);
        foreach (var item in spriteRenderer)
        {
            item.color = newColor;
        }
    }
}
