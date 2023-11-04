using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon 1", menuName = "ScriptableObjects/Weapon", order = 2)]
public class Weapon: ScriptableObject
{
    public string weaponName;
    public float Damage;
    public float CritChance;
    public float CritDamage;
    public float MultiHitChance;
    public float WeaponDropChance;
    public float attackPositionOffset;
    public Sprite weaponSprite;
}
