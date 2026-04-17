using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipementManager : MonoBehaviour
{
    #region singelton
    public static EquipementManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    Equipement[] currentEquipement;
    InventorySlot oldslot;
    Equipement oldEquipement;
    public Transform[] equipementTransform;
    private GameObject[] newEquipLook = new GameObject[4];
    public Transform leftArmTransform;  // Assign in the Inspector
    public Transform rightArmTransform; // Assign in the Inspector
    private GameObject[] newArmLook = new GameObject[2];
    public Transform leftLegTransform;  // Assign in the Inspector
    public Transform rightLegTransform;
    private GameObject[] newLegLook = new GameObject[2];
    int oldslotIndex;
    private void Start()
    {
        int numbSlot = System.Enum.GetNames(typeof(EquipementSlot)).Length;
        currentEquipement = new Equipement[numbSlot];
    }

    public void Equip(Equipement newEquip,InventorySlot slot)
    {
        int slotIndex = (int)newEquip.equipSlot;
        if (oldslot != null)
        {
            //oldslot.EquipementOff();
        }
        if(oldEquipement != null  && oldslotIndex == slotIndex)
        {
            oldslot.isEquippedItem = false;
            Swap(oldEquipement);
        }
        oldslotIndex = slotIndex;


        // Handle arm equipment separately
        if (slotIndex == (int)EquipementSlot.arm)
        {
            // Instantiate left arm armor
            GameObject leftArmPiece = Instantiate(newEquip.leftArmArmor); // Add a `leftArmArmor` property to your Equipement class
            leftArmPiece.transform.SetParent(leftArmTransform);
            leftArmPiece.transform.localPosition = Vector3.zero;
            leftArmPiece.transform.localRotation = Quaternion.identity;

            // Instantiate right arm armor
            GameObject rightArmPiece = Instantiate(newEquip.rightArmArmor); // Add a `rightArmArmor` property to your Equipement class
            rightArmPiece.transform.SetParent(rightArmTransform);
            rightArmPiece.transform.localPosition = Vector3.zero;
            rightArmPiece.transform.localRotation = Quaternion.identity;

            // Store references for cleanup
            newArmLook[0] = leftArmPiece;
            newArmLook[1] = rightArmPiece;
        }
        else if (slotIndex == (int)EquipementSlot.leg)//handle leg equipment seperatly for th eanimation look 
        {
            // Instantiate left leg armor
            GameObject leftLegPiece = Instantiate(newEquip.leftLegArmor); // Add a `leftArmArmor` property to your Equipement class
            leftLegPiece.transform.SetParent(leftLegTransform);
            leftLegPiece.transform.localPosition = Vector3.zero;
            leftLegPiece.transform.localRotation = Quaternion.identity;

            // Instantiate right leg armor
            GameObject rightLegPiece = Instantiate(newEquip.rightLegArmor); // Add a `rightArmArmor` property to your Equipement class
            rightLegPiece.transform.SetParent(rightLegTransform);
            rightLegPiece.transform.localPosition = Vector3.zero;
            rightLegPiece.transform.localRotation = Quaternion.identity;

            // Store references for cleanup
            newLegLook[0] = leftLegPiece;
            newLegLook[1] = rightLegPiece;
        }
        else
        {
            // Handle other equipment slots as usual
            newEquipLook[slotIndex] = Instantiate(newEquip.currentArmor);
            Transform targetBone = equipementTransform[slotIndex];
            newEquipLook[slotIndex].transform.SetParent(targetBone);
            newEquipLook[slotIndex].transform.localPosition = Vector3.zero;
            newEquipLook[slotIndex].transform.localRotation = Quaternion.identity;
        }

        currentEquipement[slotIndex] = newEquip;
        //add booster
        Stats.instance.EquipArmor(newEquip.slashRes,newEquip.BleudRes,newEquip.pierceRes, newEquip.weightModifyer);
        oldEquipement = newEquip;
        oldslot = slot;
    }

    public void Swap(Equipement oldEquip)
    {
        //remove old booster 
        Stats.instance.UnequipArmor(oldEquip.slashRes, oldEquip.BleudRes, oldEquip.pierceRes, oldEquip.weightModifyer);
        // Handle arm equipment
        if (oldslotIndex == (int)EquipementSlot.arm)
        {
            Destroy(newArmLook[0]); // Destroy left arm piece
            Destroy(newArmLook[1]); // Destroy right arm piece
            newArmLook[0] = null;
            newArmLook[1] = null;
        }else if (oldslotIndex == (int)EquipementSlot.leg)
        {
            Destroy(newLegLook[0]); // Destroy left arm piece
            Destroy(newLegLook[1]); // Destroy right arm piece
            newLegLook[0] = null;
            newLegLook[1] = null;
        }
        else
        {
            // Handle other equipment
            Destroy(newEquipLook[oldslotIndex]);
            newEquipLook[oldslotIndex] = null;
        }
    }
    public void Unequip(int slotIndex, Equipement equip)
    {
        if (currentEquipement[slotIndex] != null)
        {
            // Remove boosters
            Stats.instance.UnequipArmor(equip.slashRes, equip.BleudRes, equip.pierceRes, equip.weightModifyer);

            if (slotIndex == (int)EquipementSlot.arm)
            {
                // Handle arm equipment
                Destroy(newArmLook[0]); // Destroy left arm piece
                Destroy(newArmLook[1]); // Destroy right arm piece
                newArmLook[0] = null;
                newArmLook[1] = null;
            }
            else if (slotIndex == (int)EquipementSlot.leg)
            {
                // Handle leg equipment
                Destroy(newLegLook[0]); // Destroy left leg piece
                Destroy(newLegLook[1]); // Destroy right leg piece
                newLegLook[0] = null;
                newLegLook[1] = null;
            }
            else
            {
                // Handle other equipment
                Destroy(newEquipLook[slotIndex]);
                newEquipLook[slotIndex] = null;
            }

            currentEquipement[slotIndex] = null; // Clear the equipped item
            Debug.Log(oldEquipement);
        }
    }
    public void UnequipWhenDead()
    {
        for (int i = 0; i < currentEquipement.Length; i++)
        {
            if (currentEquipement[i] != null)
            {
                Unequip(i, currentEquipement[i]); // Use the existing Unequip method
            }
        }
    }
}
