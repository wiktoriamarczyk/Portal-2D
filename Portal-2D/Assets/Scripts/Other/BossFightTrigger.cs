using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BossFightTrigger : MonoBehaviour
{
   [SerializeField] UnityEvent triggerBossFight;
   [SerializeField] TextMeshProUGUI prompt;
    bool isInitialized = false;

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

    IEnumerator DisplayPrompt()
    {
        yield return new WaitForSeconds(5f);
        prompt.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
