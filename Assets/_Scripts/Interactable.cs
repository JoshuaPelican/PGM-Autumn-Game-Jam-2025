using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Audio")]
    public AudioPlayable interactAudio;

    [Header("Interact Events")]
    public UnityEvent OnInteract = new UnityEvent();
    public UnityEvent OnInteractEnter = new UnityEvent();
    public UnityEvent OnInteractExit = new UnityEvent();
    public UnityEvent OnInteractSelected = new UnityEvent();
    public UnityEvent OnInteractDeselected = new UnityEvent();

    [Header("References")]
    [SerializeField] protected Collider2D trigger;

    public void Interact()
    {
        OnInteract?.Invoke();
        if (interactAudio.audioResource)
            AudioManager.Instance.PlayClip2D(interactAudio, $"interact_{name}");
    }

    public void InteractEnter()
    {
        OnInteractEnter?.Invoke();
    }

    public void InteractExit()
    {
        OnInteractExit?.Invoke();
    }

    public void InteractSelected()
    {
        OnInteractSelected?.Invoke();
    }

    public void InteractDeselected()
    {
        OnInteractDeselected?.Invoke();
    }
}
