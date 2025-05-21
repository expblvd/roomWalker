using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCombat : MonoBehaviour
{

    Animator anim;

    public Camera playerCamera;
    public GameObject player;

    public AudioSource audioSource;
    public AudioClip weaponWoosh;

    public int minDamage;
    public int maxDamage;
    public int critDamage;
    public static int currentPlayerDamage;

    public bool coolingDown;
    public bool isBlocking;
    public bool isBlockFatigued;
    public float attackCoolDown;
    public float blockCoolDown;
    public float blockTime;
    public float blockLength;
    public float blockRecoverSpeed;

    public Image blockStaminaUIBar;

    Direction lastDirection;

    public float blockStartTimer;
    public float blockTapLength;

    public Color blockMeterColorFull;
    public Color blockMeterColorFatigued;

    public enum Direction {
        Left,
        Right,
        Center
    }

    public Direction currentDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        anim = GetComponent<WeaponHandler>().animator;
        player = GameObject.Find("Player");
        blockTime = blockLength;
    }

    void GetInputDirection() {
        if (Input.GetKey(KeyCode.A)) currentDirection = Direction.Left;
        if (Input.GetKey(KeyCode.D)) currentDirection = Direction.Right;
        if (Input.GetKey(KeyCode.S)) currentDirection = Direction.Center;
        anim.SetInteger("currentDirection", (int)currentDirection);
    }

    // Update is called once per frame
    void Update() {
        GetInputDirection();
        if (lastDirection != currentDirection) {
            audioSource.pitch = Random.Range(0.8f, 1.05f);
            audioSource.PlayOneShot(weaponWoosh);
            lastDirection = currentDirection;
        }

        if (Input.GetMouseButtonDown(0) && !coolingDown) {
                Attack();
                coolingDown = true;
                //StartCoroutine(CoolDown(attackCoolDown));
            }

        if (Input.GetMouseButtonDown(1)) {
            blockStartTimer = 0;
        }

        if (Input.GetMouseButton(1) && (blockStartTimer >= 0.1f||blockStartTimer == 0)){

            if (lastDirection != currentDirection) {
                anim.SetInteger("currentDirection", (int)currentDirection);
                lastDirection = currentDirection;
            }
        }

        if (Input.GetMouseButton(1) && !isBlockFatigued) {
            Block();
            GetComponentInChildren<WeaponAnimationEvents>().StopAllCoroutines();
            blockTime -= Time.deltaTime;
            blockStartTimer += Time.deltaTime;
            StopAllCoroutines();
            //StartCoroutine(CoolDown(blockCoolDown));
        }

        if (Input.GetMouseButtonUp(1)) {
            if (blockStartTimer >= blockTapLength) {
                anim.SetBool("isBlocking", false);
                blockStartTimer = 0;
            }
            else {
                StartCoroutine(TimedCancelBlock(blockTapLength - blockStartTimer));
            }
        }

        if (!isBlocking && !isBlockFatigued && blockTime < blockLength) {
            blockTime += (blockRecoverSpeed * 1.5f) * Time.deltaTime;
        }

        if (blockTime <= 0) {
            StartCoroutine(RecoverBlock());
            anim.SetBool("isBlocking", false);
        }

        blockStaminaUIBar.fillAmount = Mathf.Clamp01(blockTime / blockLength);
    }

    IEnumerator RecoverBlock() {
        float i = 0f;
        isBlockFatigued = true;
        blockStaminaUIBar.color = blockMeterColorFatigued;
        while (i < blockLength) {

            i += blockRecoverSpeed * Time.deltaTime;
            blockTime = i;
            yield return null; // Waits 1 frame before continuing the loop
        }

        blockTime = blockLength; // Ensure it ends exactly at the target
        isBlockFatigued = false;
        blockStaminaUIBar.color = blockMeterColorFull;
    }

    IEnumerator TimedCancelBlock(float timeToCancel) {
        yield return new WaitForSeconds(timeToCancel);
        anim.SetBool("isBlocking", false);
        blockStartTimer = 0;
    }

    void Attack(){
        //Direction dir = GetInputDirection();
        int chance = Random.Range(0,100);

        if(chance >= 0 && chance <= 50){
            currentPlayerDamage = minDamage;
        }else if(chance >50 && chance < 80){
            currentPlayerDamage = maxDamage;
        }else{
            currentPlayerDamage = critDamage;
        }
        Debug.Log(currentPlayerDamage);

        switch (currentDirection){
            case Direction.Left:
                anim.SetTrigger("StabLeft");
                currentDirection = Direction.Left;
            break;

            case Direction.Right:
                anim.SetTrigger("StabRight");
                currentDirection = Direction.Right;
            break;

            case Direction.Center:
                anim.SetTrigger("StabCenter");
                currentDirection = Direction.Center;
            break;
        }
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo)){
            if(hitInfo.transform.GetComponent<EnemyBase>() != null){
                //hitInfo.transform.GetComponent<EnemyBase>().TakeDamage(1);
            }
        }
    }

    void Block() {
        //Direction dir = GetInputDirection();
        switch (currentDirection) {
            case Direction.Left:
                currentDirection = Direction.Left;
                break;

            case Direction.Right:
                currentDirection = Direction.Right;
                break;

            case Direction.Center:
                currentDirection = Direction.Center;
                break;
        }
        anim.SetBool("isBlocking", true);
    }
    /*
    IEnumerator CoolDown(float coolDownPeriod){
        yield return new WaitForSeconds(coolDownPeriod);
        coolingDown = false;
    }*/
}
