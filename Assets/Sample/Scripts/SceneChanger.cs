using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class SceneChanger : MonoBehaviour
{
    public Image fadeOverlay; // Assign a UI Image in the Inspector
    public float fadeDuration = 1f; // Duration of fade effect

    private void Start()
    {
        if (fadeOverlay != null)
        {
            // Start with the overlay fully visible and fade out
            fadeOverlay.color = new Color(0, 0, 0, 1);
            StartCoroutine(FadeOut());
        }
    }

    public void ChangeScene(string phoneme)
    {
        int sceneNo = 2;
        StaticData.selectedPhoneme = phoneme;
        if (sceneNo >= 0 && sceneNo < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(FadeAndSwitchScene(sceneNo));
        }
        else
        {
            Debug.LogError("Scene name is invalid!");
        }
    }

    public void ChangeSceneNo(int sceneNo)
    {
        if (sceneNo >= 0 && sceneNo < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(FadeAndSwitchScene(sceneNo));
        }
        else
        {
            Debug.LogError("Scene name is invalid!");
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color startColor = fadeOverlay.color;
        Color endColor = new Color(0, 0, 0, 0);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeOverlay.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeOverlay.color = endColor; // Ensure it's fully transparent
    }

    private IEnumerator FadeAndSwitchScene(int sceneNo)
    {
        float elapsedTime = 0f;
        Color startColor = fadeOverlay.color;
        Color endColor = new Color(0, 0, 0, 1);

        // Fade to black
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeOverlay.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeOverlay.color = endColor; // Ensure it's fully black

        // Load the new scene
        SceneManager.LoadScene(sceneNo);
    }
}
