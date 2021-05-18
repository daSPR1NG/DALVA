﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private List<SpawnerSystem> spawners = new List<SpawnerSystem>();

    [Header("WAVE COUNT")]
    [SerializeField] private TextMeshProUGUI placeToDefendHealtAmountText;

    [Header("WAVE")]
    [SerializeField] private GameObject waveIndicationUI;
    [SerializeField] private GameObject waveDisplayerParent;

    [Header("TIMER")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("WAVE COUNT")]
    [SerializeField] private TextMeshProUGUI waveCountText;

    float timeValue = 0f;

    #region Singleton
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private void OnEnable()
    {
        PlaceToDefend.OnHealthValueChanged += UpdatePlaceToDefendHealth;
    }

    private void OnDisable()
    {
        PlaceToDefend.OnHealthValueChanged -= UpdatePlaceToDefendHealth;
    }

    private void Start()
    {
        timerText.SetText("00 : 00");
        PopulateSpawnersList();

        UpdapteGameTimer();
    }

    private void LateUpdate() => UpdapteGameTimer();

    void PopulateSpawnersList()
    {
        foreach (SpawnerSystem spawner in GameManager.Instance.Spawners)
        {
            spawners.Add(spawner);
        }
    }

    void UpdapteGameTimer()
    {
        if (!GameManager.Instance.GameIsInPlayMod()) return;

        timeValue += Time.deltaTime;

        string minutes = Mathf.Floor(timeValue / 60).ToString("0");
        string seconds = Mathf.Floor(timeValue % 60).ToString("00");

        timerText.SetText(minutes + " : " + seconds);
    }

    public void UpdateWaveCount(int amnt)
    {
        waveCountText.SetText(amnt.ToString("0"));
    }

    public void UpdatePlaceToDefendHealth(int amnt)
    {
        placeToDefendHealtAmountText.SetText(amnt.ToString("0"));
    }
}