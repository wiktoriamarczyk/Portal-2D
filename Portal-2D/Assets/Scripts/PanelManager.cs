using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PanelManager : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;
    [SerializeField] GameObject startingPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] TextMeshProUGUI musicVolumeValue;
    [SerializeField] TextMeshProUGUI musicSound;
    // [SerializeField] GameObject game;
    GameObject currentPanel;
    float musicVolume = 0.2f;

    void Awake()
    {
        SetMusicVolume(musicVolume);
        //game.SetActive(false);
        ; foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        //ShowPanel(startingPanel);
    }

    void Update()
    {
        if (currentPanel == null && Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPanel(pausePanel);
            Camera.main.GetComponent<PostProcessLayer>().enabled = true;
            //game.SetActive(false);
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

    public void RestartGame()
    {
        //game = new GameObject();
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
