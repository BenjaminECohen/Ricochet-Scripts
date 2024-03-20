using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTriggerVolume : MonoBehaviour
{
    public AudioSource playerAudioSource;
    [Tooltip("If True, re-entering the volume will not trigger additional sounds again")]
    public bool activateOnce = true;
    bool _lock = false;

    public List<SoundTrigger> soundTriggers = new List<SoundTrigger>();

    Coroutine currTrigger;

    PlayerController _playerController;

    [System.Serializable]
    public struct SoundTrigger
    {        
        public AudioClip audioClip;
        public float playDelaySec;
    }

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!_lock && other.tag == "Player")
        {
            if (activateOnce)
            {
                _lock = true;
            }
            foreach (var trigger in soundTriggers) 
            {
                StartCoroutine(playSoundTrigger(trigger));
            }
        }
    }

    IEnumerator playSoundTrigger(SoundTrigger trigger)
    {
        yield return new WaitForSeconds(trigger.playDelaySec);
        playerAudioSource.clip = trigger.audioClip;
        playerAudioSource.Play();

        if (trigger.audioClip.name.StartsWith("Disc"))
        {
            _playerController.Animator.SetBool("Talk", true);
        }
        currTrigger = StartCoroutine(currentSoundTrigger(trigger));


    }

    IEnumerator currentSoundTrigger(SoundTrigger trigger)
    {
        yield return new WaitForSeconds(trigger.audioClip.length);
        playerAudioSource.Stop();
        _playerController.Animator.SetBool("Talk", false);

    }

    public void StopCurrentAudio()
    {
        if (currTrigger != null)
            StopCoroutine(currTrigger);

        playerAudioSource.Stop();
        _playerController.Animator.SetBool("Talk", false);
    }
}
