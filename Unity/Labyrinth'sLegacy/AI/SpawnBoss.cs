using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnBoss : MonoBehaviour
{
    private GameObject target;
    public float radius = 6;
    private Text interactText;
    public GameObject boss;
    public string[] itemName;
    public int[] itemAmount;
    private bool bossHere;
    private Inventory playerInventory;
    private void Start()
    {
        target = GameObject.Find("Player");
        playerInventory = Inventory.Intance;
        interactText = GameObject.Find("TextInteract").GetComponent<Text>();
    }
    private void Update()
    {
        float distance = Vector3.Distance(target.transform.position, gameObject.transform.position);
        if(distance < radius && !bossHere)
        {
            interactText.text = "Pressed (E) to give 5 amber and 3 boar meat to summon".ToString();
            if (Input.GetKey(KeyCode.E) && HasRequiredItems())
            {
                bossHere = true;
                RemoveItemsFromInventory();
                //spawn boss
                Vector3 bossP = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
                GameObject bossClone = Instantiate(boss);
                bossClone.transform.position = bossP;
            }
        }
        else
        {
            interactText.text = "".ToString();
        }
    }

    private bool HasRequiredItems()
    {
        for (int i = 0; i < itemName.Length; i++)
        {
            string requiredItemName = itemName[i];
            int requiredAmount = itemAmount[i];

            int totalAmount = 0;

            foreach (Slot slot in playerInventory.slots)
            {
                foreach (Item item in slot.items)
                {
                    if (item.name.Equals(requiredItemName))
                    {
                        totalAmount += 1; // Count each item instance
                    }
                }
            }

            if (totalAmount < requiredAmount)
            {
                Debug.LogWarning($"Not enough {requiredItemName}. Required: {requiredAmount}, Found: {totalAmount}");
                return false;
            }
        }
        return true;
    }
    public void RemoveItemsFromInventory()
    {
        for (int i = 0; i < itemName.Length; i++)
        {
            string requiredItemName = itemName[i];
            int requiredAmount = itemAmount[i];
            int remainingAmountToRemove = requiredAmount;

            List<(Slot, int)> itemsToRemove = new List<(Slot, int)>();

            foreach (Slot slot in playerInventory.slots)
            {
                if (remainingAmountToRemove <= 0) break;

                int itemCountInSlot = 0;

                // Count the number of required items in the slot
                for (int j = 0; j < slot.items.Count; j++)
                {
                    Item item = slot.items[j];
                    if (item.name.Equals(requiredItemName))
                    {
                        itemCountInSlot++;
                    }
                }

                // If there are required items in the slot
                if (itemCountInSlot > 0)
                {
                    int itemsToRemoveFromSlot = Mathf.Min(itemCountInSlot, remainingAmountToRemove);
                    itemsToRemove.Add((slot, itemsToRemoveFromSlot));
                    remainingAmountToRemove -= itemsToRemoveFromSlot;
                }
            }

            // Remove marked items
            foreach (var (slot, count) in itemsToRemove)
            {
                for (int j = 0; j < count && slot.items.Count > 0; j++)
                {
                    slot.items.RemoveAt(slot.items.Count - 1);
                }

                // Check if the slot is empty and remove it if necessary
                if (slot.items.Count == 0)
                {
                    playerInventory.RemoveEmptySlot(slot);
                }
            }
        }

        if (playerInventory.onItemCallBack != null)
        {
            playerInventory.onItemCallBack.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
