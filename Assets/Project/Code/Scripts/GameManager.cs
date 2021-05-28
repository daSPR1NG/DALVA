﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PlayMod,
    Pause,
    StandbyMod,
    Victory,
    Defeat,
}

public class GameManager : MonoBehaviour
{
    public delegate void WaveSpawnHandler();
    public static event WaveSpawnHandler OnWaveSpawningSoon;

    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("COUCOU", transform);
            Destroy(this);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);

            if (Spawner == null)
                Debug.LogError("Need to add at least one spawner in the field array of --Spawner--", transform);

            //Debug.Log("COUCOU", transform);
        }
    }
    #endregion

    public static GameState GameState;

    [Header("WAVE")]
    [SerializeField] private int waveDone = 0;
    private int InternalCounter = 0;
    private bool waveCountHasBeenSet = false;

    [Header("SPAWNERS")]
    [SerializeField] private SpawnerSystem spawner;
    public SpawnerSystem Spawner { get => spawner; }
    public int WaveDone { get => waveDone; set => waveDone = value; }
    public bool WaveCountHasBeenSet { get => waveCountHasBeenSet; set => waveCountHasBeenSet = value; }
    public bool ItIsABossWave = false;
    public int RemainingMonstersValue = 0;
    private bool gameWasInPlayMod = false;

    public Transform Player { get; set; }
    
    void Start()
    {
        ShopPhase();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (UtilityClass.IsKeyPressed(KeyCode.Escape))
        {
            if (!GameIsInPause())
            {
                PauseGame();
            }
            else if (GameIsInPause())
            {
                SetGameToProperMod();
                //SetGameToPlayMod();
            }
        }
    }

    private void LateUpdate()
    {
        if(GameIsInPlayMod() && !ItIsABossWave)
            UpdateSpawnersState();
    }

    private void UpdateInternalCounter()
    {
        InternalCounter++;

        if (InternalCounter == 6)
        {
            InternalCounter = 0;
            ShopPhase();
        }
    }

    public void UpdateRemainingMonsterValue(int value)
    {
        RemainingMonstersValue += value;
        //Display & Update UI

        if (RemainingMonstersValue == 0 && ItIsABossWave)
        {
            ResetWhenBossWaveIsDone();
            //Hide UI
        }
    }

    public void ResetWhenBossWaveIsDone()
    {
        ItIsABossWave = false;
        UpdateWaveCount();
        UpdateSpawnersState();
    }

    public void UpdateWaveCount()
    {
        WaveDone++;
        UIManager.Instance.UpdateWaveCount(WaveDone);
        UpdateInternalCounter(); 
        WaveCountHasBeenSet = true;
    }

    public void UpdateSpawnersState()
    {
        if (spawner.waveState == WaveState.Standby && WaveCountHasBeenSet)
        {
            Debug.Log("Update Check", transform);
            spawner.IndexOfCurrentWave++;
            spawner.spawnEventEndedHasBeenHandled = true;
            spawner.UpdateElementsOnSpawnFinished();

            WaveCountHasBeenSet = false;
        }
    }
    

    public void ShopPhase()
    {
        SetGameToStandbyMod();
        PlayerHUDManager.Instance.OpenWindow(PlayerHUDManager.Instance.ShopWindow);
    }

    #region Game Mods
    #region Pause
    public void PauseGame()
    {
        //Save previous state here
        if (GameIsInPlayMod()) gameWasInPlayMod = true;
        else if (GameIsInStandBy()) gameWasInPlayMod = false;

        GameState = GameState.Pause;
        Time.timeScale = 0;

        UIManager.Instance.DisplayPauseMenu();
    }

    private bool GameIsInPause()
    {
        return GameState == GameState.Pause;
    }
    #endregion

    #region PlayMod
    public void SetGameToProperMod()
    {
        if (!gameWasInPlayMod) SetGameToStandbyMod();
        else if (gameWasInPlayMod) SetGameToPlayMod();
    }

    public void SetGameToPlayMod()
    {
        GameState = GameState.PlayMod;
        Time.timeScale = 1;

        UIManager.Instance.HidePauseMenu();
    }

    public bool GameIsInPlayMod()
    {
        return GameState == GameState.PlayMod;
    }
    #endregion

    #region Standby
    public void SetGameToStandbyMod()
    {
        GameState = GameState.StandbyMod;

        if (Time.timeScale != 1) Time.timeScale = 1;

        UIManager.Instance.HidePauseMenu();
    }

    public bool GameIsInStandBy()
    {
        return GameState == GameState.StandbyMod;
    }
    #endregion

    public void Victory()
    {
        GameState = GameState.Victory;
        Time.timeScale = 0;
    }

    public void Defeat()
    {
        GameState = GameState.Defeat;
        Time.timeScale = 0;
    }
    #endregion
}
