﻿using UnityEngine;
using UnityEngine.AI;

public enum ProjectileType { None, TravelsForward, TravelsToAPosition }

[RequireComponent(typeof(Rigidbody))]
public class ProjectileLogic : MonoBehaviour
{
    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private GameObject onHitEffect;
    [SerializeField] private AudioClip onHitSound;

    [SerializeField] private Transform projectileSender;

    [SerializeField] private Transform target;
    [SerializeField] private CharacterStats targetStats;

    Vector3 targetPosition;

    public ProjectileType ProjectileType { get => projectileType; set => projectileType = value; }
    public float ProjectileSpeed { get => projectileSpeed; }
    public GameObject OnHitEffect { get => onHitEffect; }
    
    public Transform Target { get => target; set => target = value; }
    public CharacterStats TargetCharacterStats { get => targetStats; set => targetStats = value; }

    public Transform ProjectileSender { get => projectileSender; set => projectileSender = value; }
    public CharacterStats ProjectileSenderCharacterStats => ProjectileSender.GetComponent<CharacterStats>();
  
    private Rigidbody Rb => GetComponent<Rigidbody>();

    private void Start()
    {
        if (ProjectileType == ProjectileType.TravelsToAPosition)
        {
            if (Target != null)
                TargetCharacterStats = Target.GetComponent<CharacterStats>();
        }
    }

    private void FixedUpdate()
    {
        switch (projectileType)
        {
            case ProjectileType.None:
                break;
            case ProjectileType.TravelsForward:
                ProjectileTravelsForward(ProjectileSender);
                break;
            case ProjectileType.TravelsToAPosition:
                ProjectileMoveToATarget();
                break;
            default:
                break;
        }
    }

    #region Projectile Behaviours
    void ProjectileMoveToATarget()
    {
        if (Target.GetComponent<NavMeshAgent>())
        {
            targetPosition = Target.position;

            Rb.MovePosition(Vector3.MoveTowards(transform.position,
                targetPosition + new Vector3(0, Target.GetComponent<NavMeshAgent>().height / 2, 0),
                ProjectileSpeed * Time.fixedDeltaTime));

            transform.LookAt(Target);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void ProjectileTravelsForward(Transform sender)
    {
        ProjectileSender = sender;
        Rb.MovePosition(transform.position += transform.forward * (ProjectileSpeed * Time.fixedDeltaTime));
    }
    #endregion

    #region On hit behaviour
    private void OnTriggerEnter(Collider other)
    {
        OnProjectileDestruction();

        ApplyDamageOnTargetHit(other);
    }

    protected void ApplyDamageOnTargetHit(Collider targetCollider)
    {
        if (targetCollider.gameObject.GetComponent<CharacterStats>() != null)
        {
            Debug.Log("Enemy touched !");

            EntityDetection entityFound = targetCollider.gameObject.GetComponent<EntityDetection>();

            if (entityFound.TypeOfEntity == TypeOfEntity.Enemy)
            {
                Debug.Log("Projectile Applies Damage !");
                targetCollider.gameObject.GetComponent<CharacterStats>().TakeDamage(
                    ProjectileSender,
                    ProjectileSenderCharacterStats.CurrentAttackDamage,
                    ProjectileSenderCharacterStats.CurrentMagicDamage,
                    ProjectileSenderCharacterStats.CurrentCriticalStrikeChance,
                    ProjectileSenderCharacterStats.CurrentCriticalStrikeMultiplier,
                    ProjectileSenderCharacterStats.CurrentArmorPenetration,
                    ProjectileSenderCharacterStats.CurrentMagicResistancePenetration);

                Destroy(gameObject);
            }
        }
    }

    protected void OnProjectileDestruction()
    {
        PlaySoundOnCollision();
        InstantiateHitEffectOnCollision(OnHitEffect);
    }

    private void PlaySoundOnCollision()
    {
        if (onHitSound != null)
            AudioSource.PlayClipAtPoint(onHitSound, transform.position);
    }

    private void InstantiateHitEffectOnCollision(GameObject objToInstantiate)
    {
        if (objToInstantiate != null)
            Instantiate(objToInstantiate, transform.position, Quaternion.identity);
    }
    #endregion
}