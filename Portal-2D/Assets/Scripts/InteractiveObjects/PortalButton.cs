using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class responsible for the behaviour of portal buttons
/// </summary>
public class PortalButton : MonoBehaviour
{
    /// <summary>
    /// Animator component
    /// </summary>
    Animator animator;
    /// <summary>
    /// Event invoked when the button is pressed
    /// </summary>
    [SerializeField] UnityEvent onButtonPressed;
    /// <summary>
    /// Event invoked when the button is released
    /// </summary>
    [SerializeField] UnityEvent onButtonReleased;
    /// <summary>
    /// Sound played when button was pressed
    /// </summary>
    [SerializeField] AudioSource buttonPressed;
    /// <summary>
    /// Sound played when button was released
    /// </summary>
    [SerializeField] AudioSource buttonReleased;

    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Method called when the object pressed the button
    /// </summary>
    /// <param name="collider">the object with which the collision occurred</param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        animator.SetTrigger("ButtonPressed");
        onButtonPressed.Invoke();
        buttonPressed.Play();
    }
    /// <summary>
    ///  Method called when the object released the button
    /// </summary>
    /// <param name="collider">the object with which the collision occurred</param>
    void OnTriggerExit2D(Collider2D collider)
    {
        animator.SetTrigger("ButtonReleased");
        onButtonReleased.Invoke();
        buttonReleased.Play();
    }
}
