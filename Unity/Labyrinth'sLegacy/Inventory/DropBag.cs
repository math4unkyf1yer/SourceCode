using System.Collections.Generic;
using UnityEngine;

public class DropBag : MonoBehaviour
{
    public List<Slot> storedSlots = new List<Slot>();
    public float radius = 2.5f;
    public bool waitForPlayerReady;

    private Transform player;

    public void GetPlayer()
    {
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < radius && waitForPlayerReady)
        {
            RestoreItemsToInventory();
            Destroy(gameObject);
        }
    }

    public void StoreItems(List<Slot> slots)
    {
        foreach (Slot slot in slots)
        {
            storedSlots.Add(new Slot
            {
                position = slot.position,
                maxItems = slot.maxItems,
                items = new List<Item>(slot.items)
            });
        }
    }

    public void RestoreItemsToInventory()
    {
        waitForPlayerReady = false;
        Inventory inventory = Inventory.Intance;

        foreach (Slot storedSlot in storedSlots)
        {
            foreach (Item item in storedSlot.items)
                inventory.Add(item);
        }

        storedSlots.Clear();
    }
}
