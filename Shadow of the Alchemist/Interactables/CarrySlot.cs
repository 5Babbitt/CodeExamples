using UnityEngine;

/// <summary>
/// CarrySlot
/// </summary>
public class CarrySlot : MonoBehaviour
{
    public PlayerInteractor player;
    public Carriable carryItem;

    public Transform carryTransform;
    public bool IsEmpty => (carryItem == null);

    private void Start()
    {
        player = transform.root.GetComponent<PlayerInteractor>();
    }


    public void Clear()
    {
        carryItem = null;
    }

    public void Drop()
    {
        if (carryItem == null)
            return;

        carryItem.Drop(player);
    }
}
