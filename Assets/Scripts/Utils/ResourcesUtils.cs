using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourcesUtils
{
    public static Weapon _defaultWeapon;
    public static Weapon DefaultWeapon
    {
        get
        {
            if (_defaultWeapon == null)
                _defaultWeapon = Resources.Load<Weapon>("Entities/Weapons/Fist");

            return _defaultWeapon;
        }
    }
    public static ParticleSystem _playerHitParticles;
    public static ParticleSystem PlayerHitParticles
    {
        get
        {
            if (_playerHitParticles == null)
                _playerHitParticles = Resources.Load<ParticleSystem>("Prefabs/PlayerHitParticles");

            return _playerHitParticles;
        }
    }
    public static WeaponDropper _weaponDropper;
    public static WeaponDropper WeaponDropper
    {
        get
        {
            if (_weaponDropper == null)
                _weaponDropper = Resources.Load<WeaponDropper>("Prefabs/WeaponDropper");

            return _weaponDropper;
        }
    }
}
