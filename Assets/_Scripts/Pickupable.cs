using UnityEngine;
using UnityEngine.Events;

public class Pickupable : Interactable
{
    [Header("Pickup Events")]
    public UnityEvent OnPickup = new();
    public UnityEvent OnPickupEnter = new UnityEvent();
    public UnityEvent OnPickupExit = new UnityEvent();

    public void Pickup()
    {
        OnPickup?.Invoke();
    }

    public void PickupEnter()
    {
        OnPickupEnter?.Invoke();
    }

    public void PickupExit()
    {
        OnPickupExit?.Invoke();
    }
}
