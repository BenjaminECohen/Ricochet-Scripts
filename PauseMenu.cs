using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject _container;
    float _timeScale = 1f;
    PlayerController _playerController;
    LevelTeleporter _sceneTransitioner;


    // Start is called before the first frame update
    void Start()
    {
        _timeScale = Time.timeScale;
        _container.SetActive(false);
        _playerController = FindObjectOfType<PlayerController>();
        _sceneTransitioner = FindObjectOfType<LevelTeleporter>();
        

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_container.activeInHierarchy)
            {
                TogglePauseMenu();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;               
            }
            else
            {
                TogglePauseMenu();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }    
            
        }
    }

    public void ReloadScene()
    {
        Time.timeScale = _timeScale;
        if (_sceneTransitioner != null)
        {
            _sceneTransitioner.RemoteSceneTransition(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        

    }

    public void LoadScene(int index)
    {
        Time.timeScale = _timeScale;
        if (_sceneTransitioner != null)
        {
            _sceneTransitioner.RemoteSceneTransition(index);
        }
        else
        {
            SceneManager.LoadScene(index);
        }
            
        
    }



    public void TogglePauseMenu()
    {
        _container.SetActive(!_container.activeInHierarchy);
        if (_container.activeInHierarchy)
        {
            Time.timeScale = 0;
            _playerController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = _timeScale;
            _playerController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
    }

}
