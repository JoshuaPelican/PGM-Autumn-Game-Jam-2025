using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractUser : MonoBehaviour
{
    //[SerializeField] ContactFilter2D interactFilter = new ContactFilter2D() { useTriggers = true, useLayerMask = true };
    //[SerializeField] Collider2D interactDetectionCollider;

    public bool IsInteracting { get; private set; } = false;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Interactable interactable))
            return;

        interactable.InteractEnter();
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsInteracting)
            return;

        IsInteracting = false;

        if (!collision.TryGetComponent(out Interactable interactable))
            return;

        interactable.Interact();
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Interactable interactable))
            return;

        interactable.InteractExit();
    }


    public void TryInteract()
    {
        IsInteracting = true;
    }
}
