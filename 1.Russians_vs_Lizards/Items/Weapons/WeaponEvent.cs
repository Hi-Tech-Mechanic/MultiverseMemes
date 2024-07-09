using UnityEngine;

public class WeaponEvent : DataStructure
{
    public void UpgradeStatLink(string stat_name)
    {
        RectTransform[] weapon_parent;
        weapon_parent = gameObject.GetComponentsInParent<RectTransform>();

        Weapons.UpgradeStat(stat_name, weapon_parent);
    }

    public void UnlockWeapon(int weapon_index)
    {
        Weapons.UnlockWeapon(weapon_index);
    }

    public void SelectWeapon(int weapon_index)
    {
        Weapons.SwitchChoose(weapon_index);
    }
}
