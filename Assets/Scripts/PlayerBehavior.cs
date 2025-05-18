using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerBehavior : EntityStats
{

    public enum PlayerState{
        Idle,
        Swinging,
        Blocking,
        Stunned,
        Dead
    }

    public PlayerState currentState = PlayerState.Idle;

    public GameObject startingRoom;
    public Vector3 startingPosition;

    public Camera cam;

    public int coins;
    public int potions;
    public int xp;
    public int level;

    public float walkSpeed;
    
    public Image heartIcon;
    public Sprite[] hearts;
    public AudioClip blockSound;
    public AudioClip missSound;
    public AudioClip hurtSound;

    public InventoryManager inventory;

    bool isInventoryOpen;
    public bool dead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(dead) return;
        if(Input.GetKeyDown(KeyCode.E)){
            
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit)){
                if(hit.collider.gameObject.tag == "Interactable"){
                    hit.collider.gameObject.GetComponent<InteractableObject>().Interact();
                    Debug.Log("hit rug!");
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.I) && !isInventoryOpen){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isInventoryOpen = true;
            GetComponent<CameraControls>().enabled = false;
        }else if(Input.GetKeyDown(KeyCode.I) && isInventoryOpen){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isInventoryOpen = false;
            GetComponent<CameraControls>().enabled = true;
        }

        if(Input.GetKeyDown(KeyCode.Y)){
            Debug.Log("using first item");
            inventory.items[0].itemData.Use(this.gameObject);
            inventory.RemoveItem(inventory.items[0].itemData);
        }

        if(Input.GetKeyDown(KeyCode.U)){
            inventory.items[0].itemData.Use(this.gameObject);
            inventory.RemoveItem(inventory.items[1].itemData);
        }

        if(Input.GetKeyDown(KeyCode.P)){
            inventory.items[0].itemData.Use(this.gameObject);
            inventory.RemoveItem(inventory.items[2].itemData);
        }
    }

    public void PlayerMove(){
        StartCoroutine(MoveToNextRoom());
    }
        
    IEnumerator MoveToNextRoom(){
        Vector3 initialPosition = transform.position;
        float timeElapsed = 0f;

        while (timeElapsed < walkSpeed)
        {
            transform.position = Vector3.Lerp(initialPosition, initialPosition + Vector3.forward * 6, timeElapsed / walkSpeed);
            timeElapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure it's exactly the target rotation when done
        transform.position = initialPosition;
        GameObject.Find("RoomSpawn").GetComponent<RoomSpawner>().MoveRoom();
        UIManager.Instance.roomNumber ++;
        UIManager.Instance.roomCount.text = "Room #" + UIManager.Instance.roomNumber.ToString();
    }

    public void ShopTime(Vector3 customerPosition){
        StartCoroutine(MoveToShop(customerPosition));
    } 

    IEnumerator MoveToShop(Vector3 customerPosition){
        Vector3 initialPosition = transform.position;
        float timeElapsed = 0f;

        while (timeElapsed < walkSpeed)
        {
            transform.position = Vector3.Lerp(initialPosition, customerPosition, timeElapsed / (walkSpeed / 2));
            timeElapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure it's exactly the target rotation when done
        transform.position = customerPosition;
    }

    public void LootDrop(){
        coins += Random.Range(1, 10);
        xp += Random.Range(5,20);
    }

    public void TakeDamage(){
        currentHealth -= 1;
        currentState = PlayerState.Stunned;
        if(currentHealth >= 1){
            heartIcon.sprite = hearts[currentHealth - 1];
            GetComponent<AudioSource>().PlayOneShot(hurtSound);
        }else{
            GetComponent<AudioSource>().PlayOneShot(hurtSound);
            Die();
        }

        FindFirstObjectByType<ScreenShake>().Shake(0.2f,0.4f);
    }

    public void Heal(int healAmount){
        currentHealth += healAmount;
        heartIcon.sprite = hearts[currentHealth - 1];
    }

    public void LootCoins(int lootedCoins){
        coins += lootedCoins;
        UIManager.Instance.coinCount.text = "Coins: " + coins.ToString();
    }



    void Die(){
        if(currentHealth <= 0){
            FindFirstObjectByType<EnemyBase>().StopAllCoroutines();
            UIManager.Instance.ShowDieScreen();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GetComponent<WeaponHandler>().weaponRenderer.material.color = new Color(1,1,1,0);
            dead = true;
            GetComponent<CameraControls>().dead = true;
            Debug.Log("you died");
        }
    }

    public void Restart(){
        GameObject.Find("RoomSpawn").GetComponent<RoomSpawner>().SpawnStartingRoom();
        GameObject.Find("RoomSpawn").GetComponent<RoomSpawner>().MoveRoom();
        transform.position = startingPosition;
        UIManager.Instance.roomNumber = 0;
        UIManager.Instance.roomCount.text = "Room #" + UIManager.Instance.roomNumber.ToString();
        UIManager.Instance.ShowMainScreen();
        currentHealth = 3;
        heartIcon.sprite = hearts[currentHealth - 1];
        dead = false;
        GetComponent<CameraControls>().dead = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
