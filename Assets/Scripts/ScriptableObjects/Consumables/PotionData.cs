using UnityEngine;
[CreateAssetMenu(fileName = "New Potion", menuName = "Items/Potions")]
public class Potion : ItemData{
    
    public int potencyAmount;
    public enum EffectType{
        Heal,
        Poison,
        Speed
    }
    public EffectType effect;

    public override void Use(GameObject userObject){
        Debug.Log("Using potion!");
        EntityStats stats = userObject.GetComponent<EntityStats>();
        switch(effect){
            case EffectType.Heal:
                stats.currentHealth += potencyAmount;
                break;
            case EffectType.Poison:
                stats.currentHealth -= potencyAmount;
                break;
        }
    }
}