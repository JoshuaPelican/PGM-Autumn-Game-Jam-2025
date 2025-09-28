using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float rotationSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] float initialBoostSpeed;

    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] InteractUser interactUser;
    [SerializeField] PickupUser pickupUser;
    [SerializeField] ParticleSystem movePSystem;
    
    [Header("Audio")]
    [SerializeField] AudioPlayable jetpackAudio;
    [SerializeField] AudioPlayable jetpackInitialAudio;
    [SerializeField] AudioPlayable interactAudio; 


    float rotationDirection = 0;
    bool isMoving = false;
    float moveTimer = 0;

    public void OnRotate(InputAction.CallbackContext context)
    {
        rotationDirection = context.ReadValue<float>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        isMoving = context.ReadValueAsButton();

        if (isMoving && context.started)
            StartMoving();
        else if(!isMoving && context.canceled)
            StopMoving();
    }

    public void OnDetatch(InputAction.CallbackContext context)
    {
        if (context.started)
            Drop();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Interact();
            Pickup();
        }
        if (context.canceled)
        {
            StopInteract();
            StopPickup();
        }
    }

    private void FixedUpdate()
    {
        if (rotationDirection != 0)
            Rotate();
        if (isMoving)
            Move();
    }


    void Rotate()
    {
        float normDir = Mathf.Sign(-rotationDirection);
        rb.AddTorque(rotationSpeed * normDir);
    }

    void StartMoving()
    {
        movePSystem.Play();
        rb.AddForce(initialBoostSpeed * rb.transform.up, ForceMode2D.Impulse);
        moveTimer = 2.5f;
        AudioManager.Instance.PlayClip2D(jetpackInitialAudio, "jetpackInit");
    }

    void Move()
    {
        rb.AddForce(moveSpeed * rb.transform.up, ForceMode2D.Force);
        moveTimer += Time.deltaTime;
        if(moveTimer > 2.5)
        {
            moveTimer = 0;
            AudioManager.Instance.PlayClip2D(jetpackAudio, "jetpack");
        }
    }

    void Drop()
    {
        pickupUser.TryDrop();
    }

    void Interact()
    {
        interactUser.TryInteract();
        AudioManager.Instance.PlayClip2D(interactAudio, "interactPlayer");
    }

    void StopInteract()
    {
        interactUser.StopInteract();
    }

    void StopPickup()
    {
        pickupUser.StopPickup();
    }

    void Pickup()
    {
        pickupUser.TryPickup();
    }

    void StopMoving()
    {
        movePSystem.Stop();
        AudioManager.Instance.StopSound("jetpack", true);
    }
}
