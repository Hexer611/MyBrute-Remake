using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BrutePlayer : MonoBehaviour
{
    public Player playerEntity;
    public List<Weapon> weapons;
    public Weapon currentWeapon;

    public float currentHP;
    public float currentHitChance;
    public Vector3 direction = Vector3.zero;

    Vector3 defaultPosition;
    List<BrutePlayer> enemyList = new List<BrutePlayer>();
    BrutePlayer currentEnemy;
    [SerializeField] GameObject selfSpriteRenderer;
    [SerializeField] SpriteRenderer weaponSpriteRenderer;
    [SerializeField] SpriteRenderer[] bodySpriteRenderers;
    [SerializeField] Animator selfAnimator;
    bool canSelectNewAction;

    public bool IsAlive
    {
        get
        {
            return currentHP > 0;
        }
    }
    public float Power
    {
        get
        {
            return playerEntity.Strength * currentWeapon.Damage;
        }
    }
    public float CritChance
    {
        get
        {
            return currentWeapon.CritChance;
        }
    }
    public float CritDamage
    {
        get
        {
            return currentWeapon.CritDamage;
        }
    }
    public float MultiHitChance
    {
        get
        {
            return currentWeapon.MultiHitChance;
        }
    }
    public float WeaponDropChance
    {
        get
        {
            return currentWeapon.WeaponDropChance;
        }
    }
    // Start is called before the first frame update
    public void InitializePlayer()
    {
        // Set initial variables from entities
        weapons = new List<Weapon>(playerEntity.weapons);
        currentHP = playerEntity.HP;

        defaultPosition = transform.position;

        currentWeapon = ResourcesUtils.DefaultWeapon;

        foreach (var item in bodySpriteRenderers)
            item.color = playerEntity.bodyColor;

        SetPlayerSpriteDirection();
    }

    public void SetEnemy(BrutePlayer enemy)
    {
        this.enemyList.Add(enemy);
    }

    public Vector3 GetAttackPosition(float attackPositionOffset)
    {
        return currentEnemy.GetDefendPosition(attackPositionOffset);
    }

    public Vector3 GetDefendPosition(float attackPositionOffset)
    {
        return defaultPosition + direction * attackPositionOffset;
    }

    public IEnumerator Attack()
    {
        ChoseEnemy();
        canSelectNewAction = true;
        while (canSelectNewAction)
        {
            var newAction = SelectAction();
            yield return newAction;
        }
    }

    public IEnumerator WalkFromTo(Vector3 startPosition, Vector3 endPosition, float speed)
    {
        float n_Distance = 0;

        SetAnimatorState("Running");

        var scale = selfSpriteRenderer.transform.localScale;
        scale.x = startPosition.x > endPosition.x ? -1 : 1;
        selfSpriteRenderer.transform.localScale = scale;
        while (true)
        {
            n_Distance += Time.deltaTime * speed;
            if (n_Distance >= 1)
            {
                n_Distance = 1;
                break;
            }
            transform.position = Vector3.Lerp(startPosition, endPosition, n_Distance);
            yield return null;
        }

        transform.position = Vector3.Lerp(startPosition, endPosition, n_Distance);

        SetAnimatorState("Idle");

        yield return null;
    }

    public void SetAnimatorState(string targetState)
    {
        selfAnimator.SetBool("Idle", false);
        selfAnimator.SetBool("Running", false);
        selfAnimator.SetBool("Hitting", false);
        
        selfAnimator.SetBool(targetState, true);
    }

    public void HitEnemy()
    {
        SetAnimatorState("Hitting");
        float damage = CritChance < Random.Range(0f, 1f) ? Power : Power * CritDamage;
        currentEnemy.currentHP -= damage;
        if (WeaponDropChance > Random.Range(0f, 1f))
            currentEnemy.DropWeapon();
        var hitParticles = Instantiate(ResourcesUtils.PlayerHitParticles);
        hitParticles.transform.position = currentEnemy.transform.position;
        hitParticles.transform.localScale = new Vector3(direction.x, 1,1);
        var _burst = hitParticles.emission;
        _burst.SetBurst(0, new ParticleSystem.Burst(0, (int)(damage * 50)));
        Destroy(hitParticles.gameObject, 2f);
        MainFightUI.Instance.RefreshUI();
    }

    public void DropWeapon()
    {
        // Create weapon drop animation with weapon dropper
        if (currentWeapon != ResourcesUtils.DefaultWeapon)
        {
            var weaponDropper = Instantiate(ResourcesUtils.WeaponDropper);
            weaponDropper.Initialize(weaponSpriteRenderer, -direction);
        }

        // Remove weapon from our weapon list
        if (weapons.Contains(currentWeapon))
            weapons.Remove(currentWeapon);

        // Set our new weapon as default
        currentWeapon = ResourcesUtils.DefaultWeapon;
        weaponSpriteRenderer.sprite = currentWeapon.weaponSprite;
    }

    public void ChoseEnemy()
    {
        currentEnemy = enemyList[0];
    }

    public IEnumerator SelectAction()
    {
        if (currentWeapon.weaponName == "Fist")
            SelectWeapon();
        return AttackAction();
    }
    public void SelectWeapon()
    {
        if (weapons.Count == 0)
            return;
        currentWeapon = weapons[Random.Range(0, weapons.Count)];
        weaponSpriteRenderer.sprite = currentWeapon.weaponSprite;
    }
    public IEnumerator AttackAction()
    {
        canSelectNewAction = false;
        var attackPosition = GetAttackPosition(currentWeapon.attackPositionOffset);

        yield return WalkFromTo(defaultPosition, attackPosition, 1.25f);
        float hitChance = 1;

        while (hitChance >= Random.Range(0f, 1f))
        {
            HitEnemy();
            yield return new WaitForSeconds(0.5f);
            SetAnimatorState("Idle");
            yield return new WaitForEndOfFrame();
            hitChance = MultiHitChance;
        }

        yield return WalkFromTo(attackPosition, defaultPosition, 1.25f);

        SetPlayerSpriteDirection();

        yield return null;
    }
    public void SetPlayerSpriteDirection()
    {
        var scale = selfSpriteRenderer.transform.localScale;
        scale.x = direction.x < 0 ? -1 : 1;
        selfSpriteRenderer.transform.localScale = scale;
    }
}
