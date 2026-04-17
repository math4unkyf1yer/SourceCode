using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equip", menuName = "Inventory/Equip")]
public class Equipement : Item
{
    public int pierceRes;
    public int slashRes;
    public int BleudRes;
    public int weightModifyer;
    public GameObject currentArmor;
    public GameObject leftArmArmor;  // For left arm piece
    public GameObject rightArmArmor; // For right arm piece
    public GameObject leftLegArmor;
    public GameObject rightLegArmor;

    public EquipementSlot equipSlot;
    public bool equiped;

    public override void Use(InventorySlot slot)
    {
        base.Use(slot);
        if (!slot.isEquippedItem)
        {
            EquipementManager.instance.Equip(this, slot);
            slot.isEquippedItem = true;
        }
        else
        {
            EquipementManager.instance.Unequip((int)this.equipSlot, this);
            slot.isEquippedItem = false;
        }
        /*base.Use(slot);
        if(equiped == false)
        {
            EquipementManager.instance.Equip(this, slot);
            equiped = true;
        }
        else
        {
            EquipementManager.instance.Unequip((int)this.equipSlot, this);
            equiped = false;
        }*/
    }
}
public enum EquipementSlot { Head, chest, arm , leg}
