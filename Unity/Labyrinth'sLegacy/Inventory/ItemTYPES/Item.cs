
using UnityEngine;

[CreateAssetMenu(fileName ="NewItem",menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string names = "New Item";
    public Sprite icon;
    public GameObject itemBox;
    public int max;
    public bool itemThatCanRurnToFood;
    public GameObject newItem;

    public virtual void Use(InventorySlot slot)
    {
        // Implement use functionality here
    }

}

