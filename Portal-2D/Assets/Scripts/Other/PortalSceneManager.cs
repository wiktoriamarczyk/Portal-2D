using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

/// <summary>
/// Class responsible for managing current scene behaviour
/// </summary>
public class PortalSceneManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static PortalSceneManager Instance { get; private set; }
    /// <summary>
    /// Panel showed when player needs to pause the game
    /// </summary>
    [SerializeField] GameObject pausePanel;
    /// <summary>
    /// Player game object
    /// </summary>
    [SerializeField] GameObject player;
    /// <summary>
    /// Music volume value saved in player prefs
    /// </summary>
    const string musicVolumePlayerPrefsName = "MusicVolume";
    /// <summary>
    /// Name of the start scene
    /// </summary>
    const string startSceneName = "StartScene";
    /// <summary>
    /// Awake is called when the script instance is being loaded - responsible for setting singleton instance,
    /// and setting music volume
    /// </summary>
    void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        else
            Instance = this;

        SetMusicVolume(PlayerPrefs.GetFloat(musicVolumePlayerPrefsName));
    }

    /// <summary>
    /// Update is called every frame if the MonoBehaviour is enabled - responsible for managing the behaviour of the player input
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pausePanel.SetActive(true);
            Camera.main.GetComponent<PostProcessLayer>().enabled = true;
            player.SetActive(false);
        }
        if (!pausePanel.activeSelf)
        {
            player.SetActive(true);
        }
    }
    /// <summary>
    /// Method responsible for setting music volume
    /// </summary>
    /// <param name="volume">music volume</param>
    public void SetMusicVolume(float volume)
    {
        var musicVolume = Mathf.Clamp(volume, 0, 1.01f);
        AudioListener.volume = musicVolume;
    }
    /// <summary>
    /// Deactivates pause panel
    /// </summary>
    public void DeactivatePausePanel()
    {
        pausePanel.SetActive(false);
        player.SetActive(true);
    }
    /// <summary>
    /// Restarts current level
    /// </summary>
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// Method responsible for loading starting scene
    /// </summary>
    public void LoadStartScene()
    {
        SceneManager.LoadScene(startSceneName);
    }
    /// <summary>
    /// Method responsible for quitting the game
    /// </summary>
    public void QuitGame()
    {
        PanelManager.Instance.QuitGame();
    }
    /// <summary>
    /// Method responsible for loading next level
    /// </summary>
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    /// <summary>
    /// Returns current scene index
    /// </summary>
    /// <returns></returns>
    public int GetSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    /// <summary>
    /// Clear singleton instance on destroy
    /// </summary>
    private void OnDestroy()
    {
        // singleton
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
