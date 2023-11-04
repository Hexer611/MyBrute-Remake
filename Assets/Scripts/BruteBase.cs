using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class BruteBase : MonoBehaviour
{
    public Player playerEntity;
    public List<Weapon> weapons;
    public Weapon currentWeapon;

    public float currentHP;
    public float currentHitChance;
    public Vector3 direction = Vector3.zero;

    Vector3 defaultPosition;
    public List<BruteBase> petList = new List<BruteBase>();
    BruteBase currentEnemy;
    [SerializeField] GameObject selfSpriteRenderer;
    [SerializeField] SpriteRenderer[] bodySpriteRenderers;
    [SerializeField] protected Animator selfAnimator;
    bool canSelectNewAction;

    public bool IsAlive => currentHP > 0;
    public float Power => playerEntity.Strength * currentWeapon.Damage;
    public float CritChance => currentWeapon.CritChance;
    public float CritDamage => currentWeapon.CritDamage;
    public float MultiHitChance => currentWeapon.MultiHitChance;
    public float WeaponDropChance => currentWeapon.WeaponDropChance;
    public static List<BruteBase> leftBrutes = new List<BruteBase>();
    public static List<BruteBase> rightBrutes = new List<BruteBase>();

    // Start is called before the first frame update
    public void InitializePlayer()
    {
        RegisterToBruteList();
        // Set initial variables from entities
        weapons = new List<Weapon>(playerEntity.weapons);
        currentHP = playerEntity.HP;

        defaultPosition = transform.position;

        currentWeapon = playerEntity.defaultWeapon;

        foreach (var item in bodySpriteRenderers)
            item.color = playerEntity.bodyColor;

        SetSpriteDirection();

        foreach (var pet in playerEntity.pets)
        {
            var newPet = Instantiate(pet.petPrefab);
            newPet.transform.position = defaultPosition + new Vector3(0,Random.Range(0.2f, -1.0f),0) - direction;
            newPet.direction = direction;
            newPet.InitializePlayer();
            petList.Add(newPet);
        }
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
        currentEnemy.ReceiveDamage(damage);
        if (WeaponDropChance > Random.Range(0f, 1f))
            currentEnemy.DropWeapon();
        var hitParticles = Instantiate(ResourcesUtils.PlayerHitParticles);
        hitParticles.transform.position = currentEnemy.transform.position;
        hitParticles.transform.localScale = new Vector3(direction.x, 1, 1);
        var _burst = hitParticles.emission;
        _burst.SetBurst(0, new ParticleSystem.Burst(0, (int)(damage * 50)));
        Destroy(hitParticles.gameObject, 2f);
        MainFightUI.Instance.RefreshUI();
    }

    public void ReceiveDamage(float damage)
    {
        currentHP -= damage;
        if (!IsAlive)
        {
            if (direction.x == 1)
                leftBrutes.Remove(this);
            if (direction.x == -1)
                rightBrutes.Remove(this);

            // Since this is only an animation we can start it parallel to all the action
            StartCoroutine(DIE());
        }
    }

    public void ChoseEnemy()
    {
        // We chose our enemy randomly

        if (direction.x == 1)
            currentEnemy = rightBrutes[Random.Range(0, rightBrutes.Count)];
        if (direction.x == -1)
            currentEnemy = leftBrutes[Random.Range(0, leftBrutes.Count)];
    }

    public virtual IEnumerator SelectAction()
    {
        return AttackAction();
    }

    public IEnumerator AttackAction()
    {
        canSelectNewAction = false;
        var attackPosition = GetAttackPosition(currentWeapon.attackPositionOffset);

        yield return WalkFromTo(defaultPosition, attackPosition, 1.25f);
        float hitChance = 1;

        while (hitChance >= Random.Range(0f, 1f) && currentEnemy.IsAlive)
        {
            HitEnemy();
            yield return new WaitForSeconds(0.5f);
            SetAnimatorState("Idle");
            yield return new WaitForEndOfFrame();
            hitChance = MultiHitChance;
        }

        yield return WalkFromTo(attackPosition, defaultPosition, 1.25f);

        SetSpriteDirection();

        yield return null;
    }

    public void SetSpriteDirection()
    {
        var scale = selfSpriteRenderer.transform.localScale;
        scale.x = direction.x < 0 ? -1 : 1;
        selfSpriteRenderer.transform.localScale = scale;
    }

    public virtual void DropWeapon()
    {

    }

    private void RegisterToBruteList()
    {
        if (direction.x == 1)
            leftBrutes.Add(this);
        if (direction.x == -1)
            rightBrutes.Add(this);
    }

    protected virtual IEnumerator DIE()
    {
        float timer = 4f;
        while (timer > 0)
        {
            foreach (var item in bodySpriteRenderers)
                item.color -= new Color(0, 0, 0, Time.deltaTime / 2);

            timer -= Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
