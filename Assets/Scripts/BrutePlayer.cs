using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BrutePlayer : BruteBase
{
    [SerializeField] SpriteRenderer weaponSpriteRenderer;

    public override void DropWeapon()
    {
        // Create weapon drop animation with weapon dropper
        if (currentWeapon != playerEntity.defaultWeapon)
        {
            var weaponDropper = Instantiate(ResourcesUtils.WeaponDropper);
            weaponDropper.Initialize(weaponSpriteRenderer, -direction);
        }

        // Remove weapon from our weapon list
        if (weapons.Contains(currentWeapon))
            weapons.Remove(currentWeapon);

        // Set our new weapon as default
        currentWeapon = playerEntity.defaultWeapon;
        weaponSpriteRenderer.sprite = currentWeapon.weaponSprite;
    }

    public void SelectWeapon()
    {
        if (weapons.Count == 0)
            return;
        currentWeapon = weapons[Random.Range(0, weapons.Count)];
        weaponSpriteRenderer.sprite = currentWeapon.weaponSprite;
    }

    public override IEnumerator SelectAction()
    {
        if (currentWeapon.weaponName == "Fist")
            SelectWeapon();
        return AttackAction();
    }

    protected override IEnumerator DIE()
    {
        // An interesting way to use any state of the animator :D
        // If we just say dead is true the animation keeps starting over
        selfAnimator.SetBool("Dead", true);
        yield return null;
        selfAnimator.SetBool("Dead", false);
        yield return null;
    }
}
