using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public GameObject startingRoom;
    public GameObject currentRoom;
    public GameObject nextRoom;
    public GameObject spawnedRoom;

    public void SpawnStartingRoom(){
        spawnedRoom = Instantiate(startingRoom, transform.position, Quaternion.Euler(0,0,0));
    }

    public void Spawn(){
        spawnedRoom = Instantiate(nextRoom, transform.position, Quaternion.Euler(0,0,0));
    }

    public void MoveRoom(){
        spawnedRoom.transform.position = Vector3.zero;
        Destroy(currentRoom);
        currentRoom = spawnedRoom;
    }
}
