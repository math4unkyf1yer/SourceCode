using System.Collections;
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
    private menuScript menuScripts;
    private PlayerMovement playerMovementScript;
    private ThirdPersonCam thirdPersonScript;
    private Attack attackScript;
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
    private void Awake()
    {
        Intance = this;
    }
    #endregion
    private void Start()
    {
        attackScript = gameObject.GetComponent<Attack>();
        playerMovementScript = PlayerMovement.instance;
        thirdPersonScript = ThirdPersonCam.Intance;
    }

    public Slot getSlotAt(int index)
    {
        try
        {        
            foreach(Slot slot in slots)
            {
                if(slot.position == index)
                {
                    return slot;
                }
            }
            return null;
        }
        catch(System.Exception e)
        {
            return null;
        }
        
    }

    public bool Add(Item item)
    {
        bool bAdded = false;

        foreach (Slot slot in slots)
        {
            Debug.Log(slot.items[0].name);
            if (item.name.Equals(slot.items[0].name))
            {
                if (slot.items.Count < slot.maxItems)
                {
                    slot.items.Add(item);
                    bAdded = true;
                    break;
                }
            }
            
        }

        if (!bAdded)
        {
            if(slots.Count < maxSlots)
            {
                Slot s = new Slot();
                s.items.Add(item);
                s.maxItems = item.max;
                s.position = GetNextFreeSlot();
                slots.Add(s);
            }
            else
            {
                Debug.Log("Not enough room");
                return false;
            }
        }

        if(onItemCallBack != null)
            onItemCallBack.Invoke();
        return true;
    }
    int GetNextFreeSlot()
    {
        int i;
        bool bOccupied;
        //to do
        for(i = 0; i < maxSlots; i++)
        {
            bOccupied = false;
            foreach(Slot slot in slots)
            {
                if (slot.position == i)
                {
                    bOccupied = true;
                    break;
                }
            }
            if (bOccupied == false)
                return i;
        }
        return -1;
    }
    public void Swap(int slotoldId,int slotnewID)
    {
        Slot oldslot = null;
        Slot newSlot = null;
        foreach (Slot slot in slots)
        {
            if (slot.position.Equals(slotnewID))
            {
                newSlot = slot;
            }
            if (slot.position.Equals(slotoldId))
            {
                oldslot = slot;
            }
        }
        if(oldslot != null)
        {
            oldslot.position = slotnewID;
        }
        if(newSlot != null)
        {
            newSlot.position = slotoldId;
        }

        onItemCallBack.Invoke();

    }

    public void Remove(Item item,bool discarded)
    {
        bool checkDiscard = discarded;
        foreach (Slot slot in slots)
        {           
                // inventoryUi.slotsUi[slot.].EquipementOff();
           if (slot.items.Contains(item))
           {
                if (!slot.equip)
                {
                    discarded = checkDiscard;
                    slot.items.Remove(item);
                    if (slot.items.Count == 0)
                    {
                        inventoryUi.slotsUi[slot.position].RemoveText();
                        slots.Remove(slot);
                    }
                    break;
                }
                else
                {
                    discarded = false;
                }
                    
            }

        }

        if (discarded)
        {
            GameObject objectsclone = Instantiate(item.itemBox);
            objectsclone.transform.position = SpawnDropItem.position;
            // which scene  that it drops
            Transform closestParent = FindClosestParentToPlayer();
            if (closestParent != null)
            {
               objectsclone.transform.SetParent(closestParent);
            }
        }
        if (onItemCallBack != null)
            onItemCallBack.Invoke();
    }
    public void RemoveEmptySlot(Slot slot)//inportant in crafting
    {
        if (slot.items.Count == 0)
        {
            inventoryUi.slotsUi[slot.position].RemoveText(); // Update UI
            slots.Remove(slot);
        }
    }

    //finding which scene is closest 
    private Transform FindClosestParentToPlayer()
    {
        Transform closestParent = null;
        float closestDistance = float.MaxValue;
        Vector3 playerPosition = PlayerMovement.instance.transform.position;
        TerGen[] terGens = FindObjectsOfType<TerGen>();
        foreach (TerGen terGen in terGens)
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
        if(inventoryStoper == false)
        {
            if (menuScripts == null)
            {
                menuScripts = menuScript.Intance;
            }
            if (Input.GetKeyDown(KeyCode.Tab) && inventoryOpen == true && WorkBenchPage.activeSelf)
            {
                ClosePage();
                holderPage.SetActive(true);
                WorkBenchPage.SetActive(false);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Tab) && inventoryOpen == false && menuScripts.escape == false && !attackScript.onCraftPage)
            {
                Tutorial tutoScript = gameObject.GetComponent<Tutorial>();
                if(tutoScript.currentStep == 1)
                {
                    tutoScript.currentStep++;
                }
                OpenPage();
            }
            else if (Input.GetKeyDown(KeyCode.Tab) && inventoryOpen == true)
            {
                ClosePage();
            }
        }
    }
    public void ClosePage()
    {
        thirdPersonScript.StartCameraMovement();
        playerMovementScript.TurnMovementOff = false;
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor in the center of the screen
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
