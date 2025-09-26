using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interact Events")]
    public UnityEvent OnInteract = new UnityEvent();
    public UnityEvent OnInteractEnter = new UnityEvent();
    public UnityEvent OnInteractExit = new UnityEvent();

    public void Interact()
    {
        OnInteract?.Invoke();
    }

    public void InteractEnter()
    {
        OnInteractEnter?.Invoke();
    }

    public void InteractExit()
    {
        OnInteractExit?.Invoke();
    }
}
