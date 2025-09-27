using UnityEngine;

public class PickupUser : InteractUser
{
    [SerializeField] Transform heldPickupContainer;
    [SerializeField] Pickupable heldPickup;

    public bool IsPickingUp { get; private set; } = false;
    public bool IsDropping { get; private set; } = false;

    public bool IsHolding => HeldPickup != null;
    public Pickupable HeldPickup => heldPickup;

    Pickupable closestPickup;
    float closestPickupDistance = float.MaxValue;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (!collision.TryGetComponent(out Pickupable pickup))
            return;

        GetClosestPickup(pickup);

        closestPickup.PickupEnter();
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);

        if (!IsPickingUp)
            return;

        IsPickingUp = false;

        if (!collision.TryGetComponent(out Pickupable pickup))
            return;

        GetClosestPickup(pickup);

        closestPickup.Pickup();
        Pickup(closestPickup);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (!collision.TryGetComponent(out Pickupable pickup))
            return;

        pickup.PickupExit();
        if (pickup == closestPickup)
        {
            closestPickup = null;
            closestPickupDistance = float.MaxValue;
        }

    }

    private void FixedUpdate()
    {
        if (!IsDropping)
            return;

        IsDropping = false;

        if (!IsHolding)
            return;

        heldPickup.Drop();
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

    void GetClosestPickup(Pickupable pickup)
    {
        float dist = (pickup.transform.position - transform.position).sqrMagnitude;
        if (closestPickupDistance < dist)
            return;

        closestPickup = pickup;
        closestPickupDistance = dist;
    }

    void Pickup(Pickupable pickup)
    {
        if(IsHolding)
        {
            // Drop the current item or just not let user do it
            Drop();
        }

        heldPickup = pickup;
        pickup.transform.SetParent(heldPickupContainer);
        pickup.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void Drop()
    {
        if(!IsHolding)
        {
            // Nothing to drop
            return;
        }

        heldPickup.transform.SetParent(null);
        heldPickup = null;
    }
}
