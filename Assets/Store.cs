using System.Collections.Generic;
using UnityEngine;

public class Store : InteractableObject
{
    public Transform customerPosition;
    public Transform[] itemSlots;
    public GameObject[] inventory;
    public List<GameObject> itemsForSale = new List<GameObject>();

    public override void Interact(){
        GameObject.FindWithTag("Player").GetComponent<PlayerBehavior>().ShopTime(customerPosition.position);
        Debug.Log("calling interact!");
    }

    void Start()
    {
        foreach(Transform slot in itemSlots){

            int retryCount = 0;
            int maxRetries = 10;
            GameObject chosenItem = inventory[Random.Range(0, inventory.Length)];
            while(itemsForSale.Contains(chosenItem)){
                if(++retryCount >= maxRetries){
                    Debug.Log("Attempted " + maxRetries + "Times... Spawning Duplicates sorry");
                    break;
                }
                chosenItem = inventory[Random.Range(0, inventory.Length)];
            }
            itemsForSale.Add(chosenItem);
            Instantiate(chosenItem, slot.position, Quaternion.identity);
        }
    }
}
