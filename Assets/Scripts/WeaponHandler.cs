using System;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Animator animator;
    public Renderer weaponRenderer;

    public static Action<WeaponData> OnWeaponChange;
    public WeaponCombat weaponCombat;

    void Start(){
        OnWeaponChange += UpdateWeapon;
        weaponCombat = GetComponent<WeaponCombat>();
        weaponRenderer.material.color = new Color(1,1,1,0);
    }

    void UpdateWeapon(WeaponData weaponData){
        weaponRenderer.material.color = new Color(1,1,1,1);
        animator.runtimeAnimatorController = weaponData.animatorOverride;
        animator.SetFloat("AttackSpeed", weaponData.attackSpeed);
        weaponRenderer.material.mainTexture = weaponData.holdingImage.texture;
        GetComponent<WeaponCombat>().minDamage = weaponData.minDamage;
        GetComponent<WeaponCombat>().maxDamage = weaponData.maxDamage;
        GetComponent<WeaponCombat>().critDamage = weaponData.critDamage;
    }
}
