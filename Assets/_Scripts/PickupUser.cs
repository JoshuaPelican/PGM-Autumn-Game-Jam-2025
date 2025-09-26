using UnityEngine;

public class PickupUser : InteractUser
{
    [SerializeField] Transform heldItemContainer;

    public bool IsPickingUp { get; private set; } = false;
    public bool IsDropping { get; private set; } = false;

    public bool IsHolding => HeldPickup != null;
    public Pickupable HeldPickup { get; private set; }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (!collision.TryGetComponent(out Pickupable pickup))
            return;

        pickup.PickupEnter();
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);

        if (!IsPickingUp)
            return;

        IsPickingUp = false;

        if (!collision.TryGetComponent(out Pickupable pickup))
            return;

        pickup.Pickup();
        Pickup(pickup);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (!collision.TryGetComponent(out Pickupable pickup))
            return;

        pickup.PickupExit();
    }

    private void FixedUpdate()
    {
        if (!IsDropping)
            return;

        IsDropping = false;

        if (HeldPickup == null)
            return;

        Drop();
    }

    public void TryPickup()
    {
        IsPickingUp = true;
    }

    public void TryDrop()
    {
        IsDropping = true;
    }

    void Pickup(Pickupable pickup)
    {
        if(HeldPickup != null)
        {
            // Drop the current item or just not let user do it
            Drop();
        }

        pickup.transform.SetParent(heldItemContainer);
        pickup.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void Drop()
    {
        if(HeldPickup == null)
        {
            // Nothing to drop
            return;
        }

        HeldPickup.transform.SetParent(null);
        HeldPickup = null;
    }
}
