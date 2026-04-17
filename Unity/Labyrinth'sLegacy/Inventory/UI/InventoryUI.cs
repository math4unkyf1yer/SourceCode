using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;
    public Transform itemParents;
    public Transform itemParents2;
 

    public InventorySlot[] slotsUi;

    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.Intance;
        inventory.onItemCallBack += UpdateUI;
      //  slotsUi = itemParents.GetComponentsInChildren<InventorySlot>();
       // slots2UI = itemParents2.GetComponentsInChildren<InventorySlot>();
        
    }

    private void UpdateUI()
    {
        for(int i = 0; i < slotsUi.Length; i++)
        {
            Slot s = inventory.getSlotAt(i);

            if (s != null && s.items.Count > 0)
            {
                slotsUi[i].SetInventoryInfo(s.items[0], s.items.Count,s);
            }
            else
            {
                slotsUi[i].SetInventoryInfo(null, 0,null);
            }
          
        }
    }
}
