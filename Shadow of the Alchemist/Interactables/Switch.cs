using FiveBabbittGames;
using UnityEngine;

public class Switch : Interactable
{
    [Header("Switch Settings")]
    public GameEvent switchEvent;

    [SerializeField] protected bool singleUse = false;

    public bool IsActive => isActive;
    protected bool isActive;

    public override void OnInteract(PlayerInteractor interactingPlayer)
    {
        if (!canInteract)
            return;

        base.OnInteract(interactingPlayer);

        isActive = !isActive;
        switchEvent.Raise(this, isActive);

        if (singleUse)
            canInteract = false;

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

    protected override void UpdateInteractText()
    {
        if (isActive && !singleUse)
            useText = "Switch Off";
        else if (!isActive)
            useText = "Switch On";

        if (isActive && singleUse)
            useText = "";

        base.UpdateInteractText();
    }
}
