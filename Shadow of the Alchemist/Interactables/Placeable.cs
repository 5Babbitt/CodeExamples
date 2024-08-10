using FiveBabbittGames;
using UnityEngine;

/// <summary>
/// Placeable
/// </summary>
public class Placeable : Interactable
{
    [Header("Placeable Settings")]
    [SerializeField] protected GameEvent onKeyItemPlaced;
    [SerializeField] protected Carriable carrySlot;
    [SerializeField] protected Vector3 dimensions = Vector3.one;
    private BoxCollider placementCollider;
    public EnemyEventBroadcaster enemyEventBroadcaster;
    public KeyData KeyData;

    public override void Start()
    {
        base.Start();
        placementCollider = GetComponent<BoxCollider>();
        if(enemyEventBroadcaster == null)
        {
            this.gameObject.AddComponent<EnemyEventBroadcaster>();
            enemyEventBroadcaster = GetComponent<EnemyEventBroadcaster>();
            enemyEventBroadcaster.type = EnemyEventBroadcaster.ObjectType.Puzzle;
        }

    }

    public override void OnInteract(PlayerInteractor interactingPlayer)
    {
        base.OnInteract(interactingPlayer);

        var playerCarry = interactingPlayer.GetCarriable();

        if (carrySlot != null || playerCarry == null) 
            return;

        if (!KeyDataMatch(playerCarry))
        {
            enemyEventBroadcaster.InvokeSusOccurrence();
            return;
        }


        carrySlot = interactingPlayer.GetCarriable();
        carrySlot.transform.SetParent(transform);
        carrySlot.transform.position = transform.position;
        onKeyItemPlaced.Raise();
        interactingPlayer.carrySlot.Clear();
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

    protected bool KeyDataMatch(Carriable carriable)
    {
        if (KeyData == carriable.KeyData)
        {
            Debug.Log("KeyData Matches");
            return true;
        }

        return false;
    }

    protected virtual void OnValidate()
    {
        placementCollider = GetComponent<BoxCollider>();
        placementCollider.size = dimensions;
    }

    protected override void UpdateInteractText()
    {
        if (carrySlot != null)
        {
            useText = "";
            base.UpdateInteractText();
            return;
        }
        
        Carriable carriable = PlayerController.Instance.Interactor.GetCarriable();

        if (PlayerController.Instance.Interactor.GetCarriable() == null || !KeyDataMatch(carriable))
        {
            useText = "Wrong Key Item";
        }
        else
        {
            useText = "Place Item";
        }

        base.UpdateInteractText();
    }
}
