using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    GameObject LoadingScreen = null;

    public void LoadScene(string sceneName)
    {
        if (!sceneName.Equals(""))
        {
            if (LoadingScreen != null)
                LoadingScreen.SetActive(true);
            SceneManager.LoadSceneAsync(sceneName);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
