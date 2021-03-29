﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HarvesterBillboard : BillBoard
{
    [Header("HARVESTER BILLBOARD INFORMATIONS")]
    [SerializeField] private TextMeshProUGUI harvestedRessourcesText;
    [SerializeField] private Image filledImage;

    private void OnEnable()
    {
        HarvesterLogic.OnHarvestingRessources += SetHarvesterUIElements;
    }

    private void OnDisable()
    {
        HarvesterLogic.OnHarvestingRessources -= SetHarvesterUIElements;
    }

    protected override void Awake() => base.Awake();

    protected override void LateUpdate() => base.LateUpdate();

    private void SetHarvesterUIElements(float current, float maximum)
    {
        filledImage.fillAmount = current / maximum;
        harvestedRessourcesText.text = current.ToString("0") + " / " + maximum.ToString();
    }
}