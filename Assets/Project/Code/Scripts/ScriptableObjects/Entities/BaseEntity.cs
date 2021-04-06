﻿using System.Collections.Generic;
using UnityEngine;

public enum EntityType 
{ 
    Archer, 
    Berzerk, 
    Coloss, 
    DaggerMaster, 
    Mage, 
    Priest,
    Dummy,
    MeleeMinion,
    DistanceMinion,
    SpecialMinion,
    ForestMonster,
    Boss,
    Nexus,
    Stele,
    None 
}

[CreateAssetMenu(fileName = "Entity_", menuName = "ScriptableObjects/Entities", order = 2)]
public class BaseEntity : ScriptableObject
{
    [Header("INFORMATIONS")]
    [SerializeField] private string entityName = "[Type in]";
    [SerializeField] private EntityType entityType = EntityType.None;
    public EntityType EntityType { get => entityType; }

    [SerializeField] private CombatType combatType = CombatType.None;
    [SerializeField] private List<Stat> entityStats;

    [Header("ANIMATION")]
    [SerializeField] private RuntimeAnimatorController animatorController;
    public RuntimeAnimatorController AnimatorController { get => animatorController; }
    public List<Stat> EntityStats { get => entityStats; private set => entityStats = value; }

    private void OnValidate()
    {
        for (int i = 0; i < EntityStats.Count; i++)
        {
            if (EntityStats.Count == System.Enum.GetValues(typeof(StatType)).Length)
                EntityStats[i].StatType = (StatType)System.Enum.GetValues(typeof(StatType)).GetValue(i);

            EntityStats[i].Name = EntityStats[i].StatType.ToString() + " - " + EntityStats[i].BaseValue.ToString();
        }
    }
}