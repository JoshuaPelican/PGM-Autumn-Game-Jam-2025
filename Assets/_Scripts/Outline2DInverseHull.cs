using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class Outline2DInverseHull : MonoBehaviour
{
    [SerializeField] bool isActive = true;
    [SerializeField] Color outlineColor = Color.gray;
    [SerializeField] float outlineWidth = 0.1f;

    [SerializeField] SpriteRenderer outlineRenderer;

    private void Awake()
    {
        if (outlineRenderer)
            return;

        SetupOutline();
    }

    private void OnValidate()
    {
        SetActive(isActive);
        outlineRenderer.color = outlineColor;
        outlineRenderer.transform.localScale = Vector3.one * (1 + outlineRenderer.sprite.pixelsPerUnit / 100f * outlineWidth);
    }

    void SetupOutline()
    {
        SpriteRenderer thisRenderer = GetComponent<SpriteRenderer>();
        outlineRenderer = new GameObject($"{gameObject.name} Outline").AddComponent<SpriteRenderer>();
        outlineRenderer.transform.SetParent(transform);
        outlineRenderer.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        outlineRenderer.sprite = thisRenderer.sprite;
        outlineRenderer.color = outlineColor;
        outlineRenderer.transform.localScale = Vector3.one * (1 + outlineRenderer.sprite.pixelsPerUnit / 100f * outlineWidth);
        outlineRenderer.sortingOrder = thisRenderer.sortingOrder - 1;
        outlineRenderer.sharedMaterials = thisRenderer.sharedMaterials;
    }

    public void SetActive(bool active)
    {
        outlineRenderer.gameObject.SetActive(active);
    }
}
