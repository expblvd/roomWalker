using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JasonTheBoxer : EnemyBase
{
    public AudioClip tiredSound;
    public AudioClip idleSound;
    public AudioClip chargeSound;
    public float freezeFrameTime;
    void Start()
    {
        audioSource.clip = idleSound;
        audioSource.Play();
        animator = GetComponent<Animator>();
        attackAlerts = new Image[3];
        attackAlerts[(int)AttackDirection.Left] = UIManager.Instance.attackAlertLeft;
        attackAlerts[(int)AttackDirection.Right] = UIManager.Instance.attackAlertRight;
        attackAlerts[(int)AttackDirection.Center] = UIManager.Instance.attackAlertCenter;
        playerCombatStatus = GameObject.FindWithTag("Player").GetComponent<WeaponCombat>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBehavior>();
        StartCoroutine(Barrage());
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
        audioSource.PlayOneShot(attackSounds[Random.Range(0,attackSounds.Length)]);

    }

    IEnumerator Barrage(){
            yield return new WaitForSeconds(1.5f);
            Debug.Log("starting coroutine waited 1f seconds");
            AttackDirection chosenDirection;
            AttackDirection lastDirection = AttackDirection.Center;
            animator.SetTrigger("ChargeAttack");
            audioSource.Stop();
            audioSource.clip = chargeSound;
            audioSource.Play();
            for(int i = 0; i < Random.Range(5,8); i++){
                Debug.Log("attacking!");
                chosenDirection = (AttackDirection)UnityEngine.Random.Range(0,3);
                while(chosenDirection == lastDirection){
                    chosenDirection = (AttackDirection)UnityEngine.Random.Range(0,3);
                }
                lastDirection = chosenDirection;
                AttackAlert(attackAlerts[(int)chosenDirection], chosenDirection);
                    yield return new WaitForSeconds(alertToAttackTime + (alertToAttackTime/4) + 0.4f);
                    Attack(chosenDirection);
                    attackAlerts[(int)chosenDirection].color = new Color(1,1,1,0);
                yield return new WaitForSeconds(alertToAttackTime);
            }
            Debug.Log("hes tired now! ATTACK");
            animator.SetTrigger("Tired");
            audioSource.clip = tiredSound;
            audioSource.Play();
            alertToAttackTime -= 0.05f;
            if(alertToAttackTime <= 0.01f){
                alertToAttackTime = 0.01f;
            }
            yield return new WaitForSeconds(2f);
            audioSource.Stop();
            audioSource.clip = idleSound;
            audioSource.Play();
            animator.SetTrigger("Idle");
            StartCoroutine(Barrage());
    }

    public void AttackReached(AttackDirection attackDirection){
        if((int)attackDirection == (int)playerCombatStatus.currentDirection && playerCombatStatus.isBlocking){
            Debug.Log("Attack Blocked!");
            SFXManager.Instance.audioSource.PlayOneShot(player.blockSound);
            //StartCoroutine(FreezeFrame(freezeFrameTime)); // 0.1 seconds freeze
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

    IEnumerator FreezeFrame(float duration)
    {
        Time.timeScale = 0f;
        FindFirstObjectByType<CameraControls>().enabled = false;
        yield return new WaitForSecondsRealtime(duration); // Realtime, so it's not affected by timescale
        Time.timeScale = 1f;
        FindFirstObjectByType<CameraControls>().enabled = true;//brings back player camera control.
    }

    //Perish or Get Destroyed if an inanimate object(Barrel, Box, Etc;)
    public override void Die(){
        base.Die();
        Debug.Log("dead");
        audioSource.Stop();
        GetComponent<AudioSource>().PlayOneShot(dieSound);
        audioSource.PlayOneShot(hurtSounds[Random.Range(0,hurtSounds.Length)]);
        StopAllCoroutines();
        animator.SetTrigger("Die");
        isStunned = true;
        DropLoot();
        //Time.timeScale = 0.2f;
    }
}
