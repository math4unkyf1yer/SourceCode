using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftSlot : MonoBehaviour
{
    public string Information;
    public Text descriptionText;
    public TextMeshProUGUI notificationText;
    public GameObject craftConfirmButton;

    //for craft without Hammer
    public GameObject parent;
    public GameObject itemDrop;
    public Transform playerPosition;
    private Inventory playerInventory; // Reference to the player's inventory

    [Header("UI")]
    public string[] itemName;
    public int[] itemAmount;
    
    public Text[] text;
    public Sprite[] sprite;
    public Image[] image;
    //for Workbench
   
    [Header("CraftWithHammer")]
    public bool craftWithHammer;
    public GameObject craftHammerObject;
    private HammerCraft hammercraft;
    private Attack attack;

    [Header("Audio")]
    private AudioSource craftingSAudio;
    private void Start()
    {
        craftingSAudio = GameObject.Find("Playerui").GetComponent<AudioSource>();
        playerInventory = Inventory.Intance; // Assuming Inventory is a singleton
        hammercraft = HammerCraft.Intance;
        attack = Attack.Intance;
    }

    public void OnClickHammerConfirm()
    {
        //confirm take away the craft page 
        if (HasRequiredItems())
        {
           
            hammercraft.ClosePage();
           GameObject hammerInstance = Instantiate(craftHammerObject);

            // Make the hammer follow the mouse position
            attack.TimeToCraft(hammerInstance,this);
        }
        else
        {
            Debug.Log("Not enought Item");
        }
    }

    IEnumerator WaitText()
    {
        yield return new WaitForSeconds(2);
        notificationText.text = "".ToString();
    }
    public void OnClickOnce()
    {
        Erased();
        // Show info
        descriptionText.text = Information.ToString();
        craftConfirmButton.SetActive(true);

        for(int i = 0; i < itemName.Length; i++)
        {
            text[i].text = itemName[i] + itemAmount[i].ToString();
            image[i].sprite = sprite[i];
        }
    }
    public void WorkbenchClickOnce()
    {
        Erased();
        descriptionText.text = Information.ToString();
        craftConfirmButton.SetActive(true);
        for (int i = 0; i < itemName.Length; i++)
        {
            text[i].text = itemName[i] + itemAmount[i].ToString();
            image[i].sprite = sprite[i];
        }
    }

    void Erased()
    {
        // Deactivate the craft confirm button for all CraftSlot instances
        CraftSlot[] craftSlots = parent.GetComponentsInChildren<CraftSlot>();
        foreach (CraftSlot craftSlot in craftSlots)
        {
            if (craftSlot.craftConfirmButton != null)
            {
                craftSlot.craftConfirmButton.SetActive(false);
            }
        }

        // Clear the description text
        if (descriptionText != null)
        {
            descriptionText.text = "";
        }
    }

    public void OnClickConfirm() // inventory
    {
        // Crafting logic
        if (HasRequiredItems())
        {
            craftingSAudio.Play();
            // Remove items from inventory
            RemoveItemsFromInventory();
            Tutorial tutoScript = GameObject.Find("Player").GetComponent<Tutorial>();
            if (tutoScript.currentStep == 4 && itemDrop.name == "HammerPickUp")
            {
                tutoScript.currentStep++;
            }
            if (tutoScript.currentStep == 5 && itemDrop.name == "ClubPickUp")
            {
                tutoScript.currentStep++;
            }
            GameObject cloneItem = Instantiate(itemDrop);
            cloneItem.transform.position = playerPosition.position;
            Debug.Log("Item crafted successfully!");
            craftConfirmButton.SetActive(false);
            
        }
        else
        {
            Debug.Log("Not enough items to craft.");
        }
    }

    public bool HasRequiredItems()
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

}
