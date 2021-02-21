﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(Transform sourceOfDamage, float attackDamageTaken, float magicDamageTaken, float characterCriticalStrikeChance, float characterCriticalStrikeMultiplier, float armorPenetration, float magicResistancePenetration);
}
