using UnityEngine;
using System.Collections;

public class DoorBehavior : InteractableObject
{
    
    public float openTime = 1f;
    public Transform doorHinge;
    Quaternion initialRotation;

    public GameObject nextRoom;
    public Transform roomSpawn;

    GameObject spawnedRoom;

    public bool isLocked;

    public bool isOpened;

    void Start()
    {
        roomSpawn = GameObject.Find("RoomSpawn").transform;
        initialRotation = doorHinge.rotation;
        isLocked = true;
    }

    public override void Interact()
    {
        if(!isLocked && !isOpened){
            Open();
            isOpened = true;
        }else if(FindFirstObjectByType<WeaponCombat>().minDamage <= 0){
            UIManager.Instance.ShowToolTip("Pick up the dagger before moving forward!");
        }
    }

    public void Open(){
        GameObject.Find("Player").GetComponent<PlayerBehavior>().PlayerMove();
        roomSpawn.gameObject.GetComponent<RoomSpawner>().Spawn();
        StartCoroutine(RotateOpen());
    }


    private IEnumerator RotateOpen(){
        
        float timeElapsed = 0f;

        while (timeElapsed < openTime)
        {
            doorHinge.rotation = Quaternion.Lerp(initialRotation, Quaternion.Euler(0,90,0), timeElapsed / openTime);
            timeElapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        doorHinge.rotation = Quaternion.Euler(0,90,0);
    }

}
