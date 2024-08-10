using UnityEngine;

/// <summary>
/// Carriable
/// </summary>
public class Carriable : Interactable
{
    public bool isCarried;
    public Collider interactableCollider;

    public KeyData KeyData;

    public override void Start()
    {
        base.Start();
        interactableCollider = GetComponent<Collider>();
    }

    public override void OnInteract(PlayerInteractor interactingPlayer)
    {
        if (interactingPlayer.GetCarriable() != null) 
            return;

        base.OnInteract(interactingPlayer);

        if (isCarried)
        {
            Drop(interactingPlayer);
        }
        else
        {
            PickUp(interactingPlayer);
        }

        UpdateInteractText();
    }

    public override void OnFocus()
    {
        UpdateInteractText();

        base.OnFocus();

    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();

    }

    public void PickUp(PlayerInteractor interactingPlayer)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        isCarried = true;
        interactableCollider.enabled = false;

        transform.SetParent(player.carrySlot.carryTransform);
        transform.position = player.carrySlot.carryTransform.position;
        interactingPlayer.SetCarriable(this);
    }

    public void Drop(PlayerInteractor interactingPlayer)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        interactableCollider.enabled = true;
        isCarried = false;

        transform.SetParent(null);
        interactingPlayer.SetCarriable(null);
    }

    protected override void UpdateInteractText()
    {
        base.UpdateInteractText();
    }
}
