using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyBase : EntityStats, IDamageable
{
    public enum BlockDirection{
        Left,
        Right,
        Center
    }

    public enum AttackDirection{
        Left,
        Right,
        Center
    }

    public Animator animator;

    public Image[] attackAlerts;
    
    public float alertToAttackTime;

    public WeaponCombat playerCombatStatus;
    public PlayerBehavior player;

    public bool randomHealth;
    public int minHealth;
    public int maxHealth;

    public float minAttackTime;
    public float maxAttackTime;
    public float attackSpeed;
    public bool isStunned;
    public bool isAttacking;

    public AudioSource audioSource;

    public AudioClip[] hurtSounds;
    public AudioClip dieSound;
    public AudioClip[] attackSounds;
    public AudioClip blipSound;

    public GameObject[] possibleLoot;
    public int lootChance;
    public GameObject damageSplat;

    /*
    void OnTriggerEnter(Collider other){
        if(other.transform.tag == "Finish" && !isAttacking){
            TakeDamage(WeaponCombat.currentPlayerDamage);
        }
    }*/
    public void TakeDamage(int damageAmount){
        currentHealth -= damageAmount;
        DamageEffect();
        GameObject dmgSplatInstance = Instantiate(damageSplat, transform.position, Quaternion.Euler(90,0,180));
        dmgSplatInstance.GetComponent<DamageSplat>().SetDamageText(damageAmount.ToString());
        FindFirstObjectByType<ScreenShake>().Shake(0.1f,0.03f);
        if(currentHealth <= 0){
            Die();
            foreach (Image alert in attackAlerts){
                alert.color = new Color(1,1,1,0);
            }
        }
    }

    public void AttackAlert(Image alert, AttackDirection direction){
        StartCoroutine(AttackAlertBlink(alert, direction));
    }

    
    IEnumerator AttackAlertBlink(Image alert, AttackDirection direction){
        float blinkTime = 0.1f;
        for(int i = 0; i < 4; i++){
            alert.color = new Color(1,1,1,1);
            yield return new WaitForSeconds(blinkTime);
            switch(direction){
                case AttackDirection.Left:
                    SFXManager.Instance.audioSource.panStereo = -1f;
                    break;
                case AttackDirection.Center:
                    SFXManager.Instance.audioSource.panStereo = 0f;
                    break;
                case AttackDirection.Right:
                    SFXManager.Instance.audioSource.panStereo = 1f;
                    break;
            }
            SFXManager.Instance.audioSource.PlayOneShot(blipSound);
            alert.color = new Color(1,1,1,0);
            yield return new WaitForSeconds(alertToAttackTime/4);
        }
    }

    public abstract void DamageEffect();
    public virtual void Die(){
        FindFirstObjectByType<DoorBehavior>().isLocked = false;
        GetComponent<BoxCollider>().enabled = false;
    }
}
