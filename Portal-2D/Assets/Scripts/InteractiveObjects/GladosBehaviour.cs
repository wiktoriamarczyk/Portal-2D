using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Component responsible for the behaviour of the Glados boss.
/// </summary>
public class GladosBehaviour : MonoBehaviour
{
    [SerializeField] List<AudioSource> neurotoxinDialogues;
    [SerializeField] AudioSource deadSound;
    [SerializeField] List<AudioSource> hitSounds;
    [SerializeField] Sprite deadSprite;
    [SerializeField] float neurotoxinFillTime = 60f;
    [SerializeField] ProgressBar neurotoxinProgressBar;
    [SerializeField] ProgressBar healthBar;
    [SerializeField] GameObject neurotoxinEffect;
    [SerializeField] GameObject cake;
    [SerializeField] float neurotoxinMinLvl;
    [SerializeField] float neurotoxinMaxLvl;
    [SerializeField] float dmgPerHit = 20f;
    [SerializeField] GameObject particleHitEffect;
    float health;
    float maxHealth = 100f;
    float neurotoxinFillSpeed;
    float neurotoxinLvl;
    bool isAwake = false;
    float neurotoxinDistance;
    float neurotoxinDisableSpeed = 0.2f;
    int soundIndex = -1;
    List<int> hitSoundIndices = new List<int>();

    /// <summary>
    /// Awake is called when the script instance is being loaded. Initialize all variables.
    /// </summary>
    void Awake()
    {
        cake.SetActive(false);
        neurotoxinEffect.SetActive(false);
        Vector3 neurotoxinPosition = neurotoxinEffect.transform.position;
        neurotoxinEffect.transform.position = new Vector3(neurotoxinPosition.x, neurotoxinMinLvl, neurotoxinPosition.z);
        health = maxHealth;
        neurotoxinFillSpeed = 1f / neurotoxinFillTime;
        neurotoxinDistance = neurotoxinMaxLvl - neurotoxinMinLvl;
    }
    /// <summary>
    /// Initialize sounds and neurotoxin effect - boss is awaken.
    /// </summary>
    public void Initialize()
    {
        isAwake = true;
        neurotoxinEffect.SetActive(true);
        neurotoxinDialogues[0].Play();
        neurotoxinDialogues[1].PlayDelayed(neurotoxinDialogues[0].clip.length);
    }
    /// <summary>
    /// Update is called once per frame. Update neurotoxin level.
    /// </summary>
    void Update()
    {
        if (!isAwake)
        {
            return;
        }
        SetNeurotoxinLevel(neurotoxinLvl + neurotoxinFillSpeed * Time.deltaTime);
    }
    /// <summary>
    /// Smoothly disable neurotoxin effect.
    /// </summary>
    /// <returns></returns>
    IEnumerator NeurotoxinDisable()
    {
        while (neurotoxinEffect.transform.position.y >= neurotoxinMinLvl)
        {
            SetNeurotoxinLevel(neurotoxinLvl - neurotoxinDisableSpeed * Time.deltaTime);
            yield return null;
        }
        neurotoxinEffect.SetActive(false);
    }
    /// <summary>
    /// Updates neurotoxin level
    /// </summary>
    /// <param name="value">new value of neurotoxin level</param>
    void SetNeurotoxinLevel(float value)
    {
        neurotoxinLvl = Mathf.Clamp(value, 0, 1);
        if (neurotoxinLvl == 1)
        {
            PanelManager.Instance.RestartLevel();
        }
        neurotoxinProgressBar.SetProgressBarValue(neurotoxinLvl);
        Vector3 neurotoxinPosition = neurotoxinEffect.transform.position;
        neurotoxinEffect.transform.position = new Vector3(neurotoxinPosition.x, neurotoxinMinLvl + neurotoxinDistance * neurotoxinLvl, neurotoxinPosition.z);
    }
    /// <summary>
    /// Returns sound index for hit sound.
    /// </summary>
    /// <returns>sound index from list</returns>
    int PickRandomHitIndex()
    {
        if (hitSoundIndices.Count == 0)
        {
            for (int i = 0; i < hitSounds.Count; ++i)
            {
                hitSoundIndices.Add(i);
            }
            hitSoundIndices = hitSoundIndices.OrderBy( x => UnityEngine.Random.value ).ToList( );
        }
        int result = hitSoundIndices[0];
        hitSoundIndices.RemoveAt(0);
        return result;
    }
    /// <summary>
    /// Called when the object enters the trigger. If it is a cube, decrease health and play hit sound. Spawn particles in hit point.
    /// </summary>
    /// <param name="collision">object with thich the collision occured</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAwake || !collision.gameObject.CompareTag("Cube"))
        {
            return;
        }

        var particles = Instantiate(particleHitEffect);
        var hitPoint = collision.contacts[0].point;
        particles.transform.position = new Vector2(hitPoint.x, hitPoint.y);

        health -= dmgPerHit;
        healthBar.SetProgressBarValue(health / maxHealth);
        if (soundIndex == -1 || !hitSounds[soundIndex].isPlaying && !neurotoxinDialogues[1].isPlaying)
        {
            soundIndex = PickRandomHitIndex();
            hitSounds[soundIndex].Play();
        }

        if (health < 0)
        {
           isAwake = false;
           GetComponent<SpriteRenderer>().sprite = deadSprite;
           hitSounds[soundIndex].Stop();
           deadSound.Play();
           cake.SetActive(true);
           StartCoroutine(NeurotoxinDisable());
        }
    }
}
