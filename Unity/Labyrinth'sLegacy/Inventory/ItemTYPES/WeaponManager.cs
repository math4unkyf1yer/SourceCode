using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    #region singelton
    public static WeaponManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    InventorySlot oldslot;
    Weapons oldWeapon;
    Stats stat;
    Attack attack;
    public Transform weaponTransform;
    private GameObject newWeaponLook;
    public Transform playerTransfom;
    [Header("WeaponTool")]
    public GameObject outlinePage;

    public void Equip(Weapons newWeapon, InventorySlot slot)
    {
        stat = Stats.instance;
        attack = Attack.Intance;//which weapon use when attack 
        if (oldWeapon != null && oldWeapon != newWeapon)
        {
            oldslot.isEquippedItem = false;
            Swap(oldWeapon);
        }
        if (newWeapon.whatType == "Tool")
        {
            outlinePage.SetActive(true);//for crafting when hammer equiped 
        }
        oldWeapon = newWeapon;
        oldslot = slot;
        if (attack.weaponNameEquipped == "Unarmed")
            stat.BleudgonDamage -= 3;
        attack.weaponNameEquipped = newWeapon.whatType;

        newWeaponLook = Instantiate(newWeapon.currentWeapon);//position for the weapon
        attack.attackCollider = newWeaponLook.GetComponent<Collider>();
        // Set the weapon's parent to the weapon transform
        newWeaponLook.transform.SetParent(weaponTransform);
        // Reset the weapon's local position to zero to align it properly with the parent
        newWeaponLook.transform.localPosition = Vector3.zero;
        // Set the weapon's local rotation to follow the player's rotation but with x-axis set to 180 degrees
        newWeaponLook.transform.localRotation = Quaternion.Euler(180f, 90f, 0f);
        stat.EquipWeapon(newWeapon.AttackSlashModifyer, newWeapon.AttackBleudgonModifyer, newWeapon.AttackPierceModifyer,newWeapon.KnockBack,newWeapon.blockDef);
        stat.strenght = newWeapon.strenght;
    }

    public void Swap(Weapons oldWeapon)
    {
        RemoveHammer();//if Hammer
        //remove old booster 
        stat.UnequipWeapon(oldWeapon.AttackSlashModifyer, oldWeapon.AttackBleudgonModifyer, oldWeapon.AttackPierceModifyer,oldWeapon.KnockBack);
        stat.strenght -= oldWeapon.strenght;
        Destroy(newWeaponLook);
    }

    public void Unequip(Weapons newWeapon)  
    {
        RemoveHammer();//if hammer
        stat.UnequipWeapon(newWeapon.AttackSlashModifyer, newWeapon.AttackBleudgonModifyer, newWeapon.AttackPierceModifyer,newWeapon.KnockBack);
        attack.weaponNameEquipped = "Unarmed";
        if(stat.BleudgonDamage < 3)
            stat.BleudgonDamage += 3;
        stat.strenght = 0;
        Destroy(newWeaponLook);
        oldWeapon = null;
    }

    public void UnequipWhenDead()
    {
        if (oldWeapon != null)
        {
            // Remove the current weapon
            Unequip(oldWeapon);
        }
        oldslot = null;
        oldWeapon = null;
    }

    void RemoveHammer()
    {
        if (outlinePage.activeSelf)
        {
            attack.HammerCraftPageClose();
            outlinePage.SetActive(false);//for removing the hammer
            if (attack.craftObject != null)//make sure you remove if trying to craft
            {
                attack.stopMaterial = 0;
                attack.isReadyToCraft = false;
                Destroy(attack.craftObject);
            }
        }
    }
}
