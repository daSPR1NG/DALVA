﻿using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    public delegate void StatusEffectApplication(StatusEffect statusEffectApplied);
    public static event StatusEffectApplication OnApplyingStatusEffectEvent;

    private bool IsThereMoreThanOneStatusEffectApplied => allStatusEffectApplied.Count > 0;

    [SerializeField] private List<StatusEffectDurationData> allStatusEffectApplied = new List<StatusEffectDurationData>();

    [System.Serializable]
    private class StatusEffectDurationData
    {
        public StatusEffect statusEffect;
        public float duration;

        public StatusEffectDurationData(StatusEffect statusEffect, float duration)
        {
            this.statusEffect = statusEffect;
            this.duration = duration;
        }
    }

    void Update()
    {
        ApplyStatusEffectDurationOverTime();
        HandleExpiredStatusEffect();
    }

    #region Status Effect Duration Handler Section
    public void ApplyNewStatusEffect(StatusEffect newStatusEffect)
    {
        allStatusEffectApplied.Add(new StatusEffectDurationData(newStatusEffect, newStatusEffect.StatusEffectDuration));

        OnApplyingStatusEffectEvent?.Invoke(newStatusEffect);
    }

    private void ApplyStatusEffectDurationOverTime()
    {
        for (int i = 0; i < allStatusEffectApplied.Count; i++)
        {
            allStatusEffectApplied[i].duration -= Time.deltaTime;
        }
    }

    public bool AreThereSimilarExistingStatusEffects(StatusEffect newStatusEffectApplied)
    {
        for (int i = 0; i <= allStatusEffectApplied.Count; i++)
        {
            if (IsThereMoreThanOneStatusEffectApplied && allStatusEffectApplied[i].statusEffect.TypeOfEffect == newStatusEffectApplied.TypeOfEffect && allStatusEffectApplied[i].statusEffect.StatusEffectName != newStatusEffectApplied.StatusEffectName)
            {
                Debug.Log(newStatusEffectApplied.StatusEffectName + " is the same that " + allStatusEffectApplied[i].statusEffect.StatusEffectName);
                return true;
            }
        }

        return false;
    }

    public void RemoveStatusEffectOfSameTypeAlreadyApplied()
    {
        for (int i = 0; i <= allStatusEffectApplied.Count; i++)
        {
            allStatusEffectApplied[i].statusEffect.RemoveStatusEffect();
            allStatusEffectApplied[i].statusEffect.StatusEffectContainer.DestroyContainer();
            allStatusEffectApplied.Remove(allStatusEffectApplied[i]);
        }
    }

    private void HandleExpiredStatusEffect()
    {
        for (int i = allStatusEffectApplied.Count - 1; i >= 0; i--)
        {
            if (allStatusEffectApplied[i].duration <= 0)
            {
                allStatusEffectApplied[i].statusEffect.RemoveStatusEffect();
                allStatusEffectApplied.RemoveAt(i);
            }
        }
    }

    public bool IsTheDurationOfStatusEffectOver(StatusEffect statusEffect)
    {
        foreach (StatusEffectDurationData durationData in allStatusEffectApplied)
        {
            if (durationData.statusEffect == statusEffect)
            {
                Debug.Log(statusEffect.StatusEffectName + " will remain for " + durationData.duration.ToString("0.0") + " seconds before expiring!");
                return true;
            }
        }

        return false;
    }
    #endregion
}