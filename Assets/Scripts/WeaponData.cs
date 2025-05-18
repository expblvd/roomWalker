using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
public class WeaponData : ItemData
{
    public int minDamage;
    public int maxDamage;
    public int critDamage;
    public Sprite holdingImage;
    public AnimatorOverrideController animatorOverride;
    public float attackSpeed;
}