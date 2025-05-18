using TMPro;
using UnityEngine;

public class ItemBuy : InteractableObject
{
    public int itemPrice;
    public ItemData item;
    public TextMeshProUGUI text;
    Color textColor;

    void Start()
    {
        textColor = text.color;
        textColor.a = 0;
        text.color = textColor;
        itemPrice = item.price;
        text.text = item.name + "\n" + item.itemDescription + "\n" + "Price: " + item.price.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToolTipShow(){
        textColor.a = 1;
        text.color = textColor;
    }

    public void ToolTipHide(){
        textColor.a = 0;
        text.color = textColor;
    }

    public override void Interact()
    {
        PlayerBehavior playerBehavior = GameObject.FindWithTag("Player").GetComponent<PlayerBehavior>();
        if(playerBehavior.coins >= item.price){
            playerBehavior.coins -= item.price;
            UIManager.Instance.coinCount.text = "Coins: "+ GameObject.FindWithTag("Player").GetComponent<PlayerBehavior>().coins.ToString();
            GameObject.FindFirstObjectByType<InventoryManager>().AddItem(item);
            Destroy(gameObject);
        }else{
            Debug.Log("not enough coins bro");
        }
    }
}
