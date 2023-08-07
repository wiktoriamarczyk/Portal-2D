using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class responsible for triggering the boss fight
/// </summary>
public class BossFightTrigger : MonoBehaviour
{
   [SerializeField] UnityEvent triggerBossFight;
   [SerializeField] TextMeshProUGUI prompt;
   bool isInitialized = false;
   const float promptDisplayTime = 5f;

    /// <summary>
    /// Method called when the trigger is entered - initialize the boss fight
    /// </summary>
    /// <param name="collision">object with which we have collision</param>
    void OnTriggerEnter2D( Collider2D collision )
    {
        if (collision.gameObject.CompareTag("Player") && !isInitialized)
        {
            isInitialized = true;
            triggerBossFight?.Invoke();
            prompt.gameObject.SetActive(true);
            StartCoroutine(DisplayPrompt());
        }
    }
    /// <summary>
    /// Coroutine which displays the prompt for 5 seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayPrompt()
    {
        yield return new WaitForSeconds(promptDisplayTime);
        prompt.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
