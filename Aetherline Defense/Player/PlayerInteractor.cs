using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : PlayerSystem
{
    [SerializeField] private float interactionRadius;
    [SerializeField] private Transform interactionOrigin;
    [SerializeField] private string interactionTag;

    [SerializeField] private List<Interactable> nearbyInteractables = new();
    [SerializeField] private Interactable currentInteractable;

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void SubscribeEvents()
    {
        player.input.OnInteractInput += OnInteract;
    }

    protected override void UnsubscribeEvents()
    {
        player.input.OnInteractInput -= OnInteract;

    }

    private void OnInteract()
    {
        Interactable nearestInteractable = null;
        float minDist = float.MaxValue;

        foreach (var current in nearbyInteractables)
        {
            if (!current.CanInteract())
                continue;

            if (nearestInteractable == null)
            {
                nearestInteractable = current;
                continue;
            }

            float distance = Vector3.Distance(transform.position, current.transform.position);

            if (distance < minDist)
            {
                minDist = distance;
                nearestInteractable = current;
            }
        }

        if (nearestInteractable == null)
            return;

        Debug.Log($"Interacting with {nearestInteractable.name}");
        currentInteractable = nearestInteractable;
        currentInteractable.Interact(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == interactionTag)
        {
            nearbyInteractables.Add(other.GetComponent<Interactable>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == interactionTag)
        {
            nearbyInteractables.Remove(other.GetComponent<Interactable>());
            currentInteractable = null;
        }
    }

    private void OnValidate()
    {
        var trigger = interactionOrigin.GetComponent<SphereCollider>();

        trigger.radius = interactionRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(interactionOrigin.position, interactionRadius);
    }
}
