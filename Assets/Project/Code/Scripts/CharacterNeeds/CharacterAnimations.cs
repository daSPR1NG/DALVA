﻿using UnityEngine;

public class CharacterAnimations : MonoBehaviour
{
    public Animator MyAnimator => GetComponent<Animator>();
    private InteractionSystem Interactions => GetComponentInParent<InteractionSystem>();
    private CharacterStats Stats => GetComponentInParent<CharacterStats>();

    private void Awake()
    {
        MyAnimator.runtimeAnimatorController = Stats.UsedCharacter.AnimatorController;
    }

    public void RangedAttack_AnimationEvent()
    {
        Interactions.RangedAttack();
    }

    public void MeleeAttack_AnimationEvent()
    {
        Interactions.MeleeAttack();
    }
}
