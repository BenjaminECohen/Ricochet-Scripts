using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillVolume : MonoBehaviour
{
    bool isLoading = false;
    public bool puzzleCompleted { get; set; } = false;
    Vector3 playerStart;

    private void Start()
    {
        playerStart = FindObjectOfType<PlayerController>().transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!puzzleCompleted && isLoading == false && other.tag == "Player")
        {
            string sceneName = gameObject.scene.name;
            isLoading = true;
            SceneManager.LoadScene(sceneName);
        }
        else if(puzzleCompleted && isLoading == false && other.tag == "Player")
        {
            other.transform.position = playerStart;
        }
    }
}
