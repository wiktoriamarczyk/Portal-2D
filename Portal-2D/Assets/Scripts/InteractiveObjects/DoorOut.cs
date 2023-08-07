using UnityEngine;

/// <summary>
/// Class responsible for the behaviour of the door leading out of the level
/// </summary>
public class DoorOut : MonoBehaviour
{
    /// <summary>
    /// Sound of opening the door
    /// </summary>
    [SerializeField] AudioSource doorOpen;
    /// <summary>
    /// Sound of closing the door
    /// </summary>
    [SerializeField] AudioSource doorClose;
    /// <summary>
    /// Sound of winning the level
    /// </summary>
    [SerializeField] AudioSource levelWinning;
    /// <summary>
    /// Panel which is displayed when the player wins the game
    /// </summary>
    [SerializeField] GameObject winningPanel;
    /// <summary>
    /// Is the door open?
    /// </summary>
    public static bool isActive = false;
    /// <summary>
    /// Animator component
    /// </summary>
    Animator animator;
    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    void Awake()
    {
        animator = GetComponent<Animator>();
        winningPanel?.SetActive(false);
    }
    /// <summary>
    /// Method called when the door is opened
    /// </summary>
    public void OpenDoor()
    {
        Debug.Log("Otwieram drzwi");
        isActive = true;
        animator.SetTrigger("OpenDoor");
        doorOpen.Play();
    }
    /// <summary>
    /// Method called when the door is closed
    /// </summary>
    public void CloseDoor()
    {
        Debug.Log("Zamykam drzwi");
        isActive = false;
        animator.SetTrigger("CloseDoor");
        doorClose.Play();
    }
    /// <summary>
    /// Method called when the player enters the door
    /// </summary>
    /// <param name="collision">the object with which the collision occurred - in this case only player</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && isActive)
        {
            levelWinning?.Play();
            if (PortalSceneManager.Instance.GetSceneIndex() < 3)
                Invoke("LoadNextLevel", 2f);
            if (PortalSceneManager.Instance.GetSceneIndex() == 3)
                winningPanel?.SetActive(true);

        }
    }
    /// <summary>
    /// Method called when the player won the level
    /// </summary>
    void LoadNextLevel()
    {
        PortalSceneManager.Instance.LoadNextLevel();
    }
}
