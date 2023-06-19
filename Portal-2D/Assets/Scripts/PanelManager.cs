using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;
    [SerializeField] GameObject startingPanel;
    [SerializeField] GameObject pausePanel;
   // [SerializeField] GameObject game;
    GameObject currentPanel;

    void Awake()
    {
        //game.SetActive(false);
;       foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        ShowPanel(startingPanel);
    }

    void Update()
    {
        if (currentPanel == null && Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPanel(pausePanel);
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

}
