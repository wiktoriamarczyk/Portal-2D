using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

/// <summary>
/// Class managing UI panels
/// </summary>
public class PanelManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static PanelManager Instance { get; private set; }
    /// <summary>
    /// List of all panels
    /// </summary>
    [SerializeField] List<GameObject> panels;
    /// <summary>
    /// Panel showed at the beginning of the game
    /// </summary>
    [SerializeField] GameObject startingPanel;
    /// <summary>
    /// Panel showed when player needs to pause the game
    /// </summary>
    [SerializeField] GameObject pausePanel;
    /// <summary>
    /// Text containing music volume
    /// </summary>
    [SerializeField] TextMeshProUGUI musicVolumeValue;
    /// <summary>
    /// Text containing info about music being on or off
    /// </summary>
    [SerializeField] TextMeshProUGUI musicSound;
    /// <summary>
    /// Player object
    /// </summary>
    [SerializeField] GameObject player;
    /// <summary>
    /// Currently showed panel
    /// </summary>
    GameObject currentPanel;
    /// <summary>
    /// Music volume value
    /// </summary>
    float musicVolume = 0.2f;
    /// <summary>
    ///  Awake is called when the script instance is being loaded - responsible for setting singleton instance,
    ///  setting music volume and hiding all panels
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

        SetMusicVolume(musicVolume);
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled - responsible for managing the behaviour of the player input
    /// </summary>
    void Update()
    {
        if (currentPanel == null && Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPanel(pausePanel);
            Camera.main.GetComponent<PostProcessLayer>().enabled = true;
            player.SetActive(false);
        }
        else if (currentPanel != null)
        {
            player.SetActive(false);
        }
        else
        {
            player.SetActive(true);
        }
    }
    /// <summary>
    /// Method showing panel
    /// </summary>
    /// <param name="panel">panel to be showed</param>
    public void ShowPanel(GameObject panel)
    {
        HideCurrentPanel();
        panel.SetActive(true);
        currentPanel = panel;
    }
    /// <summary>
    /// Method hiding current panel
    /// </summary>
    public void HideCurrentPanel()
    {
        if (currentPanel == null)
            return;
        currentPanel.SetActive(false);
        currentPanel = null;
    }
    /// <summary>
    /// Method responsible for loading level
    /// </summary>
    /// <param name="sceneName">level to be loaded</param>
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    /// <summary>
    /// Method responsible for loading next level
    /// </summary>
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    /// <summary>
    /// Method responsible for quitting the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
    /// <summary>
    /// Method responsible for enabling and disabling the music
    /// </summary>
    public void TurnOnOffMusic()
    {
        float currentVolume = 0;
        if (AudioListener.volume <= 0)
        {
            currentVolume = musicVolume;
            AudioListener.volume = currentVolume;
            musicSound.text = "SOUND: ON";
        }
        else
        {
            currentVolume = 0;
            AudioListener.volume = currentVolume;
            musicSound.text = "SOUND: OFF";
        }
        musicVolumeValue.text = (int)(currentVolume * 100) + "%";
    }
    /// <summary>
    /// Method responsible for setting music volume
    /// </summary>
    /// <param name="volume">music volume</param>
    void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp(volume, 0, 1.01f);
        AudioListener.volume = musicVolume;
        musicVolumeValue.text = (int)(musicVolume * 100) + "%";
    }
    /// <summary>
    /// Method responsible for turning music volume up
    /// </summary>
    public void TurnMusicUp()
    {
        SetMusicVolume(musicVolume + 0.1f);
    }
    /// <summary>
    /// Method responsible for turning music volume down
    /// </summary>
    public void TurnMusicDown()
    {
        SetMusicVolume(musicVolume - 0.1f);
    }
}
