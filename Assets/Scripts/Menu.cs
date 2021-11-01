using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private string _switchSceneName;

    public void Quit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene(_switchSceneName);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(_switchSceneName));
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(_switchSceneName);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(_switchSceneName));
    }

    public void Resume()
    {
        this.gameObject.SetActive(false);
    }
}
