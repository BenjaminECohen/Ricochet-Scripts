using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndOfSoundTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource audioSource;
    public AudioClip audioClip1;
    public GameObject speakerOne;
    public AudioClip audioClip2;
    public Light[] lights;



    bool _dialogueLock = false;
    bool _endActionLock = true;
    PlayerController _playerController;

    public UnityEvent discPickup;
    bool discAcquired = false;

    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!_dialogueLock) 
            {
                _dialogueLock = true;
                audioSource.clip = audioClip1;
                speakerOne.GetComponent<Animator>().SetBool("Talk", true);
                audioSource.Play();
                StartCoroutine(waitTillEndOfClip());
                
            }
            else if (!_endActionLock && !discAcquired)
            {
                _endActionLock = true;
                _playerController.ToggleDiscMesh(true);
                MakeLightsRed();
                discPickup.Invoke();
                StartCoroutine(discResponseWait());
                discAcquired = true;
            }
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!_endActionLock && !discAcquired)
            {
                _endActionLock = true;
                _playerController.ToggleDiscMesh(true);
                MakeLightsRed();
                discPickup.Invoke();
                StartCoroutine(discResponseWait());
                discAcquired = true;
            }
        }
    }


    IEnumerator waitTillEndOfClip()
    {
        yield return new WaitForSeconds(audioClip1.length);
        speakerOne.GetComponent<Animator>().SetBool("Talk", false);
        _endActionLock = false;
    }

    IEnumerator discResponseWait()
    {
        yield return new WaitForSeconds(2f);

        audioSource.clip = audioClip2;
        _playerController.Animator.SetBool("Talk", true);
        audioSource.Play();
        StartCoroutine(waitTillEndOfClip2());
    }
    IEnumerator waitTillEndOfClip2()
    {
        yield return new WaitForSeconds(audioClip2.length);
        _playerController.Animator.SetBool("Talk", false);
        _endActionLock = false;
    }

    void MakeLightsRed()
    {
        foreach (Light light in lights)
        {
            light.color = Color.red;
        }
    }
}
