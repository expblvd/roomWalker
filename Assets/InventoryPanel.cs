using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public Image[] itemIconSlots;
    public TextMeshProUGUI[] quantityText;
    public InventorySlot[] inventorySlots;

    bool isOpen;

    void OnEnable()
    {
        InventoryManager.OnItemAdded += UpdateSlots;
    }

    public void UpdateSlots(ItemData addedItem){

    }
}
