using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }
    [SerializeField] List<GameObject> panels;
    [SerializeField] GameObject startingPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] TextMeshProUGUI musicVolumeValue;
    [SerializeField] TextMeshProUGUI musicSound;
    [SerializeField] GameObject player;
    GameObject currentPanel;
    float musicVolume = 0.2f;

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

    public void ShowPanel(GameObject panel)
    {
        HideCurrentPanel();
        panel.SetActive(true);
        currentPanel = panel;
    }

    public void HideCurrentPanel()
    {
        if (currentPanel == null)
            return;
        currentPanel.SetActive(false);
        currentPanel = null;
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

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

    void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp(volume, 0, 1.01f);
        AudioListener.volume = musicVolume;
        musicVolumeValue.text = (int)(musicVolume * 100) + "%";
    }

    public void TurnMusicUp()
    {
        SetMusicVolume(musicVolume + 0.1f);
    }

    public void TurnMusicDown()
    {
        SetMusicVolume(musicVolume - 0.1f);
    }
}
