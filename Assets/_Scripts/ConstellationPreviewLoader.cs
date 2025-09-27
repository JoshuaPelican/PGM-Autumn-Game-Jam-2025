using UnityEngine;
using UnityEngine.UI;

public class ConstellationPreviewLoader : MonoBehaviour
{
    [SerializeField] Image previewImage;

    private void Start()
    {
        ConstellationData constellation = (ConstellationData)SceneManager.Instance.ScenePayload.payload.Find(x => x.Key == "ConstellationToLoad").Value;
        previewImage.sprite = constellation.Icon;
    }
}
