using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

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
    public float attackCoolDown;
    public float blockCoolDown;
    public float blockTime;


    public enum Direction{
        Left,
        Right,
        Center
    }

    public Direction currentDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<WeaponHandler>().animator;
        player = GameObject.Find("Player");
    }

    Direction GetInputDirection(){
        
        if(Input.GetKey(KeyCode.A)) return Direction.Left;
        if(Input.GetKey(KeyCode.D)) return Direction.Right;
        return Direction.Center;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButtonDown(0) && !coolingDown){
            Attack();
            coolingDown = true;
            //StartCoroutine(CoolDown(attackCoolDown));
        }

        if(Input.GetMouseButtonDown(1) && !coolingDown){
            Block();
            GetComponentInChildren<WeaponAnimationEvents>().StopAllCoroutines();
            Debug.Log(transform.rotation.y);
            coolingDown = true;
            //StartCoroutine(CoolDown(blockCoolDown));
        }
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

    void Block(){
        Direction dir = GetInputDirection();
        switch (dir){
            case Direction.Left:
                anim.SetTrigger("BlockLeft");
                currentDirection = Direction.Left;
            break;

            case Direction.Right:
                anim.SetTrigger("BlockRight");
                currentDirection = Direction.Right;
            break;

            case Direction.Center:
                anim.SetTrigger("BlockCenter");
                currentDirection = Direction.Center;
            break;
        }
    }
    /*
    IEnumerator CoolDown(float coolDownPeriod){
        yield return new WaitForSeconds(coolDownPeriod);
        coolingDown = false;
    }*/
}