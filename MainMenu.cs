using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] LevelTeleporter levelTeleporter;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        levelTeleporter.TransitionScene();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
