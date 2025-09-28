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

        GetClosestInteractable(interactable);

        interactable.InteractEnter();
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactable interactable))
        {
            GetClosestInteractable(interactable);

            if (IsInteracting)
            {
                closestInteractable.Interact();
            }
        }

        IsInteracting = false;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Interactable interactable))
            return;

        interactable.InteractExit();
        if (interactable == closestInteractable)
        {
            closestInteractable = null;
            closestInteractDistance = float.MaxValue;
        }

    }

    void GetClosestInteractable(Interactable interactable)
    {
        float dist = (interactable.transform.position - transform.position).sqrMagnitude;
        if (closestInteractDistance < dist)
            return;

        if(closestInteractable && interactable != closestInteractable)
            closestInteractable.InteractDeselected();

        closestInteractable = interactable;
        closestInteractDistance = dist;

        closestInteractable.InteractSelected();
    }

    public void TryInteract()
    {
        IsInteracting = true;
    }

    public void StopInteract()
    {
        IsInteracting = false;
    }
}
