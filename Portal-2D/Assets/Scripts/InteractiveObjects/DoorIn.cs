using UnityEngine;

/// <summary>
/// Class responsible for the behaviour of the door which player enters through
/// </summary>
public class DoorIn : MonoBehaviour
{
    /// <summary>
    /// Sound of the door closing
    /// </summary>
    [SerializeField] AudioSource doorClose;
    /// <summary>
    /// Sound of the door locking
    /// </summary>
    [SerializeField] AudioSource doorLock;
    /// <summary>
    /// Awake is called when the script instance is being loaded, here responsible for playing door sounds
    /// </summary>
    void Awake()
    {
        doorClose.Play();
        doorLock.PlayDelayed(doorClose.clip.length);
    }
}
