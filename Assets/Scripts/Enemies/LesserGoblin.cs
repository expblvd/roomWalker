using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LesserGoblin : EnemyBase
{
    Coroutine attackCycle;
    void Start()
    {
        if(randomHealth){
            currentHealth = Random.Range(minHealth,maxHealth+1);
        }
        animator = GetComponent<Animator>();
        attackAlerts = new Image[3];
        attackAlerts[(int)AttackDirection.Left] = UIManager.Instance.attackAlertLeft;
        attackAlerts[(int)AttackDirection.Right] = UIManager.Instance.attackAlertRight;
        attackAlerts[(int)AttackDirection.Center] = UIManager.Instance.attackAlertCenter;
        attackCycle = StartCoroutine(AttackCycle());
        playerCombatStatus = GameObject.FindWithTag("Player").GetComponent<WeaponCombat>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehavior>();
    }

    IEnumerator AttackCycle(){
        yield return new WaitForSeconds(0.5f);
        Debug.Log("beginning attack cycle!");
        while(true){
            while(isStunned)
                yield return null;
            float waitTime = UnityEngine.Random.Range(minAttackTime, maxAttackTime);
            Debug.Log("waiting " + waitTime.ToString() + "seconds");
            yield return new WaitForSeconds(waitTime);
            AttackDirection chosenDirection = (AttackDirection)UnityEngine.Random.Range(0,3);
            animator.SetTrigger("ChargeAttack");
            AttackAlert(attackAlerts[(int)chosenDirection], chosenDirection);
            yield return new WaitForSeconds(alertToAttackTime + (alertToAttackTime/4) + 0.4f);
            isAttacking = true;
            Attack(chosenDirection);
            yield return new WaitForSeconds(0.4f);
            isAttacking = false;
        }
    }

    void Attack(AttackDirection attackDirection){
        Debug.Log($"I am a Lesser Goblin and I am attacking you from {attackDirection}!");
        switch(attackDirection){
            case AttackDirection.Left:
                animator.SetTrigger("AttackLeft");
                break;
            case AttackDirection.Right:
                animator.SetTrigger("AttackRight");
                break;
            case AttackDirection.Center:
                animator.SetTrigger("AttackCenter");
                break;
        }
        
    }

    public void AttackReached(AttackDirection attackDirection){
        if((int)attackDirection == (int)playerCombatStatus.currentDirection && playerCombatStatus.isBlocking){
            Debug.Log("Attack Blocked!");
            SFXManager.Instance.audioSource.PlayOneShot(player.blockSound);
            FindFirstObjectByType<ScreenShake>().Shake(0.2f,0.04f);
        }else{
            Debug.Log("Attack Landed!");
            player.TakeDamage();
        }
        Debug.Log("animation reached player");
    }

    //Particles, Sounds, Etc;
    public override void DamageEffect(){
        GetComponent<AudioSource>().PlayOneShot(hurtSounds[Random.Range(0,hurtSounds.Length)]);
    }

    public void DropLoot(){
        int roll = Random.Range(0,101);
        Debug.Log(roll);
        if(roll <= lootChance){
            Debug.Log("Loot Drop!");
            Instantiate(possibleLoot[Random.Range(0,possibleLoot.Length)], transform.parent);
        }
    }

    //Perish or Get Destroyed if an inanimate object(Barrel, Box, Etc;)
    public override void Die(){
        base.Die();
        GetComponent<AudioSource>().PlayOneShot(dieSound);
        StopAllCoroutines();
        animator.SetTrigger("Die");
        isStunned = true;
        DropLoot();
    }
}