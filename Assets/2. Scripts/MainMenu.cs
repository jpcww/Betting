using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject connectMenu;

    private void Awake()
    {
        mainMenu.SetActive(true);
        connectMenu.SetActive(false);
    }

    public void OpenConnectMenu()
    {
        mainMenu.SetActive(false);
        connectMenu.SetActive(true);
    }

    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
