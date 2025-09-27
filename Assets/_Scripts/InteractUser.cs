using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractUser : MonoBehaviour
{
    //[SerializeField] ContactFilter2D interactFilter = new ContactFilter2D() { useTriggers = true, useLayerMask = true };
    //[SerializeField] Collider2D interactDetectionCollider;

    public bool IsInteracting { get; private set; } = false;


    Interactable closestInteractable;
    float closestInteractDistance = float.MaxValue;


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Interactable interactable))
            return;

        GetClosestInteract(interactable);

        interactable.InteractEnter();
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsInteracting)
            return;

        IsInteracting = false;

        if (!collision.TryGetComponent(out Interactable interactable))
            return;

        GetClosestInteract(interactable);

        closestInteractable.Interact();
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Interactable interactable))
            return;

        GetClosestInteract(interactable);

        interactable.InteractExit();
        if (interactable == closestInteractable)
        {
            closestInteractable = null;
            closestInteractDistance = float.MaxValue;
        }

    }

    void GetClosestInteract(Interactable interactable)
    {
        float dist = (interactable.transform.position - transform.position).sqrMagnitude;
        if (closestInteractDistance < dist)
            return;

        if(closestInteractable)
            closestInteractable.InteractDeselected();

        closestInteractable = interactable;
        closestInteractDistance = dist;

        closestInteractable.InteractSelected();
    }

    public void TryInteract()
    {
        IsInteracting = true;
    }
}
