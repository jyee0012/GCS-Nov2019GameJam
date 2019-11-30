using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    GameObject LoadingScreen = null;
    [SerializeField]
    AudioSource bgMusic = null;
    [SerializeField]
    float fadeTime = 0.5f;

    public void LoadScene(string sceneName)
    {
        if (!sceneName.Equals(""))
        {
            if (LoadingScreen != null)
                LoadingScreen.SetActive(true);

            //Fade the Audio
            StartCoroutine(FadeAudio());

            SceneManager.LoadSceneAsync(sceneName);
        }
    }

    public void TogglePanel(GameObject panel)
    {
        if (panel.activeSelf)
            panel.SetActive(false);
        else
            panel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator FadeAudio()
    {
        float endFade = Time.time + fadeTime;

        while (bgMusic.volume > 0)
        {
            float lerpAmount = (endFade - Time.time) / fadeTime;
            bgMusic.volume = lerpAmount;

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}
