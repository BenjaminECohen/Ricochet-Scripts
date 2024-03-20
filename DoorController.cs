using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] GameObject doorLeft;
    [SerializeField] GameObject doorRight;
    AudioSource audioSource;
    Vector3 doorLeftPos;
    Vector3 doorRightPos;
    private float doorLeftXPos;
    private float doorRightXPos;

    [Header("Audio")]
    [SerializeField] AudioClip doorOpenClip;
    [SerializeField] AudioClip doorCloseClip;

    [Header("Variables")]
    [SerializeField] float doorSlideDistance;
    [SerializeField] float doorSlideSeconds;
    bool isOpen = false;
    bool doneSliding = true;
    float slidingLerpVal = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        doorLeftPos = doorLeft.transform.localPosition;
        doorRightPos = doorRight.transform.localPosition;
        doorLeftXPos = doorLeftPos.x;
        doorRightXPos = doorRightPos.x;
    }

    private void Update()
    {
        //Door sliding open
        if(!doneSliding && isOpen)
        {
            slidingLerpVal += Time.deltaTime/doorSlideSeconds;
            slidingLerpVal = Mathf.Clamp01(slidingLerpVal);
            
            doorLeftPos.x = Mathf.Lerp(doorLeftXPos, doorLeftXPos - doorSlideDistance, slidingLerpVal);
            doorLeft.transform.localPosition = doorLeftPos;

            doorRightPos.x = Mathf.Lerp(doorRightXPos, doorRightXPos + doorSlideDistance, slidingLerpVal);
            doorRight.transform.localPosition = doorRightPos;

            if(slidingLerpVal == 1)
            {
                doneSliding = true;
            }
        }
        //Door sliding shut
        else if(!doneSliding && !isOpen)
        {
            slidingLerpVal -= Time.deltaTime/doorSlideSeconds;
            slidingLerpVal = Mathf.Clamp01(slidingLerpVal);

            doorLeftPos.x = Mathf.Lerp(doorLeftXPos, doorLeftXPos - doorSlideDistance, slidingLerpVal);
            doorLeft.transform.localPosition = doorLeftPos;

            doorRightPos.x = Mathf.Lerp(doorRightXPos, doorRightXPos + doorSlideDistance, slidingLerpVal);
            doorRight.transform.localPosition = doorRightPos;

            if (slidingLerpVal == 0)
            {
                doneSliding = true;
            }
        }
    }

    public void Open()
    {
        if(!isOpen)
        {
            audioSource.Stop();
            audioSource.clip = doorOpenClip; 
            audioSource.Play();
            doneSliding = false;
            isOpen = true;
        }
    }

    public void Close()
    {
        if(isOpen)
        {
            audioSource.Stop();
            audioSource.clip = doorCloseClip;
            audioSource.Play();
            doneSliding = false;
            isOpen = false;
        }
    }
}
