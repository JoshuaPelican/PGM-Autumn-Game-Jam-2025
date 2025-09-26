using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] ScenePayload scenePayload;

    public void LoadScene()
    {
        SceneManager.Instance.LoadScene(sceneName, scenePayload);
    }
}


public class ConstellationSceneData : ScenePayload
{
    public readonly ConstellationData ConstellationToLoad;

    public ConstellationSceneData(ConstellationData constellationToLoad)
    {
        ConstellationToLoad = constellationToLoad;
    }
}