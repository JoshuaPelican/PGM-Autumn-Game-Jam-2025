using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnInteract = new UnityEvent();

    public void Interact()
    {
        OnInteract?.Invoke();
    }
}
