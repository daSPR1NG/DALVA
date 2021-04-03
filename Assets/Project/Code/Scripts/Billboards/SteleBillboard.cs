﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteleBillboard : Billboard
{
    [SerializeField] private GameObject buttonHolder;
    private SteleLogic stele;

    private void OnEnable()
    {
        stele.OnInteraction += DisplayBuildButtons;
        stele.OnEndOFInteraction += HideBuildButtons;
    }

    private void OnDisable()
    {
        stele.OnInteraction -= DisplayBuildButtons;
        stele.OnEndOFInteraction -= HideBuildButtons;
    }

    protected override void Awake()
    {
        base.Awake();
        stele = GetComponentInParent<SteleLogic>();
    }
    protected override void Start() => base.Start();

    protected override void LateUpdate() => base.LateUpdate();

    void DisplayBuildButtons()
    {
        buttonHolder.SetActive(true);
    }

    public void HideBuildButtons()
    {
        buttonHolder.SetActive(false);
    }
}
