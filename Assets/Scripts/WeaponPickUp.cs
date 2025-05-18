using UnityEngine;

public class WeaponPickUp : InteractableObject
{
    public WeaponData weaponData;

    public override void Interact()
    {
        FindFirstObjectByType<DoorBehavior>().isLocked = false;
        WeaponHandler.OnWeaponChange.Invoke(weaponData);
        Destroy(gameObject);
    }
}