using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slot
{
    public List<Item> items = new List<Item>();
    public int maxItems = 1;
    public int position = -1;
    public bool equip;
}

public class Inventory : MonoBehaviour
{
    public List<Slot> slots = new List<Slot>();
    public InventoryUI inventoryUi;
    public int maxSlots = 40;
    public GameObject inventoryPage;
    public GameObject holderPage;
    public GameObject WorkBenchPage;
    public bool inventoryOpen;
    public bool inventoryStoper;
    public Transform SpawnDropItem;

    public delegate void OnItemChange();
    public OnItemChange onItemCallBack;

    #region Singleton
    public static Inventory Intance;
    private void Awake() => Intance = this;
    #endregion

    private menuScript menuScripts;
    private PlayerMovement playerMovementScript;
    private ThirdPersonCam thirdPersonScript;
    private Attack attackScript;

    private void Start()
    {
        attackScript = GetComponent<Attack>();
        playerMovementScript = PlayerMovement.instance;
        thirdPersonScript = ThirdPersonCam.Intance;
    }

    public Slot GetSlotAt(int index)
    {
        foreach (Slot slot in slots)
        {
            if (slot.position == index)
                return slot;
        }
        return null;
    }

    public bool Add(Item item)
    {
        foreach (Slot slot in slots)
        {
            if (item.name.Equals(slot.items[0].name) && slot.items.Count < slot.maxItems)
            {
                slot.items.Add(item);
                onItemCallBack?.Invoke();
                return true;
            }
        }

        if (slots.Count >= maxSlots)
        {
            Debug.Log("Not enough room");
            return false;
        }

        Slot newSlot = new Slot
        {
            maxItems = item.max,
            position = GetNextFreeSlot()
        };
        newSlot.items.Add(item);
        slots.Add(newSlot);

        onItemCallBack?.Invoke();
        return true;
    }

    private int GetNextFreeSlot()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            bool occupied = false;
            foreach (Slot slot in slots)
            {
                if (slot.position == i)
                {
                    occupied = true;
                    break;
                }
            }
            if (!occupied)
                return i;
        }
        return -1;
    }

    public void Swap(int oldId, int newId)
    {
        Slot oldSlot = null;
        Slot newSlot = null;

        foreach (Slot slot in slots)
        {
            if (slot.position == newId) newSlot = slot;
            if (slot.position == oldId) oldSlot = slot;
        }

        if (oldSlot != null) oldSlot.position = newId;
        if (newSlot != null) newSlot.position = oldId;

        onItemCallBack?.Invoke();
    }

    public void Remove(Item item, bool discarded)
    {
        foreach (Slot slot in slots)
        {
            if (!slot.items.Contains(item)) continue;

            if (slot.equip)
            {
                discarded = false;
            }
            else
            {
                slot.items.Remove(item);
                if (slot.items.Count == 0)
                {
                    inventoryUi.slotsUi[slot.position].RemoveText();
                    slots.Remove(slot);
                }
            }
            break;
        }

        if (discarded)
        {
            GameObject clone = Instantiate(item.itemBox);
            clone.transform.position = SpawnDropItem.position;
            Transform closestParent = FindClosestParentToPlayer();
            if (closestParent != null)
                clone.transform.SetParent(closestParent);
        }

        onItemCallBack?.Invoke();
    }

    public void RemoveEmptySlot(Slot slot)
    {
        if (slot.items.Count == 0)
        {
            inventoryUi.slotsUi[slot.position].RemoveText();
            slots.Remove(slot);
        }
    }

    private Transform FindClosestParentToPlayer()
    {
        Transform closestParent = null;
        float closestDistance = float.MaxValue;
        Vector3 playerPosition = PlayerMovement.instance.transform.position;

        foreach (TerGen terGen in FindObjectsOfType<TerGen>())
        {
            float distance = Vector3.Distance(playerPosition, terGen.parent.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestParent = terGen.parent;
            }
        }
        return closestParent;
    }

    private void Update()
    {
        if (inventoryStoper) return;

        if (menuScripts == null)
            menuScripts = menuScript.Intance;

        if (Input.GetKeyDown(KeyCode.Tab) && inventoryOpen && WorkBenchPage.activeSelf)
        {
            ClosePage();
            holderPage.SetActive(true);
            WorkBenchPage.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !inventoryOpen && !menuScripts.escape && !attackScript.onCraftPage)
        {
            Tutorial tutoScript = GetComponent<Tutorial>();
            if (tutoScript.currentStep == 1)
                tutoScript.currentStep++;
            OpenPage();
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && inventoryOpen)
        {
            ClosePage();
        }
    }

    public void ClosePage()
    {
        thirdPersonScript.StartCameraMovement();
        playerMovementScript.TurnMovementOff = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inventoryOpen = false;
        inventoryPage.SetActive(false);
    }

    public void OpenPage()
    {
        thirdPersonScript.StopCameraMovement();
        playerMovementScript.TurnMovementOff = true;
        inventoryOpen = true;
        inventoryPage.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
