using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapons", menuName = "Inventory/Weapons")]
public class Weapons : Item
{
    public int AttackSlashModifyer;
    public int AttackBleudgonModifyer;
    public int AttackPierceModifyer;
    public int speedModifyer;
    public int KnockBack;
    public int blockDef;
    public bool equiped;
    public string whatType;
    public int strenght;
    public GameObject currentWeapon;
    public override void Use(InventorySlot slot)
    {
        base.Use(slot);
        if (slot.isEquippedItem == false)
        {
            WeaponManager.instance.Equip(this,slot);
            slot.isEquippedItem = true;
        }
        else
        {
            WeaponManager.instance.Unequip(this);
            slot.isEquippedItem = false;
        }
    }
}
