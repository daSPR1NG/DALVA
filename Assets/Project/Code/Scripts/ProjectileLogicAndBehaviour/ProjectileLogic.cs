﻿using UnityEngine;
using UnityEngine.AI;

public enum ProjectileType { None, TravelsForward, TravelsToAPosition }

[RequireComponent(typeof(Rigidbody))]
public class ProjectileLogic : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType = ProjectileType.None;
    [SerializeField] private float projectileSpeed = 0f;
    [SerializeField] private float projectileAreaOfEffect = 0f;
    [Range(0, 1)][SerializeField] private float projectileBonusDamagePercentage = 0f;
    [SerializeField] private int entitiesLayerMaskValue = 12;


    [SerializeField] private StatusEffect projectileStatusEffect;
    [SerializeField] private GameObject onHitVFX;

    [SerializeField] private Transform projectileSender; //debug

    [SerializeField] private Transform target; //debug
    [SerializeField] private EntityStats targetStats; //debug

    public Ability Ability { get; set; }

    Vector3 targetPosition;

    public bool CanGoThroughTarget { get; set; }
    public bool CanHeal { get; set; }
    public bool CanBounce { get; set; }

    public float TotalPhysicalDamage { get; set; }
    public float TotalMagicalDamage { get; set; }

    public ProjectileType ProjectileType { get => projectileType; set => projectileType = value; }
    public float ProjectileSpeed { get => projectileSpeed; }
    public StatusEffect ProjectileStatusEffect { get => projectileStatusEffect; }
    public GameObject OnHitVFX { get => onHitVFX; }
    
    public Transform Target { get => target; set => target = value; }

    public Transform ProjectileSender { get => projectileSender; set => projectileSender = value; }
    public EntityStats ProjectileSenderStats => ProjectileSender.GetComponent<EntityStats>();
  
    private Rigidbody Rb => GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        switch (projectileType)
        {
            case ProjectileType.None:
                break;
            case ProjectileType.TravelsForward:
                ProjectileTravelsToPosition(ProjectileSender);
                break;
            case ProjectileType.TravelsToAPosition:
                ProjectileMoveToATarget();
                break;
        }
    }

    #region Projectile Behaviours
    void ProjectileMoveToATarget()
    {
        if (Target.GetComponent<NavMeshAgent>() != null)
        {
            targetPosition = Target.position;

            Rb.MovePosition(Vector3.MoveTowards(
                transform.position,
                targetPosition + new Vector3(0, Target.GetComponent<NavMeshAgent>().height / 2, 0),
                ProjectileSpeed * Time.fixedDeltaTime));

            transform.LookAt(Target);
        }
        else if (Target.GetComponent<NavMeshAgent>() == null)
        {
            targetPosition = Target.position;

            Rb.MovePosition(Vector3.MoveTowards(
                transform.position,
                targetPosition + new Vector3(0, 0.5f, 0),
                ProjectileSpeed * Time.fixedDeltaTime));

            transform.LookAt(Target);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ProjectileTravelsToPosition(Transform sender)
    {
        ProjectileSender = sender;
        //Rb.MovePosition(Vector3.MoveTowards(
        //    transform.position,
        //    Target.position,
        //    ProjectileSpeed * Time.fixedDeltaTime));
        Rb.MovePosition(transform.position += transform.forward * (ProjectileSpeed * Time.fixedDeltaTime));
    }
    #endregion

    #region On hit behaviour
    private void OnTriggerEnter(Collider other)
    {
        ApplyDamageOnTargetHit(other);
    }

    protected void ApplyDamageOnTargetHit(Collider targetCollider)
    {
        if (targetCollider.gameObject.GetComponent<EntityStats>() != null)
        {
            Debug.Log("Enemy touched !");

            EntityDetection entityFound = targetCollider.gameObject.GetComponent<EntityDetection>();
            EntityStats targetStat = targetCollider.GetComponent<EntityStats>();

            //Ability projectile damage appplication
            //Needs to be modified to only include Player - Interactive building - Monster - Minion
            if (entityFound.ThisTargetIsAPlayer(entityFound)
                || entityFound.ThisTargetIsAMinion(entityFound)
                || entityFound.ThisTargetIsAStele(entityFound)
                || entityFound.ThisTargetIsAMonster(entityFound)
                || ProjectileSenderStats.EntityTeam != targetStats.EntityTeam)
            {
                Debug.Log("Projectile Applies Damage !");

                ApplyStatusEffectOnHit(targetCollider);

                //Maybe it needs to add something else for the ratio added to projectile damage +% ????
                if (Ability != null)
                {
                    if (ProjectileSenderStats.EntityTeam != targetStat.EntityTeam) ApplyProjectileAbilityDamage(targetStat);
                    else if (CanHeal && ProjectileSenderStats.EntityTeam == targetStat.EntityTeam) ApplyProjectileAbilityHeal(targetStat);
                }
                else if (Target != null && targetCollider.transform.gameObject == Target.gameObject)
                {
                    ApplyProjectileAutoAttackDamage(targetStat);
                }
            }
        }
        else if (targetCollider.gameObject.GetComponent<SteleLogic>() != null)
        {
            ApplyProjectileDamageToAnInteractiveBuilding(targetCollider);
        }
    }

    private void ApplyProjectileAbilityDamage(EntityStats targetStat)
    {
        Debug.Log("Has an ability");

        float bonusDamage;

        if (targetStat.EntityIsMarked)
        {
            targetStat.EntityIsMarked = false;
            bonusDamage = projectileBonusDamagePercentage;
        }
        else bonusDamage = 0;

        if (Ability.AbilityPhysicalDamage > 0)
        {
            TotalPhysicalDamage = Ability.AbilityPhysicalDamage + (ProjectileSenderStats.GetStat(StatType.PhysicalPower).Value * (Ability.AbilityPhysicalRatio + bonusDamage));
        } 
        else Ability.AbilityPhysicalDamage = 0;

        if (Ability.AbilityMagicalDamage > 0)
        {
           TotalMagicalDamage = Ability.AbilityMagicalDamage + (ProjectileSenderStats.GetStat(StatType.MagicalPower).Value * (Ability.AbilityMagicalRatio + bonusDamage));
        } 
        else Ability.AbilityMagicalDamage = 0;

        targetStat.TakeDamage(
        ProjectileSender,
        targetStat.GetStat(StatType.PhysicalResistances).Value,
        targetStat.GetStat(StatType.MagicalResistances).Value,
        TotalPhysicalDamage,
        TotalMagicalDamage,
        ProjectileSenderStats.GetStat(StatType.CriticalStrikeChance).Value,
        175f,
        ProjectileSenderStats.GetStat(StatType.PhysicalPenetration).Value,
        ProjectileSenderStats.GetStat(StatType.MagicalPenetration).Value);

        if (!CanGoThroughTarget && !CanBounce)
            InstantiateHitEffectOnCollision(OnHitVFX);

        if (CanBounce) BounceOnOtherNearTarget();
        else if(!CanGoThroughTarget) Destroy(gameObject);
    }

    private void ApplyProjectileAbilityHeal(EntityStats targetStat)
    {
        if (!targetStat.EntityIsMarked) return;

            Debug.Log("Has an ability");

        if (targetStat.EntityIsMarked)
            targetStat.EntityIsMarked = false;

        targetStat.Heal(targetStat.transform, 25 + (ProjectileSenderStats.GetStat(StatType.MagicalPower).Value * Ability.AbilityMagicalRatio));

        if (CanBounce) BounceOnOtherNearTarget();
    }

    private void ApplyProjectileAutoAttackDamage(EntityStats targetStat)
    {
        Debug.Log("No ability");

        targetStat.TakeDamage(
            ProjectileSender,
            targetStat.GetStat(StatType.PhysicalResistances).Value,
            targetStat.GetStat(StatType.MagicalResistances).Value,
            TotalPhysicalDamage,
            TotalMagicalDamage,
            ProjectileSenderStats.GetStat(StatType.CriticalStrikeChance).Value,
            175f,
            ProjectileSenderStats.GetStat(StatType.PhysicalPenetration).Value,
            ProjectileSenderStats.GetStat(StatType.MagicalPenetration).Value);

        Debug.Log(TotalPhysicalDamage);
        Debug.Log(TotalMagicalDamage);

        InstantiateHitEffectOnCollision(OnHitVFX);
        Destroy(gameObject);
    }

    private void ApplyProjectileDamageToAnInteractiveBuilding(Collider targetCollider)
    {
        targetCollider.gameObject.GetComponent<SteleLogic>().TakeDamage(ProjectileSender, 0, 0, 1, 0, 0, 0, 0, 0);

        InstantiateHitEffectOnCollision(OnHitVFX);
        Destroy(gameObject);
    }

    protected void ApplyStatusEffectOnHit(Collider targetCollider)
    {
        StatusEffectHandler targetStatusEffectHandler = targetCollider.GetComponent<StatusEffectHandler>();

        if (ProjectileStatusEffect != null && targetStatusEffectHandler != null)
        {
            ProjectileStatusEffect.ApplyEffect(targetCollider.transform);
        }
    }

    protected void BounceOnOtherNearTarget()
    {
        //Find other near targets and for each of them go to it
        //Then if it hits the last nearer target, destroy

        Collider[] nearTarget = Physics.OverlapSphere(transform.position, projectileAreaOfEffect, entitiesLayerMaskValue);

        foreach (Collider targetColliders in nearTarget)
        {
            EntityStats targetStats = targetColliders.GetComponent<EntityStats>();

            if (ProjectileSenderStats.EntityTeam == targetStats.EntityTeam)
            {

            }
        }
    }

    private void InstantiateHitEffectOnCollision(GameObject objToInstantiate)
    {
        if (objToInstantiate != null)
            Instantiate(objToInstantiate, transform.position, Quaternion.identity);
    }
    #endregion
}