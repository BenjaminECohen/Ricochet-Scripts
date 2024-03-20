using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTeleporter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image blackScreen;
    [SerializeField] TMP_Text tutorialText;

    [Header("Variables")]
    [SerializeField] string sceneToLoad;
    [SerializeField] float fadeTime = 1f;
    bool fadingOut = false;
    bool fadingIn = true;
    Color proxyColor;
    Color tutProxyColor;

    private void Start()
    {
        proxyColor = Color.black;
        proxyColor.a = 1f;
        tutProxyColor = Color.white;
        tutProxyColor.a = 0f;
    }

    private void Update()
    {
        if(fadingIn)
        {
            if(blackScreen.color.a <= 0f)
            {
                //blackScreen.transform.parent.gameObject.SetActive(false);
                fadingIn = false;
            }
            else
            {
                proxyColor.a -= Time.deltaTime/fadeTime;
                proxyColor.a = Mathf.Clamp01(proxyColor.a);
                blackScreen.color = proxyColor;
            }
        }
        else if(fadingOut)
        {
            if (blackScreen.color.a >= 1f)
            {
                fadingOut = false;
            }
            else
            {
                proxyColor.a += Time.deltaTime / fadeTime;
                proxyColor.a = Mathf.Clamp01(proxyColor.a);
                blackScreen.color = proxyColor;

                if(tutorialText != null)
                {
                    tutProxyColor.a -= Time.deltaTime / fadeTime;
                    tutProxyColor.a = Mathf.Clamp01(tutProxyColor.a);
                    tutorialText.color = tutProxyColor;
                }
            }
        }

        if(tutorialText != null && blackScreen.color.a <= 0f && tutorialText.color.a < 1f)
        {
            tutProxyColor.a += Time.deltaTime / fadeTime;
            tutProxyColor.a = Mathf.Clamp01(tutProxyColor.a);
            tutorialText.color = tutProxyColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(SceneTransition());
        }
    }

    public void TransitionScene()
    {
        StartCoroutine(SceneTransition());
    }

    IEnumerator SceneTransition()
    {
        //blackScreen.transform.parent.gameObject.SetActive(true);
        fadingOut = true;

        yield return new WaitUntil(() => !fadingOut);

        SceneManager.LoadScene(sceneToLoad);

        yield return null;
    }

    IEnumerator SceneTransition(int index)
    {
        //blackScreen.transform.parent.gameObject.SetActive(true);
        fadingOut = true;

        yield return new WaitUntil(() => !fadingOut);

        SceneManager.LoadScene(index);

        yield return null;
    }

    public void RemoteSceneTransition(int index)
    {
        StartCoroutine(SceneTransition(index));
    }

}
