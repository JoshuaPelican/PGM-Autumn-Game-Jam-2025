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
    [SerializeField] Rope tether;
    [SerializeField] ParticleSystem movePSystem;
    
    [Header("Audio")]
    [SerializeField] AudioPlayable jetpackAudio;
    [SerializeField] AudioPlayable jetpackInitialAudio;
    [SerializeField] AudioPlayable detachAudio;
    [SerializeField] AudioPlayable reattachAudio;
    [SerializeField] AudioPlayable interactAudio; 


    float rotationDirection = 0;
    bool isMoving = false;
    float moveTimer = 0;
    bool isInteracting = false;

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
        if (tether != null && context.started)
            Detatch();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        isInteracting = context.started;

        if (isInteracting)
            Interact();
    }

    private void FixedUpdate()
    {
        if (rotationDirection != 0)
            Rotate();
        if (isMoving)
            Move();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(isInteracting && collision.TryGetComponent(out Interactable interactable))
        {
            interactable.Interact();
        }
    }

    public void AttachTether(Rope rope)
    {
        if (tether != null)
            return;

        tether = rope;
        tether.transform.SetParent(transform);
        tether.transform.localPosition = Vector3.zero;
        AudioManager.Instance.PlayClip2D(reattachAudio, "reattach");
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

    void Detatch()
    {
        tether.transform.SetParent(null);
        tether = null;
        AudioManager.Instance.PlayClip2D(detachAudio, "detatch");
    }

    void Interact()
    {
        // TODO: Physics2D Overlap Collider for interactable trigger colliders
        AudioManager.Instance.PlayClip2D(interactAudio, "interactPlayer");
    }

    void StopMoving()
    {
        movePSystem.Stop();
        AudioManager.Instance.StopSound("jetpack", true);
    }
}
