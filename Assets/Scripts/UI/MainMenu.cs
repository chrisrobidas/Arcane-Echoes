using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject m_mainMenu;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {

    }

    private void Show(bool show)
    {
        m_mainMenu.SetActive(show);
    }

    public void OnPlayButton()
    {
        GameManager.StartGame();
    }

    public void OnQuitButton()
    {
        GameManager.ExitGame();
    }
}
