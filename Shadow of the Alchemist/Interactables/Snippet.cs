using UnityEngine;

/// <summary>
/// Snippet1
/// </summary>
public class Snippet : Interactable
{
    [TextArea(15, 20)]
    [SerializeField] private string loreSnippet;

    private bool isViewing;
    public bool IsViewing => isViewing;

    public override void OnInteract(PlayerInteractor interactingPlayer)
    {
        base.OnInteract(interactingPlayer);

        if (!isViewing)
        {
            if (HUDController.Instance != null)
                HUDController.Instance.ViewSnippet(loreSnippet);
            isViewing = true;
        }
        else
        {
            if (HUDController.Instance != null)
                HUDController.Instance.CloseSnippet();
            isViewing = false;
        }
    }

    public override void OnFocus()
    {
        useText = "Read";

        base.OnFocus();
    }

    public override void OnLoseFocus()
    {
        if (HUDController.Instance != null)
            HUDController.Instance.CloseSnippet();

        base.OnLoseFocus();
    }

    protected override void UpdateInteractText()
    {


        base.UpdateInteractText();
    }
}
