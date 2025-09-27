using UnityEngine;
using UnityEngine.Events;

public class Pickupable : Interactable
{
    [Header("Audio")]
    [SerializeField] AudioPlayable pickupAudio;
    [SerializeField] AudioPlayable dropAudio;

    [Header("Pickup Events")]
    public UnityEvent OnPickup = new();
    public UnityEvent OnDrop = new();
    public UnityEvent OnPickupEnter = new();
    public UnityEvent OnPickupExit = new();


    public void Pickup()
    {
        OnPickup?.Invoke();
        if (pickupAudio.audioResource)
            AudioManager.Instance.PlayClip2D(pickupAudio, $"pickup_{name}");
    }

    public void Drop()
    {
        OnDrop?.Invoke();

        if (dropAudio.audioResource)
            AudioManager.Instance.PlayClip2D(dropAudio, $"drop_{name}");
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
