using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(Instance != null)
        {
            Destroy(Instance);
            Instance = this;
        }
        else
        {
            Instance = this;
        }
    }
    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
    public void LoadScene(int lvlIndex)
    {
        SceneManager.LoadScene(lvlIndex);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitButtonPressed()
    {
        Application.Quit();
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(GetCurrentSceneIndex());
    }
}
