using UnityEngine;

public class ItemPickUp : InteractableObject
{
    public ItemData item;
    public override void Interact()
    {
        Debug.Log("Obtained " + item.name);
        GameObject.FindFirstObjectByType<InventoryManager>().AddItem(item);
        Destroy(gameObject);
    }
}
