using UnityEngine;

/// <summary>
/// Class representing cake easter egg
/// </summary>
public class Cake : MonoBehaviour
{
    /// <summary>
    /// Sound played when cake is eaten
    /// </summary>
    [SerializeField] AudioSource cakeSound;
    /// <summary>
    /// Animator component of the cake
    /// </summary>
    Animator animator;
    /// <summary>
    /// Flag indicating if the cake has been eaten
    /// </summary>
    bool activated = false;
    /// <summary>
    /// Method called when the object is created
    /// </summary>
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Method called when the player enters the cake trigger
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !activated)
        {
            activated = true;
            cakeSound.Play();
            animator.SetTrigger("CakeEating");
            Invoke("Destroy", 1.75f);
        }
    }
    /// <summary>
    /// Method called when the cake is eaten - destroys object in the end
    /// </summary>
    void Destroy()
    {
        Destroy(gameObject);
    }
}
