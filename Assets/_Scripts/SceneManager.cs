using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    #region Singleton
    public static SceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    static ScenePayload scenePayload;
    public ScenePayload ScenePayload => scenePayload;

    public void LoadScene(string sceneName, ScenePayload payload)
    {
        scenePayload = payload;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int buildIndex, ScenePayload payload)
    {
        scenePayload = payload;
        UnityEngine.SceneManagement.SceneManager.LoadScene(buildIndex);
    }
}

[Serializable]
public class ScenePayload
{
    public List<Data> payload = new ();
}

[Serializable]
public struct Data
{
    public string Key;
    public UnityEngine.Object Value;
}