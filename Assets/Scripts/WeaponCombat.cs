using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCombat : MonoBehaviour
{

    Animator anim;

    public Camera playerCamera;
    public GameObject player;

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

    public enum Direction{
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

    Direction GetInputDirection(){

        if(Input.GetKey(KeyCode.A)) return Direction.Left;
        if(Input.GetKey(KeyCode.D)) return Direction.Right;
        return Direction.Center;
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetMouseButtonDown(0) && !coolingDown) {
            Attack();
            coolingDown = true;
            //StartCoroutine(CoolDown(attackCoolDown));
        }

        if (Input.GetMouseButton(1) && !isBlockFatigued) {
            Block();
            GetComponentInChildren<WeaponAnimationEvents>().StopAllCoroutines();
            blockTime -= Time.deltaTime;
            StopAllCoroutines();
            //StartCoroutine(CoolDown(blockCoolDown));
        }

        if (Input.GetMouseButtonUp(1)) {
            anim.SetBool("isBlocking", false);
        }

        if (!isBlocking && !isBlockFatigued && blockTime < blockLength) {
            blockTime += (blockRecoverSpeed * 1.5f) * Time.deltaTime;
        }

        if (blockTime <= 0) {
            StartCoroutine(RecoverBlock());
            anim.SetBool("isBlocking", false);
        }

        currentDirection = GetInputDirection();
        if (lastDirection != currentDirection) {
            anim.SetInteger("currentDirection", (int)currentDirection);
            lastDirection = currentDirection;
        }

        blockStaminaUIBar.fillAmount = Mathf.Clamp01(blockTime / blockLength);
    }

    IEnumerator RecoverBlock() {
        float i = 0f;
        isBlockFatigued = true;
        blockStaminaUIBar.color = new Color(1, 0, 0);
        while (i < blockLength) {

            i += blockRecoverSpeed * Time.deltaTime;
            blockTime = i;
            yield return null; // Waits 1 frame before continuing the loop
        }

        blockTime = blockLength; // Ensure it ends exactly at the target
        isBlockFatigued = false;
        blockStaminaUIBar.color = new Color(0, 1, 0);
    }

    void Attack(){
        Direction dir = GetInputDirection();
        int chance = Random.Range(0,100);

        if(chance >= 0 && chance <= 50){
            currentPlayerDamage = minDamage;
        }else if(chance >50 && chance < 80){
            currentPlayerDamage = maxDamage;
        }else{
            currentPlayerDamage = critDamage;
        }
        Debug.Log(currentPlayerDamage);

        switch (dir){
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
        Direction dir = GetInputDirection();
        switch (dir) {
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
