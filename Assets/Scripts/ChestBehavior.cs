using UnityEngine;
using System.Collections;

public class ChestBehavior : InteractableObject
{
    public int coins;



    void Start()
    {
        coins = Random.Range(2,22); 
    }

    public override void Interact(){
        OpenChest();
    }

    void OpenChest(){
        GameObject.Find("Player").GetComponent<PlayerBehavior>().LootCoins(coins);
        Destroy(this.gameObject);
    }
}
