using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    public int healthIncrease;
    public int staminaIncrease;
    public float foodTimer;
    public float healthPersec;
    public float staminaPersec;
    public override void Use(InventorySlot slot)
    {
        base.Use(slot);    
        if(Stats.instance.foodAte != 3)
        {
            for (int i = 0; i < Stats.instance.nameOfFood.Length; i++)
            {
                if(this.name == Stats.instance.nameOfFood[i])
                {
                    Debug.Log("Already ate that type of food");
                    return;
                }
            }
            if (slot.amount == 1)
            {
                slot.amount = 0;
            }
            slot.TextItem(this);
            RemoveFromInventory();
            Stats.instance.Eating(staminaIncrease, healthIncrease,healthPersec, staminaPersec,foodTimer, this.icon, this.names);
        }
        else
        {
            Debug.Log("full");
        }
    }
    public void RemoveFromInventory()
    {
        Inventory.Intance.Remove(this, false);
    }

}
