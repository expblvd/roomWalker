[System.Serializable]
public class InventoryItem
{
    public ItemData itemData;
    public int quantity;

    public InventoryItem(ItemData item)
    {
        itemData = item;
        AddToStack();
    }

    public void AddToStack(){
        quantity ++;
    }

    public void RemoveFromStack(){
        quantity --;
    }

}