using System.Collections;
using UnityEngine;

public class WeaponAnimationEvents : MonoBehaviour
{
    public WeaponCombat weaponCombat;
    public float attackRadius;
    public LayerMask enemyLayer;
    void Start()
    {
        weaponCombat = GetComponentInParent<WeaponCombat>();
    }

    public void AttackShot(){
        Collider[] collisionsDetected = Physics.OverlapSphere(transform.GetChild(0).position, attackRadius, enemyLayer);
        if(collisionsDetected.Length >= 1 && collisionsDetected[0].transform.GetComponent<EnemyBase>().isAttacking == false){
            collisionsDetected[0].transform.GetComponent<EnemyBase>().TakeDamage(WeaponCombat.currentPlayerDamage);
        }
    }

    public void EnableAttack(){
        weaponCombat.coolingDown = false;
        StartCoroutine(UnBlock());
    }

    public void Block(){
        weaponCombat.isBlocking = true;
    }

    IEnumerator UnBlock(){
        yield return new WaitForSeconds(0.2f);
        weaponCombat.isBlocking = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.GetChild(0).position, attackRadius);
    }
}
