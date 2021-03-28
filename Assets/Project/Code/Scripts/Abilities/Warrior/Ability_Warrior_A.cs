﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThrowingAbilityProjectile))]
public class Ability_Warrior_A : AbilityLogic
{

    //ADD THE VENT FOR WHEN THE CHARACTER ATTACHED TO THIS ABILITY ATTACKS
    private void OnEnable()
    {
        Interactions.OnAttacking += RemoveEffect;
    }

    private void OnDisable()
    {
        Interactions.OnAttacking -= RemoveEffect;
    }


    protected override void Update()
    {
        base.Update();
    }

    protected override void Cast()
    {
        UsedEffectIndex = 0;
        Ability.UsedAbilityEffect = Ability.AbilityEffects[(int)UsedEffectIndex /*int value qui change /rapport au shop*/ ];

        switch (UsedEffectIndex)
        {
            case AbilityEffect.I:
                AbilityBuff(Stats, StatType.BonusPhysicalPower, 20 + (Stats.GetStat(StatType.PhysicalPower).Value * 0.3f), this);
                break;
            case AbilityEffect.II:
                break;
            case AbilityEffect.III:
                break;
            case AbilityEffect.IV:
                break;
        }
    }

    void RemoveEffect()
    {
        RemoveAbilityBuff(StatType.BonusPhysicalPower, this);
    }
}