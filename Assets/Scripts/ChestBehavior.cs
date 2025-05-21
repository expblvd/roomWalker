using UnityEngine;
using System.Collections;

public class ChestBehavior : InteractableObject {

    public Vector3 chestOpeningPosition;
    public int coins;

    void Start() {
        coins = Random.Range(2, 22);
    }

    public override void Interact() {
        OpenChest();
    }

    void OpenChest() {
        //GameObject.Find("PlayerHolder").transform.position = chestOpeningPosition;
        //GameObject.Find("PlayerHolder").transform.rotation = Quaternion.Euler(0, 45, 0);
    }
}
