using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,IDropHandler
{
    Item item;
    public KeyCode input;
    public Image icon;
    public Text amountText;
    public int amount;
    public GameObject equipementOn;
    Equipement equipment;
    Weapons weapon;
    public bool dragItem;
    public int slotNumber;
    public bool isEquippedItem;
    public Slot linkedSlot;
    public DragDrop dragDropScript;
    public void SetInventoryInfo(Item newItem, int nbItems,Slot slot)
    {
        if(newItem != null)
        {
           linkedSlot = slot;
            if (linkedSlot.equip)
                isEquippedItem = true;
            item = newItem;
            if (item is Equipement)
                equipment = (Equipement)item;
            if (item is Weapons)
                weapon = (Weapons)item;

            icon.sprite = item.icon;
            icon.enabled = true;
            amount = nbItems;
            TextItem(newItem);
        }
        else
        {
            ClearSlot();
        }

    }

    public void TextItem(Item item)
    {
        if(amount == 0)
        {
            RemoveText();
        }
        else
        {
            amountText.text = amount + "/" + item.max;
        }
    }
    public void RemoveText()
    {
        amountText.text = "";
    }

    public void ClearSlot()
    {
        isEquippedItem = false;
        EquipementOff();
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        RemoveText();
    }

    public void OnRemoveButton()
    {
        if(item != null && !isEquippedItem)
        {
            TextItem(item);
            if (equipementOn.gameObject.activeSelf && item is Equipement)
            {
                isEquippedItem = false;
                EquipementManager.instance.Unequip((int)equipment.equipSlot, equipment);
                NoEquipementInSlot();
            }
            if(equipementOn.gameObject.activeSelf && item is Weapons)
            {
                isEquippedItem = false;
                WeaponManager.instance.Unequip(weapon);
                NoEquipementInSlot();
            }
            Inventory.Intance.Remove(item, true);
        }
    }
    public void NoEquipementInSlot()
    {
        equipment = null;
        weapon = null;
    }

    public void EquipementOn()
    {
        equipementOn.gameObject.SetActive(true);
        if (linkedSlot != null)
        {
            linkedSlot.equip = true; // Mark the Slot as not equipped
            dragDropScript.enabled = false;
        }
    }
    public void EquipementOff()
    {
      equipementOn.gameObject.SetActive(false);
        if (linkedSlot != null)// && //dragDropScript != null)
        {
            linkedSlot.equip = false; // Mark the Slot as not equipped
            dragDropScript.enabled = true;
        }
    }

    private void Update()
    {
        if (isEquippedItem)
        {
            EquipementOn();
        }
        else
        {
            EquipementOff();
        }

        if (Input.GetKeyDown(input) && !dragItem)
        {
            Use();
        }
        /* if(equip != null)
         {
             if (equip.equiped)
             {
                 EquipementOn();
             }
             else
             {
                 EquipementOff();
             }
         }else if (equipementOn.gameObject.activeSelf)
         {
             EquipementOff();
         }
         if(item == null && equip != null)
         {
             equip = null;
         }
         if (Input.GetKeyDown(input) && dragItem == false)
         {
        //     Debug.Log("pressed");
             Use();
         }*/
    }
    public void OnDrop(PointerEventData eventData)
    {
        DragDrop dragdrop = eventData.pointerDrag.GetComponent<DragDrop>();
        dragdrop.transform.SetParent(dragdrop.staticParent.transform);
        InventorySlot slotMoving = dragdrop.GetComponentInParent<InventorySlot>();
        Inventory.Intance.Swap(slotMoving.slotNumber, slotNumber);
    }

    public void Use()
    {
        if(item != null && dragItem == false)
        {
            item.Use(this);
        }
      //  else { Debug.Log("No Item"); }
    }
}
