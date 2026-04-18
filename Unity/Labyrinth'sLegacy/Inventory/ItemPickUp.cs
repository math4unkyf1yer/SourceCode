using UnityEngine;

public class ItemPickUp : Interactable
{
    public Item item;

    public override void Interact()
    {
        base.Interact();
        PickUp();
    }

    private void PickUp()
    {
        bool wasPickedUp = Inventory.Intance.Add(item);
        if (wasPickedUp)
            Destroy(gameObject);
    }
}
